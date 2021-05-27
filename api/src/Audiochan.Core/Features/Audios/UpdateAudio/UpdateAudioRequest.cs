using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioRequest : AudioAbstractRequest, IRequest<IResult<AudioDetailViewModel>>
    {
        [JsonIgnore] public Guid AudioId { get; set; }
    }


    public class UpdateAudioRequestHandler : IRequestHandler<UpdateAudioRequest, IResult<AudioDetailViewModel>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAudioRequestHandler(ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<AudioDetailViewModel>> Handle(UpdateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var audio = await _unitOfWork.Audios.GetBySpecAsync(new GetAudioForUpdateSpecification(request.AudioId),
                    cancellationToken: cancellationToken);

            if (audio == null)
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (request.Tags.Count > 0)
            {
                var newTags = await _unitOfWork.Tags.GetAppropriateTags(request.Tags, cancellationToken);

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