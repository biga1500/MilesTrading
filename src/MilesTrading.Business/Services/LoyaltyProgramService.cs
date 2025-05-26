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
    public class LoyaltyProgramService : ILoyaltyProgramService
    {
        private readonly MilesTradingContext _context;

        public LoyaltyProgramService(MilesTradingContext context)
        {
            _context = context;
        }

        public async Task<LoyaltyProgram> GetByIdAsync(int id)
        {
            return await _context.LoyaltyPrograms.FindAsync(id);
        }

        public async Task<IEnumerable<LoyaltyProgram>> GetAllAsync()
        {
            return await _context.LoyaltyPrograms.ToListAsync();
        }

        public async Task<IEnumerable<LoyaltyProgram>> GetActiveAsync()
        {
            return await _context.LoyaltyPrograms
                .Where(lp => lp.IsActive)
                .ToListAsync();
        }

        public async Task<LoyaltyProgram> CreateAsync(LoyaltyProgram program)
        {
            if (await _context.LoyaltyPrograms.AnyAsync(lp => lp.Name == program.Name))
                throw new Exception("Já existe um programa de fidelidade com este nome");

            program.CreatedAt = DateTime.Now;
            program.UpdatedAt = DateTime.Now;

            _context.LoyaltyPrograms.Add(program);
            await _context.SaveChangesAsync();

            return program;
        }

        public async Task<bool> UpdateAsync(LoyaltyProgram program)
        {
            var existingProgram = await _context.LoyaltyPrograms.FindAsync(program.Id);
            if (existingProgram == null)
                return false;

            existingProgram.Name = program.Name;
            existingProgram.Description = program.Description;
            existingProgram.IsActive = program.IsActive;
            existingProgram.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActiveStatusAsync(int id)
        {
            var program = await _context.LoyaltyPrograms.FindAsync(id);
            if (program == null)
                return false;

            program.IsActive = !program.IsActive;
            program.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
