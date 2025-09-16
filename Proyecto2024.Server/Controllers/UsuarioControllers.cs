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
        // Servicios de ASP.NET
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        // Constructor que recibe los servicios y los asigna
        public UsuarioControllers(UserManager<IdentityUser> userManager,
                                  SignInManager<IdentityUser> signInManager,
                                  IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }


        // Metodos de la API
        [HttpPost("registrar")]
        public async Task<ActionResult<UserTokenDTO>> RegistrarUsuario([FromBody] UserInfoDTO userInfoDTO)
        {
            // Crea un nuevo objeto de usuario de Identity. Se usa el email como nombre de usuario.

            var usuario = new IdentityUser { UserName = userInfoDTO.Email, Email = userInfoDTO.Email };

            // Intenta crear el usuario en la base de datos con la contraseña proporcionada
            // Identity se encarga de (encriptar) la contraseña automáticamente
            var res = await userManager.CreateAsync(usuario, userInfoDTO.Password);

            if (res.Succeeded)
            {
                // Llama al método para generar un token JWT y lo devuelve
                return await ConstruirToken(userInfoDTO);
            }
            // Si hubo algún error (ej: contraseña débi email ya existente), devuelve el primer error
            else
            {
                // Devuelve el primer error 
                return BadRequest(res.Errors.First());
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] UserInfoDTO userInfoDTO)
        {
            // Intenta validar las credenciales del usuario (email y password).
            var res = await signInManager.PasswordSignInAsync(userInfoDTO.Email,
                                                              userInfoDTO.Password,
                                                              isPersistent: false, // false: la sesión no se recuerda si se cierra el navegador
                                                              lockoutOnFailure: false); // false: no se bloquea la cuenta tras varios intentos fallidos

            if (res.Succeeded)
            {
                // genera un token JWT y lo devuelve como respuesta exitosa.
                return await ConstruirToken(userInfoDTO);
            }
            // Si las credenciales son incorrecta, devuelve un error con un mensaje
            else
            {
                return BadRequest("Login incorrecto");
            }

        }

        // Este método no es un endpoint, es una función de ayuda para no repetir código
        private async Task<UserTokenDTO> ConstruirToken(UserInfoDTO userInfoDTO)
        {
            // Los "claims" son  datos sobre el usuario que se guardan dentro del token
            var claims = new List<Claim>()
            {
                // Claim estándar para el email del usuario.
                new Claim(ClaimTypes.Email, userInfoDTO.Email),
                 // Se pueden agregar claims personalizados con cualquier información relevante
                new Claim("otro", "cualquier cosa")
            };

            // Obtiene la clave secreta desde el archivo de configuración appsettings.json
            // Esta clave es fundamental para el token y verificar que no ha sido modificado
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtkey"]!));
            // Crea las credenciales de firma usando el algoritmo de seguridad HmacSha256
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // Establece la fecha de expiración del token (en este caso 1 mes).

            var expiracion = DateTime.UtcNow.AddMonths(1);

            // Crea el objeto del token con toda la configuración: claims, expiración y credenciales
            var token = new JwtSecurityToken
                            (
                                issuer: null,
                                audience: null,
                                claims: claims,
                                expires: expiracion,
                                signingCredentials: credenciales
                            );

            // Devuelve un objeto DTO que contiene el token en formato string y su fecha de expiración.
            return new UserTokenDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion
            };

        }
    }
}