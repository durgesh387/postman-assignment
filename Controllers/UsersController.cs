using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using postman_assignment.Exceptions;
using PostmanAssignment.Entities;
using PostmanAssignment.Exceptions;
using PostmanAssignment.Models;
using PostmanAssignment.Services;
using PostmanAssignment.Utilities;

namespace PostmanAssignment.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly ApplicationSettings _appSettings;

        public UsersController(ILogger<UsersController> logger, IUserService userService, IOptions<ApplicationSettings> appSettings)
        {
            _userService = userService;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        [HttpPost("login")]
        public async Task<ActionResult> AuthenticateUserAsync([FromBody]AuthenticationModel model)
        {
            try
            {
                var user = await _userService.AuthenticateUserAsync(model.Email, model.Password);
                var token = GenerateToke(user.Id);
                Response.Headers.Add("Token", token);
                return Ok(user);
            }
            catch (InvalidArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUserAsync([FromBody]User user)
        {
            try
            {
                var createdUserId = await _userService.RegisterUserAsync(user);
                var token = GenerateToke(createdUserId);
                Response.Headers.Add("Token", token);
                return Ok(new { message = $"user created with Id : {createdUserId}" });
            }
            catch (InvalidArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ConflictingEntityException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        private string GenerateToke(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}