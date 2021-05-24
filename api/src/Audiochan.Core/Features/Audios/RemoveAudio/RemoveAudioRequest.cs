using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioRequest(Guid Id) : IRequest<IResult<bool>>;

    public class RemoveAudioRequestHandler : IRequestHandler<RemoveAudioRequest, IResult<bool>>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IApplicationDbContext _dbContext;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;

        public RemoveAudioRequestHandler(IOptions<MediaStorageSettings> options,
            IApplicationDbContext dbContext,
            IStorageService storageService,
            ICurrentUserService currentUserService)
        {
            _storageSettings = options.Value;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<bool>> Handle(RemoveAudioRequest request, CancellationToken cancellationToken)
        {
            var (audio, result) = await GetAudio(request.Id, cancellationToken);

            if (audio == null)
                return result;

            _dbContext.Audios.Remove(audio);

            await _dbContext.SaveChangesAsync(cancellationToken);

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
            return result;
        }

        private async Task<(Audio?, IResult<bool>)> GetAudio(Guid audioId, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _dbContext.Audios
                .SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);

            if (audio == null)
                return (null, Result<bool>.Fail(ResultError.NotFound));

            return !audio.CanModify(currentUserId) 
                ? (null, Result<bool>.Fail(ResultError.Forbidden)) 
                : (audio, Result<bool>.Success(true));
        }
    }
}