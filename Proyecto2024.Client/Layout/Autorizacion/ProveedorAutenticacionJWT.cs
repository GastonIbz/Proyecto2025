using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Proyecto2024.Client.Servicios;
using Proyecto2024.Shared.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;


// USUARIO LOCAL
namespace Proyecto2024.Client.Autorizacion
{
    public class ProveedorAutenticacionJWT : AuthenticationStateProvider, ILoginService
    {

        // Esto es para definir las claves del localStorage
        public static readonly string TOKENKEY = "TOKENKEY";
        public static readonly string EXPIRATIONTOKENKEY = "EXPIRATIONTOKENKEY";
        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;



        // USUARIO LOCAL - AuthenticationState es para devolver el estado de autenticación
        // El constructor de esta clase necesita un argumento 
        // Anonimo es el estado de autenticacion 
        private AuthenticationState Anonimo =>
                                    new AuthenticationState(
                                        // Esta lista de claims tiene la identidad. Pero como esta vacio esto es un usuario no autenticado 
                                        new ClaimsPrincipal(new ClaimsIdentity()));



        // ProveedorAUtenticacionJWT necesita IJSRuntime para acceder al localStorage y es para poder inyectarlo
        public ProveedorAutenticacionJWT(IJSRuntime js, HttpClient httpClient)
        {
            this.js = js;
            this.httpClient = httpClient;
        }


        // Cuando traigamos de la WEBAPI Ejecutando el metodo, vamos a obtener el Local Storage, Si el token es nulo devuelve anonimo del ProveedorAutenticacionJWT
        // Y si no es nulo tiene que construir el AuthenticationState con el token
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Este var token es para obtener el token del localStorage
            var token = await js.ObtenerDeLocalStorage(TOKENKEY);

            if (token is null)
            {
                return Anonimo;
            }

            return ConstruirAuthenticationState(token.ToString()!);
        }

        // Esto es para construir el AuthenticationState con el token
        private AuthenticationState ConstruirAuthenticationState(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", token);
            // Parsear sirve para convertir el token en una lista de claims
            var claims = ParsearClaimsDelJWT(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        // Este metodo es para convertir el token en una lista de claims
        private IEnumerable<Claim> ParsearClaimsDelJWT(string token)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenDeserializado = jwtSecurityTokenHandler.ReadJwtToken(token);
            return tokenDeserializado.Claims;
        }

        // Este metodo es para guardar el token en el localStorage y notificar a la aplicacion que el estado de autenticacion ha cambiado
        public async Task Login(UserTokenDTO tokenDTO)
        {
            await js.GuardarEnLocalStorage(TOKENKEY, tokenDTO.Token);
            await js.GuardarEnLocalStorage(EXPIRATIONTOKENKEY, tokenDTO.Expiracion.ToString());
            var authSatte = ConstruirAuthenticationState(tokenDTO.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(authSatte));
        }


        // Este metodo es para eliminar el token del localStorage y notificar a la aplicacion que el estado de autenticacion ha cambiado
        public async Task Logout()
        {
            await js.RemoverDelLocalStorage(TOKENKEY);
            await js.RemoverDelLocalStorage(EXPIRATIONTOKENKEY);
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }
    }
}