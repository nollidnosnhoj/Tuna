using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.UpdateAudio
{
    public record UpdateAudioCommand : AudioCommandRequest, IRequest<Result<AudioDetailViewModel>>
    {
        [JsonIgnore] public long Id { get; init; }
    }

    public class UpdateAudioCommandValidator : AbstractValidator<UpdateAudioCommand>
    {
        public UpdateAudioCommandValidator()
        {
            Include(new AudioCommandValidator());
        }
    }

    public class UpdateAudioCommandHandler : IRequestHandler<UpdateAudioCommand, Result<AudioDetailViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IGenreRepository _genreRepository;
        private readonly ITagRepository _tagRepository;

        public UpdateAudioCommandHandler(IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IGenreRepository genreRepository,
            ITagRepository tagRepository)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _genreRepository = genreRepository;
            _tagRepository = tagRepository;
        }

        public async Task<Result<AudioDetailViewModel>> Handle(UpdateAudioCommand request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            var audio = await _dbContext.Audios
                .Include(a => a.Favorited)
                .Include(a => a.User)
                .Include(a => a.Tags)
                .Include(a => a.Genre)
                .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (audio == null)
                return Result<AudioDetailViewModel>.Fail(ResultError.NotFound);

            if (!audio.CanModify(currentUserId))
                return Result<AudioDetailViewModel>.Fail(ResultError.Forbidden);

            if (!string.IsNullOrWhiteSpace(request.Genre) && (audio.Genre?.Slug ?? "") != request.Genre)
            {
                var genre = await _genreRepository.GetByInput(request.Genre, cancellationToken);

                if (genre == null)
                    return Result<AudioDetailViewModel>.Fail(ResultError.BadRequest, "Genre does not exist.");

                audio.UpdateGenre(genre);
            }

            if (request.Tags.Count > 0)
            {
                var newTags = await _tagRepository.CreateTags(request.Tags, cancellationToken);

                audio.UpdateTags(newTags);
            }

            audio.UpdateTitle(request.Title);
            audio.UpdateDescription(request.Description);
            audio.UpdatePublicStatus(request.IsPublic);

            _dbContext.Audios.Update(audio);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var viewModel = _mapper.Map<AudioDetailViewModel>(audio) with
            {
                IsFavorited = audio.Favorited.Any(x => x.UserId == currentUserId)
            };

            return Result<AudioDetailViewModel>.Success(viewModel);
        }
    }
}