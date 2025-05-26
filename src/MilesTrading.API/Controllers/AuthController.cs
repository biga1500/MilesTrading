using Microsoft.AspNetCore.Mvc;
using MilesTrading.Business.Interfaces;
using MilesTrading.Models.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;

namespace MilesTrading.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register([FromBody] RegisterModel model)
        {
            if (await _userService.GetByEmailAsync(model.Email) != null)
            {
                return BadRequest("Email já está em uso");
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                FinancialBalance = 0,
                TrustLevel = 1
            };

            try
            {
                var createdUser = await _userService.CreateAsync(user, model.Password);
                return CreatedAtAction(nameof(Login), new { email = user.Email }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel model)
        {
            var user = await _userService.GetByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Email ou senha inválidos");
            }

            var isValid = await _userService.ValidateCredentialsAsync(model.Email, model.Password);
            if (!isValid)
            {
                return Unauthorized("Email ou senha inválidos");
            }

            var token = await _jwtService.GenerateTokenAsync(user);
            return Ok(new { token });
        }
    }

    public class RegisterModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
