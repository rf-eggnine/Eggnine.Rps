//  ©️ 2024 by RF At EggNine All Rights Reserved
namespace Eggnine.Rps.Common;

public interface IEncryption
{
    public string Encrypt(string toEncrypt);

    public bool VerifyEncryption(string toVerify, string base64Hash);
}