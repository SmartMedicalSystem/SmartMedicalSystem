using Domain.Entities.Person;
using Domain.Identity;
using Domain.IRepository;
using Infrastructure.Context;
using Infrastructure.Services;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Infrastructure.Repository
{
    public class PersonGenericRepo<T> : GenericRepository<BasePerson>, IPersonGenericRepo
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
            var people = await _context.BasePersons
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            foreach (var person in people)
            {
                // Case 1: value is stored as plain text
                if (person.EncryptedNationalId == ssn)
                {
                    person.DecryptedNationalId = ssn;
                    return person;
                }

                // Case 2: value is encrypted
                try
                {
                    var decrypted = _encryptionService.Decrypt(
                        person.EncryptedNationalId);

                    if (decrypted == ssn)
                    {
                        person.DecryptedNationalId = decrypted;
                        return person;
                    }
                }
                catch (CryptographicException)
                {
                    // Ignore records that cannot be decrypted
                }
            }

            return null;
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
            _context.Set<BasePerson>().Update(person);
            await _context.SaveChangesAsync();
        }

        public async Task<BasePerson?> FindByIdAsync(int id)
        {
            return await _context.BasePersons
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

    
    }

}

