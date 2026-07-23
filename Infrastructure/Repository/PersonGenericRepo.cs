using Domain.Entities.Person;
using Domain.IRepository;
using Infrastructure.Context;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Infrastructure.Repository
{
    public class PersonGenericRepo<T> : GenericRepository<T>, IPersonGenericRepo where T : BasePerson
    {
        private readonly NationalIDEncryptionService _encryptionService;
        protected readonly ApplicationDbContext _context;

        public PersonGenericRepo(ApplicationDbContext context, NationalIDEncryptionService encryptionService) : base(context)
        {
            _context = context;
            _encryptionService = encryptionService;
        }
        public async Task<BasePerson?> FindBySSN(string ssn)
        {
            return await _context.BasePersons
               .FirstOrDefaultAsync(x =>
                   !x.IsDeleted &&
                   x.EncryptedNationalId == ssn);
        }

        public async Task AddPerson(string ssn, BasePerson person)
        {
            var encrypted = await _encryptionService.Encrypt(ssn);
            person.EncryptedNationalId = encrypted;

            await _context.AddAsync(person);
            await _context.SaveChangesAsync();

         
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

