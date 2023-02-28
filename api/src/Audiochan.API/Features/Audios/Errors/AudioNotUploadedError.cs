using Audiochan.Common.Models;

namespace Audiochan.API.Features.Audios.Errors;

public record AudioNotUploadedError(string UploadId) 
    : UserError("Audio has not been uploaded.", nameof(AudioNotUploadedError));