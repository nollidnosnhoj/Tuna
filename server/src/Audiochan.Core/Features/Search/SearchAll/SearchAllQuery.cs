using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Search.SearchAudios;
using Audiochan.Core.Features.Search.SearchUsers;
using MediatR;

namespace Audiochan.Core.Features.Search.SearchAll
{
    public record SearchAllQuery : IRequest<SearchAllViewModel>
    {
        public string Q { get; init; } = string.Empty;
    }
    
    public class SearchAllQueryHandler : IRequestHandler<SearchAllQuery, SearchAllViewModel>
    {
        private readonly IMediator _mediator;

        public SearchAllQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<SearchAllViewModel> Handle(SearchAllQuery request, CancellationToken cancellationToken)
        {
            var audios = await _mediator.Send(new SearchAudiosQuery
            {
                Q = request.Q,
                Page = 1,
                Size = 3
            }, cancellationToken);
            
            var users = await _mediator.Send(new SearchUsersQuery
            {
                Q = request.Q,
                Page = 1,
                Size = 3
            }, cancellationToken);

            return new SearchAllViewModel
            {
                Audios = audios,
                Users = users
            };
        }
    }
}