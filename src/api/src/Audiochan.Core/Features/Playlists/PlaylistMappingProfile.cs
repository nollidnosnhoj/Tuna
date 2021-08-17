using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;

namespace Audiochan.Core.Features.Playlists
{
    public static class PlaylistMaps
    {
        public static readonly Expression<Func<Playlist, PlaylistViewModel>> PlaylistToDetailFunc = playlist =>
            new PlaylistViewModel
            {
                Id = playlist.Id,
                Title = playlist.Title,
                Description = playlist.Description ?? string.Empty,
                Visibility = playlist.Visibility,
                Picture = playlist.Picture != null
                    ? string.Format(MediaLinkInvariants.PlaylistPictureUrl, playlist.Picture)
                    : null,
                Tags = playlist.Tags.Select(t => t.Name).ToList(),
                User = new MetaAuthorDto
                {
                    Id = playlist.User.Id,
                    Username = playlist.User.UserName,
                    Picture = playlist.User.Picture != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, playlist.User.Picture)
                        : null
                },
            };
    }
}