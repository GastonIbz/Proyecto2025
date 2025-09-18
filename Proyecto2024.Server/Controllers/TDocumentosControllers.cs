using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Proyecto2024.BD.Data;
using Proyecto2024.BD.Data.Entity;
using Proyecto2024.Server.Repositorio;

namespace Proyecto2024.Server.Controllers
{
    [ApiController]
    [Route("api/TDocumentos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Esto sirve para proteger con Autenticación JWT al controlador
    public class TDocumentosControllers : ControllerBase
    {
        private readonly ITDocumentoRepositorio repositorio;
        //6 - Crea y Asigna el campo de OutputCacheStore
        private readonly IOutputCacheStore outputCacheStore;

        //7 - Creación de constante "cacheKey"
        private const string cacheKey = "TDocumentos";

        //5 - Inyección IOutputCache Store, outputCacheStore en el constructor
        public TDocumentosControllers(ITDocumentoRepositorio repositorio, IOutputCacheStore outputCacheStore )
        {
            this.repositorio = repositorio;
            this.outputCacheStore = outputCacheStore;
            this.outputCacheStore = outputCacheStore;
        }

        [HttpGet]    //api/TDocumentos
        //3 - Agrego OutputCache al GET - En todos los GET  [OutputCache(Tags = [cacheKey])]
        // 8 - Agrego Tags = "cacheKey" - Clave para Tipo de documentos
        [OutputCache(Tags = [cacheKey])]
        [AllowAnonymous]
        public async Task<ActionResult<List<TDocumento>>> Get()
        {
            return await repositorio.Select();
        }

        /// <summary>
        /// Endpoint para obtener un objeto de tipo de documento 
        /// </summary>
        /// <param name="id">Id del objeto</param>
        /// <returns></returns>
        ///

        [HttpGet("{id:int}")] //api/TDocumentos/2
        [OutputCache(Tags = [cacheKey])]
        public async Task<ActionResult<TDocumento>> Get(int id)
        {
            TDocumento? pepe = await repositorio.SelectById(id);
            if (pepe == null)
            {
                return NotFound();
            }
            return pepe;
        }

        [HttpGet("GetByCod/{cod}")] //api/TDocumentos/GetByCod/DNI
        [OutputCache(Tags = [cacheKey])]  // En todos los GET  [OutputCache(Tags = [cacheKey])]
        public async Task<ActionResult<TDocumento>> GetByCod(string cod)
        {
            TDocumento? pepe = await repositorio.SelectByCod(cod);
            if (pepe == null)
            {
                return NotFound();
            }
            return pepe;
        }

        [HttpGet("existe/{id:int}")] //api/TDocumentos/existe/2
        [OutputCache(Tags = [cacheKey])] // En todos los GET  [OutputCache(Tags = [cacheKey])]
        public async Task<ActionResult<bool>> Existe(int id)
        {
            return await repositorio.Existe(id);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(TDocumento entidad)
        {
            try
            {
                //4 - Sintaxis del Post con ópcion de respuesta erronea
                var id = await repositorio.Insert(entidad);
                if (id == 0)
                {
                    return BadRequest("No se pudo insertar el tipo de documento");
                }
                await outputCacheStore.EvictByTagAsync(cacheKey, default); // En Post - Put - Delete
                return id;
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        [HttpPut("{id:int}")] //api/TDocumentos/2
        public async Task<ActionResult> Put(int id, [FromBody] TDocumento entidad)
        {
            try
            {
                if (id != entidad.Id)
                {
                    return BadRequest("Datos Incorrectos");
                }
                var pepe = await repositorio.Update(id, entidad);

                if (!pepe)
                {
                    return BadRequest("No se pudo actualizar el tipo de documento");
                }
                // Se agrego el outputCacheStore.EvictByTagAsync para el Put
                await outputCacheStore.EvictByTagAsync(cacheKey, default); // En Post - Put - Delete
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id:int}")] //api/TDocumentos/2
        public async Task<ActionResult> Delete(int id)
        { 
            var resp = await repositorio.Delete(id);
            if (!resp)
            { return BadRequest("El tipo de documento no se pudo borrar"); }
            await outputCacheStore.EvictByTagAsync(cacheKey, default); // En Post - Put - Delete
            return Ok();
        }

    }
}
