using Backend_dotnet.DTOs;
using Backend_dotnet.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return StatusCode(201, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);

                if (token == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                return Ok(new { token });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Request password reset email
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                await _authService.SendResetTokenAsync(dto.Email);
                return Ok(new { message = "Password reset link has been sent to your email" });
            }
            catch (KeyNotFoundException)
            {
                // Don't reveal if email exists or not for security
                return Ok(new { message = "If the email exists, a password reset link will be sent" });
            }
        }

        /// <summary>
        /// Reset password using token
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _authService.ResetPasswordAsync(dto);
                return Ok(new { message = "Password has been reset successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Redirect to Google OAuth (placeholder - full OAuth requires additional setup)
        /// </summary>
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            // For now, redirect to a message page. Full Google OAuth requires:
            // 1. Google Cloud Console project with OAuth credentials
            // 2. ASP.NET Core Google Authentication configured
            // 3. Callback handler to exchange code for token
            
            _logger.LogInformation("Google OAuth login requested");
            
            // Return info about OAuth status
            return Ok(new { 
                message = "Google OAuth is not yet configured in C#. Please use email/password login.",
                status = "pending_setup"
            });
        }
    }
}
