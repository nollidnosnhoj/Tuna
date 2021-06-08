using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Shared.Requests;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioCommand : AudioAbstractRequest, IRequest<Result<AudioDetailViewModel>>
    {
        public Guid AudioId { get; set; }
    }


    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioDetailViewModel>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagRepository _tagRepository;

        public UpdateAudioCommandHandler(ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork, ITagRepository tagRepository)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(UpdateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var audio = await _unitOfWork.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == command.AudioId, cancellationToken);

            if (audio == null)
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (command.Tags.Count > 0)
            {
                var newTags = await _tagRepository.GetAppropriateTags(command.Tags, cancellationToken);

                audio.UpdateTags(newTags);
            }

            audio.UpdateTitle(command.Title);
            audio.UpdateDescription(command.Description);

            if (command.IsPublic.HasValue)
                audio.UpdatePublicity(command.IsPublic.GetValueOrDefault());

            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AudioDetailViewModel>.Success(audio.MapToDetail());
        }
    }
}