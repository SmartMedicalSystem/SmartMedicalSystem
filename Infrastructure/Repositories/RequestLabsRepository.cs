using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    public class RequestLabsrepository
        : GenericRepo<RequestLabs>,
          IRequestLabsRepository
    {
        public RequestLabsrepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<RequestLabs>>
            GetBySessionIdAsync(int sessionId)
        {
            return await dbSet
                .Where(x => x.SessionId == sessionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RequestLabs>>
            GetPendingRequestsAsync()
        {
            return await dbSet
                .Where(x =>
                    x.Status == LabRequestStatus.Pending)
                .ToListAsync();
        }
    }
}
