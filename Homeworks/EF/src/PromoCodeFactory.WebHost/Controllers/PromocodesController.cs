using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController(
        IRepository<PromoCode> promoCodeRepository,
        IRepository<Customer> customerRepository,
        IRepository<Employee>employeeRepository,
        IRepository<Preference> preferenceRepository) : ControllerBase
    {

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns>Список промокодов</returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promocodes = await promoCodeRepository.GetAllAsync();

            var response = promocodes.Select(p => new PromoCodeShortResponse
            {
                Id = p.Id,
                Code = p.Code,
                ServiceInfo = p.ServiceInfo,
                PartnerName = p.PartnerName,
                BeginDate = p.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = p.EndDate.ToString("yyyy-MM-dd")
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <param name="request">Данные промокода</param>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var preference = (await preferenceRepository.GetAllAsync())
                .FirstOrDefault(p => p.Name == request.Preference);

            if (preference == null)
                return NotFound("Preference not found");

            var customers = (await customerRepository.GetAllAsync())
                .Where(c => c.CustomerPreferences.Any(cp => cp.PreferenceId == preference.Id))
                .ToList();

            if (customers.Count == 0)
                return NotFound("No customers found with this preference");

            var employee = (await employeeRepository.GetAllAsync())
                .Where(x => x.FullName.Equals(request.PartnerName))
                .FirstOrDefault();

            if (employee == null)
                return NotFound("No customers found with this preference");

            foreach (var customer in customers)
            {
                var promocode = new PromoCode
                {
                    Id = Guid.NewGuid(),
                    Code = request.PromoCode,
                    ServiceInfo = request.ServiceInfo,
                    PartnerName = request.PartnerName,
                    BeginDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    CustomerId = customer.Id,
                    PreferenceId = preference.Id,
                    PartnerManagerId = employee.Id
                };

                await promoCodeRepository.AddAsync(promocode);
            }

            return Ok();
        }
    }
}