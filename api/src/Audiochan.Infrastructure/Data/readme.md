# Audiochan.Data

This project is the data-access layer (DAL). Since this project uses Entity Framework Core, it contains the database context (Unit of Work) for access data from our database. The project also contains migrations, configurations, and extension methods for shaping the data being pulled from the database context.

## Commands

### Updating Migrations

`dotnet ef migrations add InitialMigration -p src/Audiochan.Infrastructure -s src/Audiochan.Web -o Data/Migrations`

### Update database

`dotnet ef database update -s src/Audiochan.Web`

### Remove migration

`dotnet ef migrations remove -p src/Audiochan.Infrastructure/Data -s src/Audiochan.Web`