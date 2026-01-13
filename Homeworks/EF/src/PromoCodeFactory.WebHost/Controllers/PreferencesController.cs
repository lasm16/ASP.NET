using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Контроллер для работы с предпочтениями клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController(IRepository<Preference> preferenceRepository) : ControllerBase
    {

        /// <summary>
        /// Получить список всех предпочтений
        /// </summary>
        /// <returns>Список предпочтений в виде PreferenceResponse</returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            var preferences = await preferenceRepository.GetAllAsync();

            var response = preferences.Select(p => new PreferenceResponse
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            return Ok(response);
        }
    }
}
