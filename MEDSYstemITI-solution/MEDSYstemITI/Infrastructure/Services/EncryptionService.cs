using Microsoft.AspNetCore.DataProtection;

namespace Infrastructure.Services;
public class EncryptionService
{
    private readonly IDataProtector _dataProtector;

    public EncryptionService(IDataProtectionProvider dataProtectionProvidor)
    {
        _dataProtector = dataProtectionProvidor.CreateProtector("Secure SSN");
    }
    public async Task<string> Encrypt(string ssn)
    {
        return  _dataProtector.Protect(ssn);

    }
}