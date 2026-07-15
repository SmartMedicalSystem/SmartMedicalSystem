using Domain.Entities;
using Domain.Entities.Baseperson;
using Domain.Identity;
using Domain.IRepository;
using Infrastructure.Context;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class PersonGenericRepo<T> : GenericRepository<T>, IPersonGenericRepo where T : BasePerson
    {
        private readonly EncryptionService _encryptionService;
        protected readonly ApplicationDbContext _context;

        public PersonGenericRepo(ApplicationDbContext context, EncryptionService encryptionService) : base(context)
        {
            _context = context;
            _encryptionService = encryptionService;
        }
        public async Task<BasePerson?> FindBySSN(string ssn)
        {
            var encryptedSSN = await _encryptionService.Encrypt(ssn);
            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(e => e.EncryptedSSN == encryptedSSN && !e.IsDeleted);
            return entity as BasePerson;
        }

        public async Task AddPerson(string ssn , BasePerson person ) 
        {
            var encryptedSSN = await _encryptionService.Encrypt(ssn);
            person.EncryptedSSN = encryptedSSN;
            await _context.Set<T>().AddAsync((T)person);
        }

        public async Task UpdateSSNAsync(BasePerson person, string ssn)
        {
            var encryptedSSN = await _encryptionService.Encrypt(ssn);
            person.EncryptedSSN = encryptedSSN;
            _context.Set<T>().Update((T)person);
            await _context.SaveChangesAsync();
        }
    }

}  

