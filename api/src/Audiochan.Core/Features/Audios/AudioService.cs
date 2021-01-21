using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Extensions.Queryable;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios
{
    public class AudioService : IAudioService
    {
        private readonly IDatabaseContext _dbContext;
        private readonly IAudioMetadataService _audioMetadataService;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGenreService _genreService;

        public AudioService(IDatabaseContext dbContext, 
            ICurrentUserService currentUserService, 
            IAudioMetadataService audioMetadataService, 
            IStorageService storageService,
            IGenreService genreService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _audioMetadataService = audioMetadataService;
            _storageService = storageService;
            _genreService = genreService;
        }

        public async Task<List<AudioListViewModel>> GetFeed(long userId, PaginationQuery query,
            CancellationToken cancellationToken = default)
        {
            // Get the user Ids of the followed users
            var followedIds = await _dbContext.FollowedUsers
                .AsNoTracking()
                .Where(user => user.ObserverId == userId)
                .Select(user => user.TargetId)
                .ToListAsync(cancellationToken);

            var queryable = _dbContext.Audios
                .AsNoTracking()
                .Include(a => a.Favorited)
                .Include(a => a.User)
                .FilterVisibility(userId)
                .Where(a => followedIds.Contains(a.UserId))
                .Distinct()
                .Select(MapProjections.AudioList(userId))
                .OrderByDescending(a => a.Created);

            return await queryable.Paginate(query, cancellationToken);
        }

        public async Task<List<AudioListViewModel>> GetList(GetAudioListQuery query, 
            CancellationToken cancellationToken = default)
        {
            // Get userId of the current user
            var currentUserId = _currentUserService.GetUserId();

            // Build query
            var queryable = _dbContext.Audios
                .AsNoTracking()
                .Include(a => a.Favorited)
                .Include(a => a.User)
                .Include(a => a.Genre)
                .FilterVisibility(currentUserId);

            if (!string.IsNullOrWhiteSpace(query.Username))
                queryable = queryable.Where(a => a.User.UserName == query.Username);
            
            if (!string.IsNullOrWhiteSpace(query.Tags))
                queryable = queryable.FilterByTags(query.Tags);

            if (!string.IsNullOrWhiteSpace(query.Genre))
                queryable = queryable.FilterByGenre(query.Genre);
            
            queryable = queryable.Sort(query.Sort.ToLower());

            return await queryable
                .Select(MapProjections.AudioList(currentUserId))
                .Paginate(query, cancellationToken);
        }

        public async Task<IResult<AudioDetailViewModel>> Get(string audioId, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var audio = await _dbContext.Audios
                .AsNoTracking()
                .Include(a => a.Favorited)
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Include(a => a.Genre)
                .FilterVisibility(currentUserId)
                .Where(x => x.Id == audioId)
                .Select(MapProjections.AudioDetail(currentUserId))
                .SingleOrDefaultAsync(cancellationToken);

            return audio == null 
                ? Result<AudioDetailViewModel>.Fail(ResultStatus.NotFound) 
                : Result<AudioDetailViewModel>.Success(audio);
        }

        public async Task<string?> GetRandomAudioId(CancellationToken cancellationToken = default)
        {
            await using var dbConnection = _dbContext.Database.GetDbConnection();
            const string queryString = "SELECT id FROM audios ORDER BY random()";
            var id = dbConnection.QueryFirstOrDefault<string>(queryString, cancellationToken);
            return id;
        }

        public async Task<IResult<AudioDetailViewModel>> Create(UploadAudioRequest request, 
            CancellationToken cancellationToken = default)
        {
            var currentUser = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == _currentUserService.GetUserId(), cancellationToken);

            // Start building audio
            var audioBuilder = new AudioBuilder(request.File);

            // Create memory stream for audio file
            var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream, cancellationToken);
            
            // This gets the metadata of the audio file, including duration, bitrate, etc.
            var audioMetadata = _audioMetadataService
                .GetMetadata(memoryStream, request.File.ContentType);
            
            // Get Genre for audio
            var genre = await _genreService.GetGenre(request.Genre ?? "misc", cancellationToken);
            if (genre == null)
                return Result<AudioDetailViewModel>.Fail(ResultStatus.BadRequest, "Genre does not exist.");
            
            // Generate tags for audio
            var tags = request.Tags.Count > 0
                ? await CreateNewTags(request.Tags, cancellationToken)
                : new List<Tag>();

            // Start uploading the audio stream into a storage (file or cloud)
            var blobName = audioBuilder.GetBlobName();
            await _storageService.SaveBlobAsync(
                ContainerConstants.Audios, 
                blobName, 
                memoryStream, 
                cancellationToken: cancellationToken);
            var blob = await _storageService.GetBlobAsync(ContainerConstants.Audios, blobName, cancellationToken);

            audioBuilder = audioBuilder
                .AddTitle(request.Title)
                .AddDescription(request.Description)
                .AddGenre(genre)
                .AddTags(tags)
                .AddAudioMetadata(audioMetadata)
                .AddBlobInfo(blob)
                .SetToPublic(request.IsPublic)
                .SetToLoop(request.IsLoop)
                .AddUser(currentUser);

            try
            {
                var audio = audioBuilder.Build();
                await _dbContext.Audios.AddAsync(audio, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result<AudioDetailViewModel>.Success(AudioDetailViewModel.From(audio, currentUser.Id));
            }
            catch (Exception)
            {
                // delete blob if an unknown error occurs
                await _storageService.DeleteBlobAsync(blob.Container, blob.Name, cancellationToken);
                
                throw;
            }  
        }

        public async Task<IResult<AudioDetailViewModel>> Update(string audioId, UpdateAudioRequest request, 
            CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var audio = await _dbContext.Audios
                .Include(a => a.Favorited)
                .Include(a => a.User)
                .Include(a => a.Tags)
                .Include(a => a.Genre)
                .SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);

            if (audio == null) return Result<AudioDetailViewModel>.Fail(ResultStatus.NotFound);

            if (audio.UserId != currentUserId) return Result<AudioDetailViewModel>.Fail(ResultStatus.Forbidden);
            
            var newTags = await CreateNewTags(request.Tags, cancellationToken);

            audio.Title = request.Title ?? audio.Title;
            audio.Description = request.Description ?? audio.Description;
            audio.IsPublic = request.IsPublic ?? audio.IsPublic;
            audio.IsLoop = request.IsLoop ?? audio.IsLoop;

            if (!string.IsNullOrWhiteSpace(request.Genre) && audio.Genre.Name != request.Genre)
            {
                var genre = await _genreService.GetGenre(request.Genre, cancellationToken);

                if (genre == null)
                    return Result<AudioDetailViewModel>.Fail(ResultStatus.BadRequest, "Genre does not exist.");
                
                audio.Genre = genre!;
            }

            foreach (var audioTag in audio.Tags)
            {
                if (newTags.All(t => t.Id != audioTag.Id))
                    audio.Tags.Remove(audioTag);
            }

            foreach (var newTag in newTags)
            {
                if (audio.Tags.All(t => t.Id != newTag.Id))
                    audio.Tags.Add(newTag);
            }

            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<AudioDetailViewModel>.Success(AudioDetailViewModel.From(audio, currentUserId));
        }

        public async Task<IResult> Remove(string id, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _dbContext.Audios
                .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (audio == null)
                return Result.Fail(ResultStatus.NotFound);

            if (audio.UserId != currentUserId)
                return Result.Fail(ResultStatus.Forbidden);

            var blobName = audio.Id + audio.FileExt;
            
            _dbContext.Audios.Remove(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            await _storageService.DeleteBlobAsync(ContainerConstants.Audios, blobName, cancellationToken);

            return Result.Success();
        }
        
        public async Task<IResult<List<PopularTagViewModel>>>  GetPopularTags(
            PaginationQuery paginationQuery
            , CancellationToken cancellationToken = default)
        {
            var queryable = _dbContext.Tags
                .AsNoTracking()
                .Include(t => t.Audios)
                .Select(t => new PopularTagViewModel{ Tag = t.Id, Count = t.Audios.Count })
                .OrderByDescending(dto => dto.Count);

            var vm = await queryable.Paginate(
                paginationQuery.Page
                , paginationQuery.Limit
                , cancellationToken);

            return Result<List<PopularTagViewModel>>.Success(vm);
        }
        
        private async Task<List<Tag>> CreateNewTags(IEnumerable<string?> requestedTags, 
            CancellationToken cancellationToken = default)
        {
            var taggifyTags = requestedTags.FormatTags();

            var existingTags = await _dbContext.Tags
                .Where(tag => taggifyTags.Contains(tag.Id))
                .ToListAsync(cancellationToken);
            
            var newTags = new List<Tag>();

            foreach (var tag in taggifyTags)
            {
                if (existingTags.All(t => t.Id != tag))
                    newTags.Add(new Tag{Id = tag});
            }

            await _dbContext.Tags.AddRangeAsync(newTags, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return await _dbContext.Tags
                .Where(tag => taggifyTags.Contains(tag.Id))
                .ToListAsync(cancellationToken);
        }
    }
}