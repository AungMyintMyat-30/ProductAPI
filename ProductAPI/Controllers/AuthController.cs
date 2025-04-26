using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductCore.Helper;
using ProductCore.Interfaces;
using ProductCore.Models;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ITokenBuilder tokenBuilder,
                                IConfiguration configuration,
                                ILogger<AuthController> logger) : ControllerBase
    {
        private readonly ITokenBuilder _tokenBuilder = tokenBuilder;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<AuthController> _logger = logger;

        /// <summary>
        /// Authenticates a user and generates an access token upon successful login.
        /// </summary>
        /// <param name="request">The login credentials containing the username and password.</param>
        /// <returns>
        /// <response code="200">Returns a success response with the generated access token.</response>
        /// <response code="401">Returns an unauthorized response indicating invalid credentials.</response>
        /// </returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginModel request)
        {
            _logger.LogInformation($"Login attempt for user: {request.Username}");
            try
            {
                if (request.Username == "admin" && request.Password == "pswadmin")
                {
                    string token = _tokenBuilder.GenerateAccessToken(request.Username);
                    _logger.LogInformation($"User {request.Username} logged in successfully. JWT generated.");
                    return ResponseHelper.OK_Result(new { Token = token }, "Login successful");
                }
                else
                {
                    _logger.LogWarning($"Login failed for user: {request.Username}. Invalid credentials.");
                    return ResponseHelper.Unauthorized_Request(null, "Invalid credentials");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for user: {request.Username}");
                return ResponseHelper.InternalServerError_Request(null, "Internal server error");
            }
        }
    }
}
