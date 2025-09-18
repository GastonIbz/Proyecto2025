using Proyecto2024.Shared.DTO;

namespace Proyecto2024.Client.Autorizacion
{
    public interface ILoginService
    {
        // El Login recibe un UserTokenDTO y devuelve un UserTokenDTO
        Task Login(UserTokenDTO tokenDTO);

        // El Logout no recibe nada y no devuelve nada
        Task Logout();
    }
}