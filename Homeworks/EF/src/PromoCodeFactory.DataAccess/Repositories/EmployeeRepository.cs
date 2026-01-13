using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Persistense;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EmployeeRepository(DataContext context) : EfRepository<Employee>(context)
    {
        public override async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(x => x.Role)
                .ToListAsync();
        }

        public override async Task<Employee> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
