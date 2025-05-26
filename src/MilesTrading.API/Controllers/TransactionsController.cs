using Microsoft.AspNetCore.Mvc;
using MilesTrading.Business.Interfaces;
using MilesTrading.Models.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace MilesTrading.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var transactions = await _transactionService.GetAllAsync();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpGet("buyer/{buyerId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByBuyer(int buyerId)
        {
            var transactions = await _transactionService.GetByBuyerIdAsync(buyerId);
            return Ok(transactions);
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsBySeller(int sellerId)
        {
            var transactions = await _transactionService.GetBySellerIdAsync(sellerId);
            return Ok(transactions);
        }

        [HttpGet("offer/{offerId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByOffer(int offerId)
        {
            var transactions = await _transactionService.GetByOfferIdAsync(offerId);
            return Ok(transactions);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            try
            {
                var createdTransaction = await _transactionService.CreateAsync(transaction);
                return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.Id }, createdTransaction);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTransactionStatus(int id, [FromBody] TransactionStatus status)
        {
            try
            {
                var result = await _transactionService.UpdateStatusAsync(id, status);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteTransaction(int id)
        {
            try
            {
                var result = await _transactionService.CompleteTransactionAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelTransaction(int id)
        {
            try
            {
                var result = await _transactionService.CancelTransactionAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
