using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController(
        IRepository<Employee> employeeRepository,
        IRepository<Role> roleRepository) : ControllerBase
    {

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по id
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse
            {
                Id = employee.Id,
                Email = employee.Email,
                Role = new RoleItemResponse
                {
                    Id = employee.Role?.Id ?? Guid.Empty,
                    Name = employee.Role?.Name,
                    Description = employee.Role?.Description
                },
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync(CreateOrEditEmployeeRequest request)
        {
            var role = await roleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
                return BadRequest("Role not found");

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = role,
                AppliedPromocodesCount = request.AppliedPromocodesCount
            };

            await employeeRepository.AddAsync(employee);

            return Ok("Created");
        }

        /// <summary>
        /// Обновить данные сотрудника
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditEmployeeAsync(Guid id, CreateOrEditEmployeeRequest request)
        {
            var employee = await employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            var role = await roleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
                return BadRequest("Role not found");

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Role = role;
            employee.AppliedPromocodesCount = request.AppliedPromocodesCount;

            await employeeRepository.UpdateAsync(employee);

            return Ok("Updated");
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            var employee = await employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            await employeeRepository.DeleteAsync(employee);

            return NoContent();
        }
    }
}