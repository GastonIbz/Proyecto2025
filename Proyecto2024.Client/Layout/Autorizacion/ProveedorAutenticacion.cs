using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Proyecto2024.Client.Layout.Autorizacion
{
    public class ProveedorAutenticacion : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.Delay(2000);
            // Clase Abstracta que nos devolvera la autenticacion
            var identidad = new ClaimsIdentity();

            // Si el usuario esta autenticado, devuelve su identidad con sus claims
            var usuarioPepe = new ClaimsIdentity(
                // Lista de claims / datos claves valor
                new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Pepe Perez"),
                    // Si en NavMenu se usa AuthorizeView, con el nombre "admin" se mostrara el contenido dependiendo del rol
                    new Claim(ClaimTypes.Role, "admin"), // "admin" o "operador"
                    new Claim(ClaimTypes.Country, "Argentina"),
                    new Claim("DNI", "42.587.895"),
                   },

                // Tipo de autenticacion ("test" esta autenticado)
                authenticationType: "test"
                
                );

            // Si el usuario no esta autenticado, devuelve una identidad desconocida
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(usuarioPepe)));
        }
    }
}
