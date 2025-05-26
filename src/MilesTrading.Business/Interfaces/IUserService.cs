using MilesTrading.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilesTrading.Business.Interfaces
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user, string password);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ValidateCredentialsAsync(string email, string password);
        Task<string> GenerateJwtTokenAsync(User user);
    }
}
