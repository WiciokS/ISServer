using System;
using System.Net.Sockets;

public class SocketClient
{
    public void SendData(string ipAddress, int port, byte[] data)
    {
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            socket.Connect(ipAddress, port);
            socket.Send(data);
        }
    }
}
