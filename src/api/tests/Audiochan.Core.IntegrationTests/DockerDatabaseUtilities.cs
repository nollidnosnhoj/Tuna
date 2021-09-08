using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Npgsql;

namespace Audiochan.Core.IntegrationTests
{
    // https://wrapt.dev/blog/integration-tests-using-sql-server-db-in-docker
    // https://blog.dangl.me/archive/running-sql-server-integration-tests-in-net-core-projects-via-docker/
    public static class DockerDatabaseUtilities
    {
        public const string DB_PASSWORD = "justsomepassword!%1999";
        private const string DB_USER = "postgres";
        private const string DB_NAME = "Audiochan_Test_DB";
        private const string DB_IMAGE = "postgres:latest";
        private const string DB_CONTAINER_NAME = "Audiochan_IntegrationTest";
        private const string DB_VOLUME_NAME = "Audiochan_IntegrationTest";
        private const int PROVIDER_PORT = 5432;

        public static async Task<int> EnsureDockerStartedAsync()
        {
            await CleanupRunningContainers();
            await CleanupRunningVolumes();
            
            var freePort = GetFreePort();

            var hosts = new Hosts().Discover();
            var docker = hosts.FirstOrDefault(x => x.IsNative) 
                         ?? hosts.FirstOrDefault(x => x.Name == "default");

            var volume = docker?.GetVolumes().FirstOrDefault(v => v.Name == DB_VOLUME_NAME)
                         ?? new Builder().UseVolume().WithName(DB_VOLUME_NAME).Build();

            var container = docker?.GetContainers()
                .FirstOrDefault(c => c.Name == DB_CONTAINER_NAME);

            if (container != null)
            {
                return container.ToHostExposedEndpoint($"{PROVIDER_PORT}/tcp").Port;
            }

            container = new Builder().UseContainer()
                .WithName(DB_CONTAINER_NAME)
                .UseImage(DB_IMAGE)
                .ExposePort(freePort, PROVIDER_PORT)
                .WithEnvironment($"POSTGRES_DB={DB_NAME}")
                .WithEnvironment($"POSTGRES_PASSWORD={DB_PASSWORD}")
                .WaitForPort($"{PROVIDER_PORT}/tcp", 30000)
                .MountVolume(volume, "/var/lib/postgresql/data", MountType.ReadWrite)
                .Build();

            container.Start();

            await WaitUntilDatabaseAvailableAsync(freePort);
            return freePort;
        }

        public static string GetSqlConnectionString(int port)
        {
            return new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Password = DB_PASSWORD,
                Username = DB_USER,
                Database = DB_NAME,
                Port = port
            }.ToString();
        }

        private static bool IsRunningOnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        private static DockerClient GetDockerClient()
        {
            var dockerUri = IsRunningOnWindows()
                ? "npipe://./pipe/docker_engine"
                : "unix:///var/run/docker.sock";
            return new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();
        }

        private static async Task CleanupRunningContainers(int hoursTillExpiration = -24)
        {
            var dockerClient = GetDockerClient();

            var runningContainers = await dockerClient.Containers
                .ListContainersAsync(new ContainersListParameters());

            foreach (var runningContainer in runningContainers.Where(c =>
                c.Names.Any(n => n.Contains(DB_CONTAINER_NAME))))
            {
                var expiration = hoursTillExpiration > 0 ? hoursTillExpiration * -1 : hoursTillExpiration;

                if (runningContainer.Created >= DateTime.UtcNow.AddHours(expiration)) continue;
                
                try
                {
                    await EnsureDockerContainersStoppedAndRemovedAsync(runningContainer.ID);
                }
                catch
                {
                    // Ignoring failures to stop running containers
                }
            }
        }

        private static async Task CleanupRunningVolumes(int hoursTillExpiration = -24)
        {
            var dockerClient = GetDockerClient();

            var runningVolumes = await dockerClient.Volumes.ListAsync();

            foreach (var runningVolume in runningVolumes.Volumes.Where(v => v.Name == DB_VOLUME_NAME))
            {
                // Stopping all test volumes that are older than 24 hours
                var expiration = hoursTillExpiration > 0
                    ? hoursTillExpiration * -1
                    : hoursTillExpiration;

                if (DateTime.Parse(runningVolume.CreatedAt) >= DateTime.UtcNow.AddHours(expiration)) continue;
                
                try
                {
                    await EnsureDockerVolumesRemovedAsync(runningVolume.Name);
                }
                catch
                {
                    // Ignoring failures to stop running volumes
                }
            }
        }

        private static async Task EnsureDockerContainersStoppedAndRemovedAsync(string dockerContainerId)
        {
            var dockerClient = GetDockerClient();

            await dockerClient.Containers
                .StopContainerAsync(dockerContainerId, new ContainerStopParameters());

            await dockerClient.Containers
                .RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters());
        }

        private static async Task EnsureDockerVolumesRemovedAsync(string volumeName)
        {
            var dockerClient = GetDockerClient();
            await dockerClient.Volumes.RemoveAsync(volumeName);
        }

        private static async Task WaitUntilDatabaseAvailableAsync(int databasePort)
        {
            var start = DateTime.UtcNow;
            const int maxWithTimeSeconds = 60;
            var connectionEstablished = false;

            while (!connectionEstablished && start.AddSeconds(maxWithTimeSeconds) > DateTime.UtcNow)
            {
                try
                {
                    var connectionString = GetSqlConnectionString(databasePort);
                    await using var connection = new NpgsqlConnection(connectionString);
                    await connection.OpenAsync();
                    connectionEstablished = true;
                }
                catch
                {
                    await Task.Delay(500);
                }
            }

            if (!connectionEstablished)
            {
                throw new Exception(
                    $"Connection to the PostgreSQL docker database could not be established within {maxWithTimeSeconds} seconds.");
            }
        }

        private static int GetFreePort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }
    }
}