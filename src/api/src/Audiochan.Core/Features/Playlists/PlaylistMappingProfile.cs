using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Playlists.GetPlaylist;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Playlists
{
    public static class PlaylistMaps
    {
        public static readonly Expression<Func<Playlist, PlaylistDto>> PlaylistToDetailFunc = playlist =>
            new PlaylistDto
            {
                Id = playlist.Id,
                Title = playlist.Title,
                Description = playlist.Description ?? string.Empty,
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