using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public class UpdateAudioRequestHandler : IRequestHandler<UpdateAudioRequest, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITagRepository _tagRepository;
        private readonly AudiochanOptions _audiochanOptions;

        public UpdateAudioRequestHandler(IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            ITagRepository tagRepository,
            IOptions<AudiochanOptions> options)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _tagRepository = tagRepository;
            _audiochanOptions = options.Value;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(UpdateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = await _dbContext.Users
                .Select(u => u.Id)
                .SingleOrDefaultAsync(id => id == _currentUserService.GetUserId(), cancellationToken);

            if (string.IsNullOrEmpty(currentUserId))
                return Result<AudioDetailViewModel>.Fail(ResultError.Unauthorized);

            var audio = await _dbContext.Audios
                .Include(a => a.User)
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (audio == null)
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);
            
            if (request.Tags.Count > 0)
            {
                var newTags = await _tagRepository.GetListAsync(request.Tags, cancellationToken);

                audio.UpdateTags(newTags);
            }

            audio.UpdateTitle(request.Title);
            audio.UpdateDescription(request.Description);
            audio.UpdatePublicityStatus(request.Visibility);
            
            if (audio.Visibility == Visibility.Private)
                audio.SetPrivateKey();
            else
                audio.ClearPrivateKey();

            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var viewModel = audio.MapToDetail(_audiochanOptions);

            return Result<AudioDetailViewModel>.Success(viewModel);
        }
    }
}