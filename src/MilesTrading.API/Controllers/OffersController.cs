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
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Offer>>> GetOffers()
        {
            var offers = await _offerService.GetAllAsync();
            return Ok(offers);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Offer>>> GetActiveOffers()
        {
            var offers = await _offerService.GetActiveOffersAsync();
            return Ok(offers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Offer>> GetOffer(int id)
        {
            var offer = await _offerService.GetByIdAsync(id);

            if (offer == null)
            {
                return NotFound();
            }

            return Ok(offer);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Offer>>> GetOffersByUser(int userId)
        {
            var offers = await _offerService.GetByUserIdAsync(userId);
            return Ok(offers);
        }

        [HttpGet("program/{programId}")]
        public async Task<ActionResult<IEnumerable<Offer>>> GetOffersByProgram(int programId)
        {
            var offers = await _offerService.GetByProgramIdAsync(programId);
            return Ok(offers);
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<Offer>>> GetOffersByType(OfferType type)
        {
            var offers = await _offerService.GetByTypeAsync(type);
            return Ok(offers);
        }

        [HttpPost]
        public async Task<ActionResult<Offer>> CreateOffer(Offer offer)
        {
            try
            {
                var createdOffer = await _offerService.CreateAsync(offer);
                return CreatedAtAction(nameof(GetOffer), new { id = createdOffer.Id }, createdOffer);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOffer(int id, Offer offer)
        {
            if (id != offer.Id)
            {
                return BadRequest();
            }

            try
            {
                var result = await _offerService.UpdateAsync(offer);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOffer(int id)
        {
            try
            {
                var result = await _offerService.CancelOfferAsync(id);

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
