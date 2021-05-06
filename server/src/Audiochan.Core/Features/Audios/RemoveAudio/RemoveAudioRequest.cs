using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioRequest(long Id) : IRequest<IResult<bool>>
    {
    }
    
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
            var currentUserId = await _dbContext.Users
                .Select(u => u.Id)
                .SingleOrDefaultAsync(id => id == _currentUserService.GetUserId(), cancellationToken);

            if (string.IsNullOrEmpty(currentUserId))
                return Result<bool>.Fail(ResultError.Unauthorized);

            var audio = await _dbContext.Audios
                .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (audio == null)
                return Result<bool>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<bool>.Fail(ResultError.Forbidden);

            _dbContext.Audios.Remove(audio);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var tasks = new List<Task>
            {
                _storageService.RemoveAsync(
                    _storageSettings.Audio.Bucket,
                    _storageSettings.Audio.Container, 
                    BlobHelpers.GetAudioBlobName(audio),
                    cancellationToken)
            };
            if (!string.IsNullOrEmpty(audio.Picture))
            {
                tasks.Add(_storageService.RemoveAsync(_storageSettings.Audio.Bucket, audio.Picture, cancellationToken));
            }
                
            await Task.WhenAll(tasks);
            return Result<bool>.Success(true);
        }
    }

}