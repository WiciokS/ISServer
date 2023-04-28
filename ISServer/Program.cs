using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

public class Program
{
    private static int _dataCounter = 0;
    private static string _receivedPublicKeyXml;
    private static string _receivedMessage;
    private static byte[] _receivedSignature;
    private static DigitalSignature _digitalSignature = new DigitalSignature();
    private static SocketClient _socketClient = new SocketClient();

    public static void Main()
    {
        SocketServer socketServer = new SocketServer();
        socketServer.DataReceived += HandleData;
        socketServer.StartListening(9000);
    }

    private static void HandleData(object sender, byte[] data)
    {
        if (_dataCounter == 0)
        {
            _receivedPublicKeyXml = Encoding.UTF8.GetString(data);
            Console.WriteLine("Received Public Key (XML):\n" + _receivedPublicKeyXml);
            _socketClient.SendData("127.0.0.1", 9001, data);
        }
        else if (_dataCounter == 1)
        {
            _receivedMessage = Encoding.UTF8.GetString(data);
            Console.WriteLine("Received Message:\n" + _receivedMessage);
            _socketClient.SendData("127.0.0.1", 9001, data);
        }
        else if (_dataCounter == 2)
        {
            _receivedSignature = data;
            Console.WriteLine("Received Signature (Base64):\n" + Convert.ToBase64String(_receivedSignature));

            // Deserialize the public key from XML
            RSAParameters publicKey;
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(_receivedPublicKeyXml);
                publicKey = rsa.ExportParameters(false);
            }

            bool isSignatureValid = _digitalSignature.VerifySignature(_receivedMessage, _receivedSignature, publicKey);
            Console.WriteLine($"Is the signature valid? {isSignatureValid}");

            // Prompt user to change the signature value
            Console.Write("Do you want to change the signature value? (y/n): ");
            string userInput = Console.ReadLine();

            if (userInput.ToLower() == "y")
            {
                Console.Write("Enter the index of the byte you want to change (0 - {0}): ", _receivedSignature.Length - 1);
                int byteIndex = int.Parse(Console.ReadLine());
                _receivedSignature[byteIndex] ^= 0xFF; // Invert the chosen byte of the signature
            }

            // Send the (changed) signature to the third application
            _socketClient.SendData("127.0.0.1", 9001, _receivedSignature);

            // Reset the counter
            _dataCounter = -1;
        }

        _dataCounter++;
    }
}
