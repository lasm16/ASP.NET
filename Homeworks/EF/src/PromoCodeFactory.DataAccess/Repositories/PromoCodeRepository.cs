using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Persistense;
using System;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class PromoCodeRepository(DataContext context) : EfRepository<PromoCode>(context)
    {
        public override async Task<PromoCode> GetByIdAsync(Guid id)
        {
            return await _context.PromoCodes
                .Include(x => x.Preference)
                .Include(x => x.PartnerManager)
                    .ThenInclude(x => x.Role)
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
