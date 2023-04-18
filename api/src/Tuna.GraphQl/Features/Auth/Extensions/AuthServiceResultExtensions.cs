using System;
using System.Collections.Generic;
using Tuna.Application.Features.Auth.Results;
using Microsoft.AspNetCore.Mvc;

namespace Tuna.GraphQl.Features.Auth.Extensions;

public static class AuthServiceResultExtensions
{
    public static IActionResult ToActionResult(
        this AuthServiceResult result,
        Func<IActionResult> onSuccess,
        Func<List<AuthServiceError>, IActionResult> onError)
    {
        return result.Succeeded ? onSuccess() : onError(result.Errors);
    }
    
    public static IActionResult ToActionResult<T>(
        this AuthServiceResult<T> result,
        Func<T, IActionResult> onSuccess,
        Func<List<AuthServiceError>, IActionResult> onError)
    {
        return result.Succeeded ? onSuccess(result.Result) : onError(result.Errors);
    }
}