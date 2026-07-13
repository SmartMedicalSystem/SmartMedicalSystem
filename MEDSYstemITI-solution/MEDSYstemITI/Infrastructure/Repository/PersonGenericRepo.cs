using Domain.Entities;
using Domain.Entities.Baseperson;
using Domain.Identity;
using Infrastructure.Context;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Infrastructure.Repository
{
    public class PersonGenericRepo<T>: GenericRepository<T> where T : BasePerson
    {
        private readonly EncryptionService _encryptionService;
        protected readonly ApplicationDbContext _context;

    

        public PersonGenericRepo(ApplicationDbContext context, EncryptionService encryptionService) : base(context)
        {
            _context = context;
        }

        public async Task<BasePerson?> FindBySSN(int ssn)
        {
            var EncryptedSSN =await  _encryptionService.Encrypt(ssn);
            return await _context.BasePersons.FindAsync(EncryptedSSN);
        }

        public async Task AddPerson(int ssn , BasePerson person ) 
        {
            var EncryptedSSN = await _encryptionService.Encrypt(ssn);
            person.EncryptedSSN = EncryptedSSN;
            
            await _context.BasePersons.AddAsync(person);

        }
    }

}  

