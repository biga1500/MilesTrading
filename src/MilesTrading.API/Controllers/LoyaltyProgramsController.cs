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
    public class LoyaltyProgramsController : ControllerBase
    {
        private readonly ILoyaltyProgramService _loyaltyProgramService;

        public LoyaltyProgramsController(ILoyaltyProgramService loyaltyProgramService)
        {
            _loyaltyProgramService = loyaltyProgramService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoyaltyProgram>>> GetLoyaltyPrograms()
        {
            var programs = await _loyaltyProgramService.GetAllAsync();
            return Ok(programs);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<LoyaltyProgram>>> GetActiveLoyaltyPrograms()
        {
            var programs = await _loyaltyProgramService.GetActiveAsync();
            return Ok(programs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoyaltyProgram>> GetLoyaltyProgram(int id)
        {
            var program = await _loyaltyProgramService.GetByIdAsync(id);

            if (program == null)
            {
                return NotFound();
            }

            return Ok(program);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LoyaltyProgram>> CreateLoyaltyProgram(LoyaltyProgram program)
        {
            try
            {
                var createdProgram = await _loyaltyProgramService.CreateAsync(program);
                return CreatedAtAction(nameof(GetLoyaltyProgram), new { id = createdProgram.Id }, createdProgram);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLoyaltyProgram(int id, LoyaltyProgram program)
        {
            if (id != program.Id)
            {
                return BadRequest();
            }

            try
            {
                var result = await _loyaltyProgramService.UpdateAsync(program);

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

        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleLoyaltyProgramStatus(int id)
        {
            try
            {
                var result = await _loyaltyProgramService.ToggleActiveStatusAsync(id);

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
