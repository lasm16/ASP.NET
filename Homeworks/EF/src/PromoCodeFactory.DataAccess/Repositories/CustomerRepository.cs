using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Persistense;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class CustomerRepository : EfRepository<Customer>
    {
        private readonly DataContext context;

        public CustomerRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public override async Task<Customer> GetByIdAsync(Guid id)
        {
            return await context.Customers
                .Include(x => x.CustomerPreferences)
                    .ThenInclude(cp => cp.Preference)
                .Include(x => x.PromoCodes)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await context.Customers
            .Include(x => x.CustomerPreferences)
                .ThenInclude(cp => cp.Preference)
            .Include(x => x.PromoCodes)
            .ToListAsync();
        }
    }
}
