using EthereumForward.Entity.JSON;
using EthereumForward.TCP;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EthereumForward.SSL
{
    internal class SslSocketServiceMessageProcessing
    {
        SslStream sslStream;
        TcpClient sslTcpClient;
        Thread thread;
        SslSocketClient sslClient;
        SocketClient Client;
        /// <summary>
        /// 验证证书
        /// </summary>
        /// <param name="client">用户的socket</param>
        /// <param name="serverCertificate">证书对象</param>
        public void ProcessClient(TcpClient client, X509Certificate serverCertificate, ForwardItem forward)
        {
            sslStream = new SslStream(client.GetStream(), false);
            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls12, true);
                this.sslTcpClient = client;
                sslClient = new SslSocketClient(forward.ClientPort, forward.ClientIp);
                sslClient.srverClose = new SslSocketClient.SrverCloseDelegate(Close);
                sslClient.srverSend = new SslSocketClient.SrverSendDelegate(Send);
                sslClient.Start();
                thread = new Thread(() => Read(sslStream, sslClient));
                thread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Close();
                return;
            }
        }
        /// <summary>
        /// 开启TCP客户端
        /// </summary>
        public void ProcessSocket(TcpClient client, X509Certificate serverCertificate, ForwardItem forward)
        {
            sslStream = new SslStream(client.GetStream(), false);
            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls12, true);
                this.sslTcpClient = client;
                Client = new SocketClient(forward.ClientPort, forward.ClientIp);
                Client.Start();
                Client.srverClose = new SocketClient.SrverCloseDelegate(Close);
                Client.srverSend = new SocketClient.SrverSendDelegate(Send);
                this.sslTcpClient = client;
                thread = new Thread(() => Read(Client));
                thread.Start();
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
                Close();
                return;
            }
        }
        /// <summary>
        /// 给客户端发送消息
        /// </summary>
        /// <param name="str">需要发送的消息</param>
        public void Send(string str)
        {
            try
            {
                sslStream.Write(Encoding.Default.GetBytes(str));
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送给客户端出现错误：" + ex.ToString());
                if (Client != null)
                {
                    Client.Close();
                }
                if (sslClient != null)
                {
                    sslClient.Close();
                }

            }
        }
        /// <summary>
        /// Ssl接收方法
        /// </summary>
        public void Read(SslStream sslStream,SslSocketClient client)
        {
            try
            {
                while (true)
                {
                    string messageData = ReadMessage(sslStream);
                    Console.WriteLine(messageData);
                    client.Send(messageData);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Tcp接收方法
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="client"></param>
        public void Read(SocketClient client)
        {
            try
            {
                while (true)
                {
                    string messageData = ReadMessage(sslStream);
                    Console.WriteLine(messageData);
                    client.Send(messageData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务端接收出现错误：" + ex.ToString());
                Close();
                client.Close();
            }
        }

        /// <summary>
        /// 读取消息并且解密，这个地方代码没读，直接拿过来就用了
        /// </summary>
        /// <param name="sslStream"></param>
        /// <returns>解析出来的消息</returns>
        public string ReadMessage(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                if (messageData.ToString().IndexOf("") != -1)
                {
                    break;
                }
            }
            while (bytes != 0);
            return messageData.ToString();
        }

        void Close()
        {
            sslStream.Close();
            sslStream.Dispose();
            sslTcpClient.Close();
            sslTcpClient.Dispose();
        }

    }
}
