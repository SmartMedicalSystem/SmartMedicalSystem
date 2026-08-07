using Microsoft.AspNetCore.DataProtection;

namespace Infrastructure.Services
{
    public class NationalIDEncryptionService
    {
        private readonly IDataProtector _dataProtector;

        public NationalIDEncryptionService(
            IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector =
                dataProtectionProvider.CreateProtector("Secure SSN");
        }

        public Task<string> Encrypt(string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
            {
                throw new ArgumentException(
                    "National ID cannot be empty.",
                    nameof(nationalId));
            }

            return Task.FromResult(
                _dataProtector.Protect(nationalId));
        }

        public string Decrypt(string encryptedNationalId)
        {
            if (string.IsNullOrWhiteSpace(encryptedNationalId))
                return string.Empty;

            return _dataProtector.Unprotect(encryptedNationalId);
        }
    }
}