using MilesTrading.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilesTrading.Business.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<IEnumerable<Transaction>> GetByBuyerIdAsync(int buyerId);
        Task<IEnumerable<Transaction>> GetBySellerIdAsync(int sellerId);
        Task<IEnumerable<Transaction>> GetByOfferIdAsync(int offerId);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<bool> UpdateStatusAsync(int id, TransactionStatus status);
        Task<bool> CompleteTransactionAsync(int id);
        Task<bool> CancelTransactionAsync(int id);
    }
}
