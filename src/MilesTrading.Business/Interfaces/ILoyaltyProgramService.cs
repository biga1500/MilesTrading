using MilesTrading.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilesTrading.Business.Interfaces
{
    public interface ILoyaltyProgramService
    {
        Task<LoyaltyProgram> GetByIdAsync(int id);
        Task<IEnumerable<LoyaltyProgram>> GetAllAsync();
        Task<IEnumerable<LoyaltyProgram>> GetActiveAsync();
        Task<LoyaltyProgram> CreateAsync(LoyaltyProgram program);
        Task<bool> UpdateAsync(LoyaltyProgram program);
        Task<bool> ToggleActiveStatusAsync(int id);
    }
}
