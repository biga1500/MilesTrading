using MilesTrading.Business.Interfaces;
using MilesTrading.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MilesTrading.Data;

namespace MilesTrading.Business.Services
{
    public class OfferService : IOfferService
    {
        private readonly MilesTradingContext _context;

        public OfferService(MilesTradingContext context)
        {
            _context = context;
        }

        public async Task<Offer> GetByIdAsync(int id)
        {
            return await _context.Offers
                .Include(o => o.User)
                .Include(o => o.LoyaltyProgram)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Offer>> GetAllAsync()
        {
            return await _context.Offers
                .Include(o => o.User)
                .Include(o => o.LoyaltyProgram)
                .ToListAsync();
        }

        public async Task<IEnumerable<Offer>> GetByUserIdAsync(int userId)
        {
            return await _context.Offers
                .Include(o => o.LoyaltyProgram)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Offer>> GetByProgramIdAsync(int programId)
        {
            return await _context.Offers
                .Include(o => o.User)
                .Where(o => o.ProgramId == programId && o.Status == OfferStatus.ACTIVE)
                .ToListAsync();
        }

        public async Task<IEnumerable<Offer>> GetByTypeAsync(OfferType type)
        {
            return await _context.Offers
                .Include(o => o.User)
                .Include(o => o.LoyaltyProgram)
                .Where(o => o.Type == type && o.Status == OfferStatus.ACTIVE)
                .ToListAsync();
        }

        public async Task<IEnumerable<Offer>> GetActiveOffersAsync()
        {
            return await _context.Offers
                .Include(o => o.User)
                .Include(o => o.LoyaltyProgram)
                .Where(o => o.Status == OfferStatus.ACTIVE)
                .ToListAsync();
        }

        public async Task<Offer> CreateAsync(Offer offer)
        {
            // Validar quantidade mínima de milhas
            if (offer.MilesAmount < 10000)
                throw new Exception("A quantidade mínima de milhas é 10.000");

            // Verificar se o programa de fidelidade existe
            var program = await _context.LoyaltyPrograms.FindAsync(offer.ProgramId);
            if (program == null)
                throw new Exception("Programa de fidelidade não encontrado");

            // Verificar se o usuário existe
            var user = await _context.Users.FindAsync(offer.UserId);
            if (user == null)
                throw new Exception("Usuário não encontrado");

            // Se for uma oferta de venda, verificar se o usuário tem saldo suficiente
            if (offer.Type == OfferType.SELL)
            {
                var userMiles = await _context.UserMiles
                    .FirstOrDefaultAsync(um => um.UserId == offer.UserId && um.ProgramId == offer.ProgramId);
                
                if (userMiles == null || userMiles.MilesBalance < offer.MilesAmount)
                    throw new Exception("Saldo de milhas insuficiente para criar esta oferta");
            }

            offer.Status = OfferStatus.ACTIVE;
            offer.CreatedAt = DateTime.Now;
            offer.UpdatedAt = DateTime.Now;

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            return offer;
        }

        public async Task<bool> UpdateAsync(Offer offer)
        {
            var existingOffer = await _context.Offers.FindAsync(offer.Id);
            if (existingOffer == null)
                return false;

            // Não permitir atualização de ofertas concluídas ou canceladas
            if (existingOffer.Status != OfferStatus.ACTIVE)
                throw new Exception("Apenas ofertas ativas podem ser atualizadas");

            // Validar quantidade mínima de milhas
            if (offer.MilesAmount < 10000)
                throw new Exception("A quantidade mínima de milhas é 10.000");

            existingOffer.MilesAmount = offer.MilesAmount;
            existingOffer.PricePerThousand = offer.PricePerThousand;
            existingOffer.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOfferAsync(int id)
        {
            var offer = await _context.Offers
                .Include(o => o.Transactions)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offer == null)
                return false;

            // Verificar se há transações pendentes ou concluídas
            if (offer.Transactions != null && offer.Transactions.Any(t => t.Status == TransactionStatus.PENDING || t.Status == TransactionStatus.COMPLETED))
                throw new Exception("Não é possível cancelar uma oferta com transações pendentes ou concluídas");

            offer.Status = OfferStatus.CANCELLED;
            offer.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
