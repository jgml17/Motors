using ApiMotorsAdService.Data;
using ApiMotorsAdService.Resources;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorsAdModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMotorsAdService.Controllers
{
    /// <summary>
    /// Controller de Anúncios (Ads)
    /// </summary>
    //[Authorize("Bearer", Roles = "Admin")] // Autorizacao caso tivesse OAuth2
    [AllowAnonymous] // Pode acessar sem estar autorizado
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        #region Fields

        private readonly ServiceDbContext _context;
        private readonly TelemetryClient _telemetry;
        private readonly Dictionary<string, string> _tProperties;
        private readonly Dictionary<string, double> _tMetrics;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="telemetry">Telemetria ApplicationInsights</param>
        /// <param name="tProperties">Propriedades da telemetria</param>
        /// <param name="tMetrics">Metricas da telemetria</param>
        public AdsController(ServiceDbContext context, TelemetryClient telemetry, Dictionary<string, string> tProperties, Dictionary<string, double> tMetrics)
        {
            _context = context;
            _telemetry = telemetry;
            _tMetrics = tMetrics;
            _tProperties = tProperties;

            // Telemetry Inicializacao
            _tProperties.Clear();
            _tMetrics.Clear();
            _tProperties["MicroService"] = ApiResources.AppName;
            _tProperties["Version"] = ApiResources.ApiVersion;
            _tProperties["Controller"] = "Ads";
        }

        #endregion Constructors

        #region Methods

        private bool AdExists(int id)
        {
            return _context.tb_AnuncioWebmotors.Any(e => e.ID == id);
        }

        /// <summary>
        /// Remover Anuncio por id: api/Ads/5
        /// </summary>
        /// <param name="id">id do anuncio</param>
        /// <returns>Anuncio (MotorsAdModel)</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ad>> DeleteAd(int id)
        {
            // Telemetry
            _tProperties["Action"] = "DeleteAd";

            try
            {
                var ad = await _context.tb_AnuncioWebmotors.FindAsync(id);
                if (ad == null)
                {
                    return NotFound();
                }

                _context.tb_AnuncioWebmotors.Remove(ad);
                await _context.SaveChangesAsync();

                return ad;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, _tProperties);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Consultar Anuncio por id: api/Ads/5
        /// </summary>
        /// <param name="id">id do anuncio</param>
        /// <returns>Anuncio (MotorsAdModel)</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Ad>> GetAd(int id)
        {
            // Telemetry
            _tProperties["Action"] = "GetAdId";

            try
            {
                var ad = await _context.tb_AnuncioWebmotors.FindAsync(id);

                if (ad == null)
                {
                    return NotFound();
                }

                return ad;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, _tProperties);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Consultar Anuncios: api/Ads
        /// </summary>
        /// <returns>Anuncios list(MotorsAdModel)</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ad>>> Gettb_AnuncioWebmotors()
        {
            // Telemetry
            _tProperties["Action"] = "Gettb_AnuncioWebmotors";

            try
            {
                return await _context.tb_AnuncioWebmotors.ToListAsync();
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, _tProperties);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Incluir Anuncio: api/Ads
        /// </summary>
        /// <param name="ad">Anuncio (MotorsAdModel)</param>
        /// <returns>Anuncio (MotorsAdModel)</returns>
        [HttpPost]
        public async Task<ActionResult<Ad>> PostAd(Ad ad)
        {
            // Telemetry
            _tProperties["Action"] = "PostAd";

            try
            {
                _context.tb_AnuncioWebmotors.Add(ad);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAd", new { id = ad.ID }, ad);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, _tProperties);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Atualizar Anuncio: api/Ads/5
        /// </summary>
        /// <param name="id">id do anuncio</param>
        /// <param name="ad">Anuncio (MotorsAdModel)</param>
        /// <returns>Anuncio (MotorsAdModel)</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAd(int id, Ad ad)
        {
            // Telemetry
            _tProperties["Action"] = "PutAd";

            if (id != ad.ID)
            {
                return BadRequest("Ids nao conferem");
            }

            _context.Entry(ad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _telemetry.TrackException(ex, _tProperties);
                    return BadRequest(ex);
                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex, _tProperties);
                return BadRequest(ex);
            }

            return NoContent();
        }

        #endregion Methods
    }
}