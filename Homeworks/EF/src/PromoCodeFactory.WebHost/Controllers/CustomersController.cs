using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController(
        IRepository<Customer> customerRepository,
        ICustomerPreferenceRepository customerPreferenceRepository,
        IRepository<PromoCode> promoCodeRepository) : ControllerBase
    {
        /// <summary>
        /// Получение списка клиентов
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await customerRepository.GetAllAsync();

            var response = customers.Select(c => new CustomerShortResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получение одного клиента с промокодами и предпочтениями
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            var customerPrefs = (await customerPreferenceRepository.GetAllAsync())
                .Where(cp => cp.CustomerId == id)
                .ToList();

            var response = new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = [.. customerPrefs
                    .Select(cp => new PreferenceResponse
                    {
                        Id = cp.PreferenceId,
                        Name = cp.Preference.Name
                    })],
                PromoCodes = [.. customer.PromoCodes
                    .Select(p => new PromoCodeShortResponse
                    {
                        Id = p.Id,
                        Code = p.Code,
                        ServiceInfo = p.ServiceInfo,
                        BeginDate = p.BeginDate.ToString("yyyy-MM-dd"),
                        EndDate = p.EndDate.ToString("yyyy-MM-dd"),
                        PartnerName = p.PartnerName
                    })]
            };

            return Ok(response);
        }

        /// <summary>
        /// Создание нового клиента вместе с предпочтениями
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            };

            await customerRepository.AddAsync(customer);

            if (request.PreferenceIds.Count >0)
            {
                foreach (var prefId in request.PreferenceIds)
                {
                    await customerPreferenceRepository.AddAsync(new CustomerPreference
                    {
                        CustomerId = customer.Id,
                        PreferenceId = prefId
                    });
                }
            }

            return Ok("Created");
        }

        /// <summary>
        /// Редактирование данных клиента вместе с предпочтениями
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomerAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;

            await customerRepository.UpdateAsync(customer);

            var existingPrefs = (await customerPreferenceRepository.GetAllAsync())
                .Where(cp => cp.CustomerId == id)
                .ToList();

            foreach (var cp in existingPrefs)
                await customerPreferenceRepository.DeleteAsync(cp);

            if (request.PreferenceIds.Count > 0)
            {
                foreach (var prefId in request.PreferenceIds)
                {
                    await customerPreferenceRepository.AddAsync(new CustomerPreference
                    {
                        CustomerId = id,
                        PreferenceId = prefId
                    });
                }
            }

            return Ok("Updated");
        }

        /// <summary>
        /// Удаление клиента вместе с промокодами и предпочтениями
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            await customerRepository.DeleteAsync(customer);

            return NoContent();
        }
    }
}