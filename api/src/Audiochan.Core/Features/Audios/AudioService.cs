using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Builders;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Audios.Extensions;
using Audiochan.Core.Features.Audios.Mappings;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;
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
        private readonly IImageService _imageService;

        public AudioService(IDatabaseContext dbContext, 
            ICurrentUserService currentUserService, 
            IAudioMetadataService audioMetadataService, 
            IStorageService storageService,
            IGenreService genreService, 
            IImageService imageService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _audioMetadataService = audioMetadataService;
            _storageService = storageService;
            _genreService = genreService;
            _imageService = imageService;
        }

        public async Task<PagedList<AudioListViewModel>> GetFeed(string userId, PaginationQuery query,
            CancellationToken cancellationToken = default)
        {
            // Get the user Ids of the followed users
            var followedIds = await _dbContext.FollowedUsers
                .AsNoTracking()
                .Where(user => user.ObserverId == userId)
                .Select(user => user.TargetId)
                .ToListAsync(cancellationToken);

            return await _dbContext.Audios
                .AsNoTracking()
                .Include(a => a.Favorited)
                .Include(a => a.User)
                .FilterVisibility(userId)
                .Where(a => followedIds.Contains(a.UserId))
                .Distinct()
                .Select(AudioListMapping.Map(userId))
                .OrderByDescending(a => a.Created)
                .Paginate(query, cancellationToken);
        }

        public async Task<PagedList<AudioListViewModel>> GetList(GetAudioListQuery query, 
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
                .Select(AudioListMapping.Map(currentUserId))
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
                .Select(AudioDetailMapping.Map(currentUserId))
                .SingleOrDefaultAsync(cancellationToken);

            return audio == null 
                ? Result<AudioDetailViewModel>.Fail(ResultStatus.NotFound) 
                : Result<AudioDetailViewModel>.Success(audio);
        }

        public async Task<IResult<AudioDetailViewModel>> GetRandom(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();
            var count = await _dbContext.Audios.CountAsync(cancellationToken);
            Random rand = new();
            var offset = rand.Next(0, count);
            var audio = await _dbContext.Audios
                .AsNoTracking()
                .Include(a => a.Favorited)
                .Include(a => a.Tags)
                .Include(a => a.User)
                .Include(a => a.Genre)
                .FilterVisibility(currentUserId)
                .Skip(offset)
                .Select(AudioDetailMapping.Map(currentUserId))
                .SingleOrDefaultAsync(cancellationToken);

            return audio == null 
                ? Result<AudioDetailViewModel>.Fail(ResultStatus.NotFound) 
                : Result<AudioDetailViewModel>.Success(audio);
        }

        public async Task<IResult<AudioDetailViewModel>> Create(UploadAudioRequest request, 
            CancellationToken cancellationToken = default)
        {
            var audioBuilder = new AudioBuilder(request.File);
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var currentUser = await _dbContext.Users
                    .SingleOrDefaultAsync(u => u.Id == _currentUserService.GetUserId(), cancellationToken);

                var memoryStream = new MemoryStream();
                await request.File.CopyToAsync(memoryStream, cancellationToken);

                var audioMetadata = _audioMetadataService.GetMetadata(memoryStream, request.File.ContentType);

                await _storageService.SaveBlobAsync(
                    ContainerConstants.Audios,
                    audioBuilder.GetBlobName(),
                    memoryStream,
                    cancellationToken: cancellationToken);

                var audioBlob = await _storageService
                    .GetBlobAsync(ContainerConstants.Audios, audioBuilder.GetBlobName(), cancellationToken);

                var imageBlob = request.Image != null
                    ? await _imageService.UploadAudioImage(request.Image, audioBuilder.GetId(), cancellationToken)
                    : null;

                // Get Genre for audio
                var genre = await _genreService.GetGenre(request.Genre ?? "misc", cancellationToken);
                if (genre == null)
                    return Result<AudioDetailViewModel>
                        .Fail(ResultStatus.BadRequest, "Genre does not exist.");

                // Generate tags for audio
                var tags = request.Tags.Count > 0
                    ? await CreateNewTags(request.Tags, cancellationToken)
                    : new List<Tag>();

                audioBuilder = audioBuilder
                    .AddTitle(request.Title)
                    .AddDescription(request.Description)
                    .AddAudioMetadata(audioMetadata)
                    .AddBlobInfo(audioBlob)
                    .AddImage(imageBlob)
                    .SetToPublic(request.IsPublic)
                    .SetToLoop(request.IsLoop)
                    .AddGenre(genre)
                    .AddTags(tags)
                    .AddUser(currentUser);

                var audio = audioBuilder.Build();
                await _dbContext.Audios.AddAsync(audio, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return Result<AudioDetailViewModel>.Success(audio.MapToDetail(currentUser.Id));
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException)
                    await transaction.RollbackAsync(cancellationToken);
                await _storageService.DeleteBlobAsync(ContainerConstants.Audios, audioBuilder.GetBlobName(), cancellationToken);
                throw; 
            }
        }

        public async Task<IResult<AudioDetailViewModel>> Update(string audioId, UpdateAudioRequest request, 
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
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
                        return Result<AudioDetailViewModel>
                            .Fail(ResultStatus.BadRequest, "Genre does not exist.");

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
                await transaction.CommitAsync(cancellationToken);
                return Result<AudioDetailViewModel>.Success(audio.MapToDetail(currentUserId));
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException)
                    await transaction.RollbackAsync(cancellationToken);
                throw;
            }
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
            
            var blobName = audio.Id + audio.AudioFileExtension;

            _dbContext.Audios.Remove(audio);
            var task1 = _dbContext.SaveChangesAsync(cancellationToken);
            var task2 = _storageService.DeleteBlobAsync(ContainerConstants.Audios, blobName, cancellationToken);
            var task3 = _imageService.RemoveAudioImages(id, cancellationToken);
            await Task.WhenAll(task1, task2, task3);
            return Result.Success();
        }

        public async Task<IResult<string>> AddPicture(string audioId, IFormFile file,
            CancellationToken cancellationToken = default)
        {
            var audio = await _dbContext.Audios
                .SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);

            if (audio == null)
                return Result<string>.Fail(ResultStatus.NotFound, "Audio was not found.");
            
            var blobDto = await _imageService
                .UploadAudioImage(file, audio.Id, cancellationToken);

            audio.PictureUrl = blobDto.Url;
            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result<string>.Success(blobDto.Url);
        }
        
        public async Task<PagedList<PopularTagViewModel>>  GetPopularTags(
            PaginationQuery paginationQuery
            , CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tags
                .AsNoTracking()
                .Include(t => t.Audios)
                .Select(t => new PopularTagViewModel{ Tag = t.Id, Count = t.Audios.Count })
                .OrderByDescending(dto => dto.Count)
                .Paginate(paginationQuery, cancellationToken);
        }
        
        private async Task<List<Tag>> CreateNewTags(IEnumerable<string> requestedTags, 
            CancellationToken cancellationToken = default)
        {
            var taggifyTags = requestedTags.FormatTags();

            var tags = await _dbContext.Tags
                .Where(tag => taggifyTags.Contains(tag.Id))
                .ToListAsync(cancellationToken);
            
            foreach (var tag in taggifyTags.Where(tag => tags.All(t => t.Id != tag)))
            {
                tags.Add(new Tag{Id = tag});
            }

            return tags;
        }
    }
}