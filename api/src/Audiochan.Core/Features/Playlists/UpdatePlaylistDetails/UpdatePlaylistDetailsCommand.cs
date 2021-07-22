using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Playlists.UpdatePlaylistDetails
{
    public record UpdatePlaylistDetailsCommand : IRequest<Result<PlaylistDetailViewModel>>
    {
        public Guid Id { get; init; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Visibility? Visibility { get; set; }

        public UpdatePlaylistDetailsCommand(Guid id, UpdatePlaylistDetailsRequest request)
        {
            Id = id;
            Title = request.Title;
            Description = request.Description;
            Visibility = request.Visibility;
        }
    }
    
    public class UpdatePlaylistDetailsCommandHandler 
        : IRequestHandler<UpdatePlaylistDetailsCommand,Result<PlaylistDetailViewModel>>
    {
        private readonly string _currentUserId;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePlaylistDetailsCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PlaylistDetailViewModel>> Handle(UpdatePlaylistDetailsCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .LoadForUpdating(request.Id, cancellationToken);

            if (playlist is null)
                return Result<PlaylistDetailViewModel>.NotFound<Playlist>();

            if (playlist.UserId != _currentUserId)
                return Result<PlaylistDetailViewModel>.Forbidden();

            if (!string.IsNullOrWhiteSpace(request.Title))
                playlist.Title = request.Title;

            if (request.Description is not null)
                playlist.Description = request.Description;

            if (request.Visibility is not null)
                playlist.Visibility = request.Visibility.Value;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<PlaylistDetailViewModel>.Success(PlaylistMaps.PlaylistToDetailFunc.Compile().Invoke(playlist));
        }
    }
}