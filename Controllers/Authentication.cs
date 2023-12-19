using ECommerceApi.Models;
using ECommerceApi.Models.DTO.Request;
using ECommerceApi.Models.DTO.Response.ResponseBuilders;
using ECommerceApi.Persistence;
using ECommerceApi.Services;
using ECommerceApi.Utils;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ECommerceApi.Models.DTO.Response;


namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController: ControllerBase
    {
        private readonly DataContext _context;
        public AuthenticationController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginReqDTO loginReq)
        {
            try
            {
                var validationResult = MinimalValidator.Validate(loginReq);
                if (!validationResult.IsValid)
                {
                    return StatusCode(400, ResponseBuilder.ErrorResponse(400, "Validation failed", validationResult.Errors));
                }

                loginReq.Password = PasswordHasher.Make(loginReq.Password);
                UserModel user = await _context.Users.Where(user => user.Email == loginReq.Email && user.Password == loginReq.Password).FirstOrDefaultAsync();
                if (user != null) {
                    return Ok(ResponseBuilder.SuccessResponse("Success get order", new LoginRespDTO {
                        Id = user.Id,
                        Name=user.Name,
                        Email=user.Email,
                        Token=CreateTokenUser(user)
                    }));
                }
                return StatusCode(400, ResponseBuilder.ErrorResponse(400, "Invalid username or password"));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ResponseBuilder.ErrorResponse(400, ex.Message));
            }
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterReqDTO registerReq)
        {
            try
            {
                UserModel user = new()
                {
                    Name = registerReq.Name,
                    Email = registerReq.Email,
                    Password = PasswordHasher.Make(registerReq.Password)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(ResponseBuilder.SuccessResponse("Successfully registered user", user));
            }
            catch (Exception ex)
            {
                return StatusCode(400, ResponseBuilder.ErrorResponse(400, ex.Message));
            }
        }


        private string CreateTokenUser(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
            };

            return generateKey(claims);
        }

        private string generateKey(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("InterviewTest key 2023 Dot Indonesia for user authentication purpose"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}