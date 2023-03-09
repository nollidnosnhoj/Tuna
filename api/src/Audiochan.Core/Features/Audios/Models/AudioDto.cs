using System;
using System.Collections.Generic;
using Audiochan.Core.Features.Users.Models;
using HotChocolate;

namespace Audiochan.Core.Features.Audios.Models;

[GraphQLName("Audio")]
public class AudioDto
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public long UserId { get; set; }
    public decimal Duration { get; set; }
    public long Size { get; set; }
    public string? Picture { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string ObjectKey { get; set; } = null!;
}