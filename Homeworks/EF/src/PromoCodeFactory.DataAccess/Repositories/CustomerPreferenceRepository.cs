using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Persistense;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class CustomerPreferenceRepository(DataContext context) : ICustomerPreferenceRepository
    {
        private readonly DataContext _context = context;

        public async Task AddAsync(CustomerPreference entity)
        {
            _context.CustomerPreferences.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CustomerPreference entity)
        {
            _context.CustomerPreferences.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerPreference>> GetAllAsync()
        {
            return await _context.CustomerPreferences
                .Include(cp => cp.Preference)
                .ToListAsync();
        }
    }
}
