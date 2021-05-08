﻿using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Helpers;
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
        [JsonIgnore] public string UserId { get; set; }
        public string Data { get; init; }
    }

    public class UpdateUserPictureRequestHandler : IRequestHandler<UpdateUserPictureRequest, IResult<string>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly UserManager<User> _userManager;
        private readonly IImageService _imageService;
        private readonly IStorageService _storageService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UpdateUserPictureRequestHandler(IOptions<MediaStorageSettings> options,
            UserManager<User> userManager,
            IImageService imageService,
            IStorageService storageService,
            IDateTimeProvider dateTimeProvider)
        {
            _storageSettings = options.Value;
            _userManager = userManager;
            _imageService = imageService;
            _storageService = storageService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IResult<string>> Handle(UpdateUserPictureRequest request, CancellationToken cancellationToken)
        {
            var container = Path.Combine(_storageSettings.Image.Container, "users");
            var blobName = BlobHelpers.GetPictureBlobName(_dateTimeProvider.Now.ToDateTimeUtc());
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId + "");
                if (user == null) return Result<string>.Fail(ResultError.Unauthorized);

                if (!string.IsNullOrEmpty(user.Picture))
                {
                    await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, user.Picture, cancellationToken);
                    user.UpdatePicture(string.Empty);
                }

                var image = await _imageService.UploadImage(request.Data, container, blobName, cancellationToken);
                user.UpdatePicture(image.Path);
                await _userManager.UpdateAsync(user);
                return Result<string>.Success(image.Url);
            }
            catch (Exception)
            {
                await _storageService.RemoveAsync(_storageSettings.Audio.Bucket, container, blobName,
                    cancellationToken);
                throw;
            }
        }
    }
}