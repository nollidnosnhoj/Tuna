using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Common.Extensions
{
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Validate the IFormFile to ensure the file meets the given restrictions.
        /// </summary>
        /// <param name="ruleBuilder"></param>
        /// <param name="contentTypes"></param>
        /// <param name="fileSizeLimit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRuleBuilder<T, IFormFile> FileValidation<T>(this IRuleBuilder<T, IFormFile> ruleBuilder,
            IEnumerable<string> contentTypes, long fileSizeLimit)
        {
            return ruleBuilder
                .Must(file => file.Length <= fileSizeLimit)
                .WithMessage($"File size is over {fileSizeLimit / 1000000} MB")
                .Must(file => contentTypes.Contains(file.ContentType))
                .WithMessage("File type is invalid.");
        }
    }
}