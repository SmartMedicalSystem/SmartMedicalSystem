using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RequestLabsRepository : IRequestLabsRepository
    {

        private readonly ApplicationDbContext _context;

        public RequestLabsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public  async Task AddAsync(RequestLabs requestLabs)
        {
            await _context.RequestLabs.AddAsync(requestLabs);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RequestLabs requestLabs)
        {
            _context.RequestLabs.Remove(requestLabs);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RequestLabs>> GetAllAsync()
        {
            return await _context.RequestLabs.ToListAsync();
        }

        public async Task<RequestLabs> GetByIdAsync(int id)
        {
            return await _context.RequestLabs.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<RequestLabs>> GetBySessionIdAsync(int sessionId)
        {
            return await _context.RequestLabs
               .Where(x => x.SessionId == sessionId)
               .ToListAsync();
        }

        public async Task UpdateAsync(RequestLabs requestLabs)
        {
            _context.RequestLabs.Update(requestLabs);
            await _context.SaveChangesAsync();
        }
    }
}
