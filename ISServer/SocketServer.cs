using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class SocketServer
{
    public event EventHandler<byte[]> DataReceived;

    public void StartListening(int port)
    {
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

        using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            while (true)
            {
                Socket handler = listener.Accept();
                byte[] buffer = new byte[4096];
                int bytesReceived = handler.Receive(buffer);

                if (bytesReceived > 0)
                {
                    byte[] data = new byte[bytesReceived];
                    Array.Copy(buffer, 0, data, 0, bytesReceived);
                    DataReceived?.Invoke(this, data);
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}
