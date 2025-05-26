using MilesTrading.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilesTrading.Business.Interfaces
{
    public interface IOfferService
    {
        Task<Offer> GetByIdAsync(int id);
        Task<IEnumerable<Offer>> GetAllAsync();
        Task<IEnumerable<Offer>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Offer>> GetByProgramIdAsync(int programId);
        Task<IEnumerable<Offer>> GetByTypeAsync(OfferType type);
        Task<IEnumerable<Offer>> GetActiveOffersAsync();
        Task<Offer> CreateAsync(Offer offer);
        Task<bool> UpdateAsync(Offer offer);
        Task<bool> CancelOfferAsync(int id);
    }
}
