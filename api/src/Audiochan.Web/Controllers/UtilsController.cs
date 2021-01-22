using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.Web.Controllers
{
    [Route("[controller]")]
    public class UtilsController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UtilsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpHead("username", Name="CheckIfUsernameExists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Check if username exists",
            OperationId = "CheckIfUsernameExists",
            Tags = new []{"utils"}
        )]
        public async Task<IActionResult> DoesUsernameExist([FromQuery] string username, CancellationToken cancellationToken)
        {
            var exist = await _userService.CheckIfUsernameExists(username, cancellationToken);
            if (exist) return Ok();
            return NotFound();
        }
        
        [HttpHead("email", Name="CheckIfEmailExists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Check if email exists",
            OperationId = "CheckIfEmailExists",
            Tags = new []{"utils"}
        )]
        public async Task<IActionResult> DoesEmailExist([FromQuery] string email, CancellationToken cancellationToken)
        {
            var exist = await _userService.CheckIfEmailExists(email, cancellationToken);
            if (exist) return Ok();
            return NotFound();
        }
    }
}