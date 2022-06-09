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
        SslSocketClient client1;
        TcpClient client;
        /// <summary>
        /// 验证证书
        /// </summary>
        /// <param name="client">用户的socket</param>
        /// <param name="serverCertificate">证书对象</param>
        public void ProcessClient(TcpClient client, X509Certificate serverCertificate)
        {
            sslStream = new SslStream(client.GetStream(), false);
            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls12, true);
                this.client = client;
                client1 = new SslSocketClient();
                client1.srverClose = new SslSocketClient.SrverCloseDelegate(Close);
                client1.srverSend = new SslSocketClient.SrverSendDelegate(Send);
                client1.Start();
                Thread thread = new Thread(Read);
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
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 监听读取数据
        /// </summary>
        public void Read()
        {
            try
            {
                while (true)
                {
                    string messageData = ReadMessage(sslStream);
                    Console.WriteLine(messageData);
                    client1.Send(messageData);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
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
            client.Close();
            client.Dispose();
        }

    }
}
