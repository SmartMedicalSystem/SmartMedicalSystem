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
            var encrypted = await _encryptionService.Encrypt(ssn);
            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(e => e.EncryptedNationalId == encrypted && !e.IsDeleted);
            return entity as BasePerson;
        }

        public async Task AddPerson(string ssn , BasePerson person ) 
        {
            var encrypted = await _encryptionService.Encrypt(ssn);
            person.EncryptedNationalId = encrypted;
            await _context.Set<T>().AddAsync((T)person);
        }

        public async Task UpdateSSNAsync(BasePerson person, string ssn)
        {
            var encrypted = await _encryptionService.Encrypt(ssn);
            person.EncryptedNationalId = encrypted;
            _context.Set<T>().Update((T)person);
            await _context.SaveChangesAsync();
        }
    }

}  

