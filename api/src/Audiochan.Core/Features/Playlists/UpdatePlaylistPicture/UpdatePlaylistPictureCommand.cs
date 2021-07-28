using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Playlists.UpdatePlaylistPicture
{
    public record UpdatePlaylistPictureCommand : IRequest<Result<ImageUploadResponse>>
    {
        public Guid Id { get; init; }
        public string Data { get; init; } = null!;

        public UpdatePlaylistPictureCommand(Guid id, ImageUploadRequest request)
        {
            Id = id;
            Data = request.Data;
        }
    }
    
    
    public class UpdatePlaylistPictureCommandHandler : IRequestHandler<UpdatePlaylistPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly string _currentUserId;
        private readonly MediaStorageSettings _storageSettings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IStorageService _storageService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ApplicationDbContext _unitOfWork;
        private readonly ICacheService _cacheService;

        public UpdatePlaylistPictureCommandHandler(IOptions<MediaStorageSettings> options,
            IStorageService storageService,
            IImageProcessingService imageProcessingService,
            IDateTimeProvider dateTimeProvider, 
            ApplicationDbContext unitOfWork, 
            ICacheService cacheService, ICurrentUserService currentUserService)
        {
            _storageSettings = options.Value;
            _storageService = storageService;
            _imageProcessingService = imageProcessingService;
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _currentUserId = currentUserService.GetUserId();
        }
        
        public async Task<Result<ImageUploadResponse>> Handle(UpdatePlaylistPictureCommand request, CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "playlists");

            var playlist = await _unitOfWork.Playlists
                .Include(p => p.User)
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (playlist == null)
                return Result<ImageUploadResponse>.NotFound<Playlist>();

            if (playlist.UserId != _currentUserId)
                return Result<ImageUploadResponse>.Forbidden();
            
            var blobName = $"{playlist.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";
            
            await _imageProcessingService.UploadImage(request.Data, container, blobName, cancellationToken);

            if (!string.IsNullOrEmpty(playlist.Picture))
                await _storageService.RemoveAsync(_storageSettings.Image.Bucket, container, playlist.Picture,
                    cancellationToken);

            playlist.Picture = blobName;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = string.Format(MediaLinkInvariants.PlaylistPictureUrl, blobName)
            });
        }
    }
}