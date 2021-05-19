using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdatePicture
{
    public record UpdateUserPictureRequest : IRequest<IResult<string>>
    {
        [JsonIgnore] public string? UserId { get; set; }
        public string Data { get; init; } = null!;
    }

    public class UpdateUserPictureRequestHandler : IRequestHandler<UpdateUserPictureRequest, IResult<string>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly UserManager<User> _userManager;
        private readonly IImageService _imageService;
        private readonly IStorageService _storageService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserPictureRequestHandler(IOptions<MediaStorageSettings> options,
            UserManager<User> userManager,
            IImageService imageService,
            IStorageService storageService,
            IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService)
        {
            _storageSettings = options.Value;
            _userManager = userManager;
            _imageService = imageService;
            _storageService = storageService;
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<string>> Handle(UpdateUserPictureRequest request, CancellationToken cancellationToken)
        {
            var container = string.Join('/', _storageSettings.Image.Container, "users");
            
            var user = await _userManager.FindByIdAsync(request.UserId + "");
            if (user == null) return Result<string>.Fail(ResultError.NotFound);
            if (user.Id != _currentUserService.GetUserId())
                return Result<string>.Fail(ResultError.Forbidden);
            
            var blobName = $"{user.Id}/{_dateTimeProvider.Now:yyyyMMddHHmmss}.jpg";
            
            try
            {
                var response = await _imageService.UploadImage(request.Data, container, blobName, cancellationToken);
                
                if (!string.IsNullOrEmpty(user.Picture))
                    await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, user.Picture, cancellationToken);
                
                user.UpdatePicture(blobName);
                await _userManager.UpdateAsync(user);
                return Result<string>.Success(response.Url);
            }
            catch (Exception)
            {
                await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, container, blobName, cancellationToken);
                throw;
            }
        }
    }
}