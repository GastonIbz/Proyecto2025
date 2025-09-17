using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Proyecto2024.Client;
using Proyecto2024.Client.Autorizacion;
using Proyecto2024.Client.Layout.Autorizacion;
using Proyecto2024.Client.Servicios;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Inyección de Servicios
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IHttpServicio, HttpServicio>();

// Registramos el proveedor de autenticacion JWT
builder.Services.AddScoped<ProveedorAutenticacionJWT>();
// al compilar el programa ahora se ejecuta el proveedor de autenticacion - Cambiamos el objeto por ProveedorAutenticacionJWT que es el que hemos creado
builder.Services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacionJWT>(proveedor =>
    proveedor.GetRequiredService<ProveedorAutenticacionJWT>());
// al compilar el programa ahora se ejecuta el proveedor de autenticacion
builder.Services.AddScoped<ILoginService, ProveedorAutenticacionJWT>(proveedor =>
    proveedor.GetRequiredService<ProveedorAutenticacionJWT>());


await builder.Build().RunAsync();
