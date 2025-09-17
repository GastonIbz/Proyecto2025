
namespace Proyecto2024.Client.Servicios
{
    public interface IHttpServicio
    {
        Task<HttpRespuesta<T>> Get<T>(string url);
        Task<HttpRespuesta<TResp>> Post<T, TResp>(string url, T entidad);
        Task<HttpRespuesta<object>> Put<T>(string url, T entidad);
        Task<HttpRespuesta<object>> Delete(string url);
    }
}