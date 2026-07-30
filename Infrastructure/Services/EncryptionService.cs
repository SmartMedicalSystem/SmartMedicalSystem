using Microsoft.AspNetCore.DataProtection;

namespace Infrastructure.Services;
public class NationalIDEncryptionService
{
    private readonly IDataProtector _dataProtector;

    public NationalIDEncryptionService(IDataProtectionProvider dataProtectionProvidor)
    {
        _dataProtector = dataProtectionProvidor.CreateProtector("Secure SSN");
    }
    public async Task<string> Encrypt(string ssn)
    {
        return  _dataProtector.Protect(ssn);

    }
    public string Decrypt(string encryptedSsn)
    {
        return _dataProtector.Unprotect(encryptedSsn);
    }
}