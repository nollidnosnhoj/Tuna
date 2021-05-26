using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdatePicture
{
    public record UpdateUserPictureRequest : IRequest<IResult<ProfileViewModel>>
    {
        [JsonIgnore] public string UserId { get; set; } = string.Empty;
        public string Data { get; init; } = null!;
    }

    public class UpdateUserPictureRequestHandler : IRequestHandler<UpdateUserPictureRequest, IResult<ProfileViewModel>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IStorageService _storageService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserPictureRequestHandler(IOptions<MediaStorageSettings> options,
            IImageProcessingService imageProcessingService,
            IStorageService storageService,
            IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _storageSettings = options.Value;
            _imageProcessingService = imageProcessingService;
            _storageService = storageService;
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<ProfileViewModel>> Handle(UpdateUserPictureRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var container = string.Join('/', _storageSettings.Image.Container, "users");
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            
            if (user == null) 
                return Result<ProfileViewModel>.Fail(ResultError.NotFound);
        
            if (user.Id != _currentUserService.GetUserId())
                return Result<ProfileViewModel>.Fail(ResultError.Forbidden);
        
            var blobName = $"{user.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";
            
            await _imageProcessingService.UploadImage(request.Data, container, blobName, cancellationToken);
            
            if (!string.IsNullOrEmpty(user.Picture))
                await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, container, user.Picture, cancellationToken);
            
            user.UpdatePicture(blobName);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProfileViewModel>.Success(user.MapToProfile(currentUserId));
        }
    }
}