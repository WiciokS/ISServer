using System.Security.Cryptography;
using System.Text;

public class DigitalSignature
{
    private RSA _rsa;

    public DigitalSignature()
    {
        _rsa = RSA.Create();
    }

    public byte[] SignMessage(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        return _rsa.SignData(messageBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public RSAParameters GetPublicKey()
    {
        return _rsa.ExportParameters(false);
    }

    public bool VerifySignature(string message, byte[] signature, RSAParameters publicKey)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        _rsa.ImportParameters(publicKey);
        return _rsa.VerifyData(messageBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
