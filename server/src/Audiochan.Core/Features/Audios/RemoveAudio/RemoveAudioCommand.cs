using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.RemoveAudio
{
    public record RemoveAudioCommand(long Id) : IRequest<IResult<bool>>
    {
    }

    public class RemoveAudioCommandHandler : IRequestHandler<RemoveAudioCommand, IResult<bool>>
    {
        private readonly AudiochanOptions.StorageOptions _audioStorageOptions;
        private readonly IApplicationDbContext _dbContext;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;

        public RemoveAudioCommandHandler(IOptions<AudiochanOptions> options,
            IApplicationDbContext dbContext,
            IStorageService storageService,
            ICurrentUserService currentUserService)
        {
            _audioStorageOptions = options.Value.AudioStorageOptions;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<bool>> Handle(RemoveAudioCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

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
                _storageService.RemoveAsync(_audioStorageOptions.Container, BlobHelpers.GetAudioBlobName(audio),
                    cancellationToken)
            };
            if (!string.IsNullOrEmpty(audio.Picture))
                tasks.Add(_storageService.RemoveAsync(audio.Picture, cancellationToken));
            await Task.WhenAll(tasks);
            return Result<bool>.Success(true);
        }
    }
}