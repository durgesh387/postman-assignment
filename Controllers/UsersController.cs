using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostmanAssignment.Entities;
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
            var user = await _userService.AuthenticateUserAsync(model.Email, model.Password);
            var token = GenerateToke(user.Id);
            Response.Headers.Add("Token", token);
            return Ok(user);
            // if (user == null)
            //     return BadRequest(new { message = "Username or password is incorrect" });

            // return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUserAsync([FromBody]User user)
        {
            var createdUserId = await _userService.RegisterUserAsync(user);
            var token = GenerateToke(createdUserId);
            Response.Headers.Add("Token", token);
            return Ok(createdUserId);
            // if (user == null)
            //     return BadRequest(new { message = "Username or password is incorrect" });

            // return Ok(user);
        }

        private string GenerateToke(int userId)
        {
            // authentication successful so generate jwt token
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