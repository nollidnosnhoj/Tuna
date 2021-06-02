using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.GetAudio;
using Audiochan.API.Features.Shared.Requests;
using Audiochan.API.Mappings;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.API.Features.Audios.UpdateAudio
{
    public class UpdateAudioRequest : AudioAbstractRequest, IRequest<Result<AudioDetailViewModel>>
    {
        [JsonIgnore] public Guid AudioId { get; set; }
    }


    public class UpdateAudioRequestHandler : IRequestHandler<UpdateAudioRequest, Result<AudioDetailViewModel>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagRepository _tagRepository;

        public UpdateAudioRequestHandler(ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork, ITagRepository tagRepository)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _tagRepository = tagRepository;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(UpdateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            
            var audio = await _unitOfWork.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == request.AudioId, cancellationToken);

            if (audio == null)
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (request.Tags.Count > 0)
            {
                var newTags = await _tagRepository.GetAppropriateTags(request.Tags, cancellationToken);

                audio.UpdateTags(newTags);
            }

            audio.UpdateTitle(request.Title);
            audio.UpdateDescription(request.Description);

            if (request.IsPublic.HasValue)
                audio.UpdatePublicity(request.IsPublic.GetValueOrDefault());

            _unitOfWork.Audios.Update(audio);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AudioDetailViewModel>.Success(audio.MapToDetail());
        }
    }
}