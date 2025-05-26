using MilesTrading.Business.Interfaces;
using System.Threading.Tasks;

namespace MilesTrading.Business.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(MilesTrading.Models.Entities.User user);
    }
}
