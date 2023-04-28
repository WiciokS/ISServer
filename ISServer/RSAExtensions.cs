using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

public static class RSAExtensions
{
    public static string ExportPublicKeyToXml(this RSA rsa)
    {
        RSAParameters parameters = rsa.ExportParameters(false);
        XElement modulus = new XElement("Modulus", Convert.ToBase64String(parameters.Modulus));
        XElement exponent = new XElement("Exponent", Convert.ToBase64String(parameters.Exponent));
        XElement rsaKeyValue = new XElement("RSAKeyValue", modulus, exponent);

        return rsaKeyValue.ToString();
    }

    public static void ImportPublicKeyFromXml(this RSA rsa, string xmlString)
    {
        XElement rsaKeyValue = XElement.Parse(xmlString);
        RSAParameters parameters = new RSAParameters
        {
            Modulus = Convert.FromBase64String(rsaKeyValue.Element("Modulus").Value),
            Exponent = Convert.FromBase64String(rsaKeyValue.Element("Exponent").Value)
        };

        rsa.ImportParameters(parameters);
    }
}
