using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface ICustomerPreferenceRepository
    {
        Task AddAsync(CustomerPreference entity);
        Task DeleteAsync(CustomerPreference entity);
        Task<IEnumerable<CustomerPreference>> GetAllAsync();
    }
}
