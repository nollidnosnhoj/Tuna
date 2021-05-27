using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioRequest(Guid Id) : IRequest<IResult<bool>>;

    public class RemoveAudioRequestHandler : IRequestHandler<RemoveAudioRequest, IResult<bool>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAudioRequestHandler(IOptions<MediaStorageSettings> options,
            IStorageService storageService,
            ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork)
        {
            _storageSettings = options.Value;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<bool>> Handle(RemoveAudioRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
        
            var audio = await _unitOfWork.Audios.GetByIdAsync(request.Id, cancellationToken);
            
            if (audio == null)
                return Result<bool>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<bool>.Fail(ResultError.Forbidden);

            _unitOfWork.Audios.Remove(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var tasks = new List<Task>
            {
                _storageService.RemoveAsync(
                    _storageSettings.Audio.Bucket,
                    _storageSettings.Audio.Container,
                    $"{audio.Id}/{audio.FileName}",
                    cancellationToken)
            };
            if (!string.IsNullOrEmpty(audio.Picture))
            {
                tasks.Add(_storageService.RemoveAsync(_storageSettings.Image.Bucket,
                    string.Join('/', _storageSettings.Image.Container, "audios"),
                    audio.Picture,
                    cancellationToken));
            }

            await Task.WhenAll(tasks);
            return Result<bool>.Success(true);
        }
    }
}