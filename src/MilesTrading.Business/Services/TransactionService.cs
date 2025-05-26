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
    public class TransactionService : ITransactionService
    {
        private readonly MilesTradingContext _context;

        public TransactionService(MilesTradingContext context)
        {
            _context = context;
        }

        public async Task<Transaction> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Offer)
                    .ThenInclude(o => o.LoyaltyProgram)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .Include(t => t.Offer)
                    .ThenInclude(o => o.LoyaltyProgram)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByBuyerIdAsync(int buyerId)
        {
            return await _context.Transactions
                .Include(t => t.Offer)
                    .ThenInclude(o => o.LoyaltyProgram)
                .Include(t => t.Seller)
                .Where(t => t.BuyerId == buyerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetBySellerIdAsync(int sellerId)
        {
            return await _context.Transactions
                .Include(t => t.Offer)
                    .ThenInclude(o => o.LoyaltyProgram)
                .Include(t => t.Buyer)
                .Where(t => t.SellerId == sellerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByOfferIdAsync(int offerId)
        {
            return await _context.Transactions
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .Where(t => t.OfferId == offerId)
                .ToListAsync();
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Obter a oferta
                var offer = await _context.Offers
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == transaction.OfferId);

                if (offer == null)
                    throw new Exception("Oferta não encontrada");

                if (offer.Status != OfferStatus.ACTIVE)
                    throw new Exception("Esta oferta não está ativa");

                // Definir comprador e vendedor com base no tipo de oferta
                if (offer.Type == OfferType.BUY)
                {
                    transaction.BuyerId = offer.UserId;
                    // O vendedor é o usuário que está respondendo à oferta de compra
                }
                else // SELL
                {
                    transaction.SellerId = offer.UserId;
                    // O comprador é o usuário que está respondendo à oferta de venda
                }

                // Verificar se o comprador e o vendedor existem
                var buyer = await _context.Users.FindAsync(transaction.BuyerId);
                var seller = await _context.Users.FindAsync(transaction.SellerId);

                if (buyer == null || seller == null)
                    throw new Exception("Comprador ou vendedor não encontrado");

                // Definir valores da transação
                transaction.TotalAmount = offer.MilesAmount;
                transaction.TotalValue = (offer.PricePerThousand * offer.MilesAmount) / 1000;
                transaction.Status = TransactionStatus.PENDING;
                transaction.CreatedAt = DateTime.Now;
                transaction.UpdatedAt = DateTime.Now;

                // Salvar a transação
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();
                return transaction;
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(int id, TransactionStatus status)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return false;

            transaction.Status = status;
            transaction.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteTransactionAsync(int id)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Obter a transação com todos os dados relacionados
                var transaction = await _context.Transactions
                    .Include(t => t.Offer)
                        .ThenInclude(o => o.LoyaltyProgram)
                    .Include(t => t.Buyer)
                    .Include(t => t.Seller)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (transaction == null)
                    return false;

                if (transaction.Status != TransactionStatus.PENDING)
                    throw new Exception("Apenas transações pendentes podem ser concluídas");

                // Obter o saldo de milhas do vendedor
                var sellerMiles = await _context.UserMiles
                    .FirstOrDefaultAsync(um => um.UserId == transaction.SellerId && um.ProgramId == transaction.Offer.ProgramId);

                if (sellerMiles == null || sellerMiles.MilesBalance < transaction.TotalAmount)
                    throw new Exception("Saldo de milhas do vendedor insuficiente");

                // Obter ou criar o saldo de milhas do comprador
                var buyerMiles = await _context.UserMiles
                    .FirstOrDefaultAsync(um => um.UserId == transaction.BuyerId && um.ProgramId == transaction.Offer.ProgramId);

                if (buyerMiles == null)
                {
                    buyerMiles = new UserMiles
                    {
                        UserId = transaction.BuyerId,
                        ProgramId = transaction.Offer.ProgramId,
                        MilesBalance = 0,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.UserMiles.Add(buyerMiles);
                    await _context.SaveChangesAsync();
                }

                // Transferir milhas do vendedor para o comprador
                sellerMiles.MilesBalance -= transaction.TotalAmount;
                sellerMiles.UpdatedAt = DateTime.Now;

                buyerMiles.MilesBalance += transaction.TotalAmount;
                buyerMiles.UpdatedAt = DateTime.Now;

                // Transferir valor financeiro do comprador para o vendedor
                transaction.Buyer.FinancialBalance -= transaction.TotalValue;
                transaction.Seller.FinancialBalance += transaction.TotalValue;

                // Atualizar status da transação
                transaction.Status = TransactionStatus.COMPLETED;
                transaction.UpdatedAt = DateTime.Now;

                // Atualizar status da oferta se for uma oferta de venda completa
                if (transaction.Offer.Type == OfferType.SELL && transaction.TotalAmount == transaction.Offer.MilesAmount)
                {
                    transaction.Offer.Status = OfferStatus.COMPLETED;
                    transaction.Offer.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return true;
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return false;

            if (transaction.Status != TransactionStatus.PENDING)
                throw new Exception("Apenas transações pendentes podem ser canceladas");

            transaction.Status = TransactionStatus.CANCELLED;
            transaction.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
