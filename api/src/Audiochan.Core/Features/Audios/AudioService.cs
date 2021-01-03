using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IAudiochanContext _dbContext;
        private readonly IAudioMetadataService _audioMetadataService;
        private readonly IStorageService _storageService;
        private readonly ITagService _tagService;
        private readonly ICurrentUserService _currentUserService;

        public AudioService(IAudiochanContext dbContext, 
            ICurrentUserService currentUserService, 
            IAudioMetadataService audioMetadataService, 
            IStorageService storageService, ITagService tagService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _audioMetadataService = audioMetadataService;
            _storageService = storageService;
            _tagService = tagService;
        }

        public async Task<IResult<List<AudioListViewModel>>> GetFeed(long userId, PaginationQuery query,
            CancellationToken cancellationToken = default)
        {
            if (userId == 0)
                return Result<List<AudioListViewModel>>.Fail(ResultErrorCode.Unauthorized);
            
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

            var result = await queryable
                .Paginate(query, cancellationToken);

            return Result<List<AudioListViewModel>>.Success(result);
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
                .FilterVisibility(currentUserId);

            if (!string.IsNullOrWhiteSpace(query.Username))
                queryable = queryable.Where(a => a.User.UserName == query.Username);
            
            if (!string.IsNullOrWhiteSpace(query.Tags))
                queryable = queryable.FilterByTags(query.Tags);
            
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
                .FilterVisibility(currentUserId)
                .Where(x => x.Id == audioId)
                .Select(MapProjections.AudioDetail(currentUserId))
                .SingleOrDefaultAsync(cancellationToken);

            if (audio == null)
                return Result<AudioDetailViewModel>.Fail(ResultErrorCode.NotFound);

            var blob = await _storageService
                .GetBlobAsync(ContainerConstants.Audios, audio.Id + audio.FileExt, cancellationToken);

            audio.Url = blob.FoundBlob ? blob.Url : "";

            return Result<AudioDetailViewModel>.Success(audio);
        }

        public async Task<string> GetRandomAudioId(CancellationToken cancellationToken = default)
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
            
            // Create memory stream for uploaded audio file
            var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream, cancellationToken);

            // Get audio's metadata
            var data = _audioMetadataService.GetMetadata(memoryStream, request.File.ContentType);

            var id = Guid.NewGuid().ToString();
            var ext = Path.GetExtension(request.File.FileName);
            var blobName = id + ext;

            // Upload audio to storage
            await _storageService.SaveBlobAsync(
                ContainerConstants.Audios, 
                blobName, 
                memoryStream, 
                cancellationToken: cancellationToken);

            var blob = await _storageService
                .GetBlobAsync(ContainerConstants.Audios, blobName, cancellationToken);

            var title = request.Title ?? (string.IsNullOrWhiteSpace(data.Title)
                ? Path.GetFileNameWithoutExtension(request.File.FileName)
                : data.Title);

            if (string.IsNullOrEmpty(title))
            {
                return Result<AudioDetailViewModel>
                    .Fail(ResultErrorCode.BadRequest,
                        "Cannot obtain the title of song, please provide a title in the request.");
            }

            try
            {
                var audio = new Audio
                {
                    Id = id,
                    IsUploaded = true,
                    Title = title,
                    Description = request.Description ?? "",
                    IsPublic = request.IsPublic ?? true,
                    Duration = data.Duration,
                    FileSize = blob.Size,
                    User = currentUser,
                    FileExt = ext
                };

                if (request.Tags.Count > 0)
                {
                    var tags = await _tagService.CreateNewTags(request.Tags, cancellationToken);
                    audio.Tags = tags.Select(tag => new AudioTag {Tag = tag, Audio = audio}).ToList();
                }

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
                .SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);

            if (audio == null) return Result<AudioDetailViewModel>.Fail(ResultErrorCode.NotFound);

            if (audio.UserId != currentUserId) return Result<AudioDetailViewModel>.Fail(ResultErrorCode.Forbidden);

            // If values are null, do not change property
            
            audio.Title = request.Title ?? audio.Title;
            audio.Description = request.Description ?? audio.Description;
            audio.IsPublic = request.IsPublic ?? audio.IsPublic;
            
            var newTags = await _tagService.CreateNewTags(request.Tags, cancellationToken);

            var tagsToDelete = new List<AudioTag>();

            // Get audio tags to remove
            foreach (var audioTag in audio.Tags)
            {
                if (newTags.All(t => t.Id != audioTag.TagId))
                {
                    tagsToDelete.Add(audioTag);
                }
            }

            // Remove audio tags from audio
            foreach (var deleteTag in tagsToDelete)
            {
                if (audio.Tags.Any(x => x.TagId == deleteTag.TagId))
                {
                    audio.Tags.Remove(deleteTag);
                }
            }

            // Add the new audio tags
            foreach (var newTag in newTags)
            {
                if (audio.Tags.All(at => at.TagId != newTag.Id))
                {
                    audio.Tags.Add(new AudioTag
                    {
                        AudioId = audio.Id,
                        TagId = newTag.Id
                    });
                }
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
                return Result.Fail(ResultErrorCode.NotFound);

            if (audio.UserId != currentUserId)
                return Result.Fail(ResultErrorCode.Forbidden);

            var blobName = audio.Id + audio.FileExt;
            
            _dbContext.Audios.Remove(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            await _storageService.DeleteBlobAsync(ContainerConstants.Audios, blobName, cancellationToken);

            return Result.Success();
        }
    }
}