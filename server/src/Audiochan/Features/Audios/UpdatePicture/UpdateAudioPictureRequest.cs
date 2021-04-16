﻿using Audiochan.Core.Models.Interfaces;
using MediatR;
using Newtonsoft.Json;

namespace Audiochan.Features.Audios.UpdatePicture
{
    public class UpdateAudioPictureRequest : IRequest<IResult<string>>
    {
        [JsonIgnore] public long AudioId { get; set; }
        public string Data { get; init; }
    }
}