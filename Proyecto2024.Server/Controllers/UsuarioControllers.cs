using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto2024.Shared.DTO;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Proyecto2024.Server.Controllers
{
    [ApiController]
    [Route("usuarios")]
    public class UsuarioControllers : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public UsuarioControllers(UserManager<IdentityUser> userManager,
                                  SignInManager<IdentityUser> signInManager,
                                  IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        // Metodos UserTokenDTO y UserInfoDTO en Shared/DTO

        [HttpPost("registrar")]
        public async Task<ActionResult<UserTokenDTO>> RegistrarUsuario([FromBody] UserInfoDTO userInfoDTO)
        {
            var usuario = new IdentityUser { UserName = userInfoDTO.Email, Email = userInfoDTO.Email };
            var res = await userManager.CreateAsync(usuario, userInfoDTO.Password);

            if (res.Succeeded)
            {
                return await ConstruirToken(userInfoDTO);
            }
            else
            {
                return BadRequest(res.Errors.First());
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] UserInfoDTO userInfoDTO)
        {
            var res = await signInManager.PasswordSignInAsync(userInfoDTO.Email,
                                                              userInfoDTO.Password,
                                                              isPersistent: false,
                                                              lockoutOnFailure: false);

            if (res.Succeeded)
            {
                return await ConstruirToken(userInfoDTO);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }

        }

        private async Task<UserTokenDTO> ConstruirToken(UserInfoDTO userInfoDTO)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, userInfoDTO.Email),
                new Claim("otro", "cualquier cosa")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtkey"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddMonths(1);

            var token = new JwtSecurityToken
                            (
                                issuer: null,
                                audience: null,
                                claims: claims,
                                expires: expiracion,
                                signingCredentials: credenciales
                            );

            return new UserTokenDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion
            };

        }
    }
}