using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EthereumForward.SSL
{
    /// <summary>
    /// ssl客户端
    /// </summary>
    public class SslSocketClient
    {
        public delegate void SrverCloseDelegate();                                                      //关闭服务端SOCKET的委托
        public SrverCloseDelegate srverClose;                                                           //关闭服务端SOCKET的委托
        public delegate void SrverSendDelegate(string str);                                             //给客户端发送消息的委托
        public SrverSendDelegate srverSend;
        int port = 0;
        string domain = "";//给客户端发送消息的委托
        public SslSocketClient(int port, string domain) 
        {
            this.port = port;
            this.domain = domain;
        }
        /// <summary>
        /// 这个方法是确认证书是否有效，
        /// 注释掉的代码是判断是判断证书是否有效
        /// </summary>
        /// <returns>这个地方返回true就是有效，返回false就是无效</returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //应该是判断有没有错误，没有就返回有效，有就返回无效
            //if (sslPolicyErrors == SslPolicyErrors.None)
            return true;
            //Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            //return false;
        }
        TcpClient client = new TcpClient();
        Thread thread;
        SslStream sslStream;
        public void Start()
        {
            //这个地方进行DNS解析，不过好像过时了
            //测试能用管他是不是过时呢
            IPHostEntry ipHostInfo = Dns.Resolve(domain);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            client.Connect(remoteEP);
            //获取证书，以判断连接是否有效
            sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            sslStream.AuthenticateAsClient("asia2.ethermine.org");
            thread = new Thread(() => Receive(sslStream));
            thread.Start();
        }
        /// <summary>
        /// 客户端的发送方法
        /// </summary>
        /// <param name="str">要发送的信息</param>
        public void Send(string str)
        {
            try
            {
                sslStream.Write(Encoding.Default.GetBytes(str));
                sslStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送给服务端出现错误：" + ex.ToString());
                Close();
                srverClose();
            }
        }
        public void Receive(SslStream sslStream) 
        {
            try
            {
                while (true)
                {
                    string str = ReadMessage(sslStream);
                    srverSend(str);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("客户端接收出现错误"+ex.ToString());
                Close();
                srverClose();
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
        /// <summary>
        /// 关闭socket的方法
        /// </summary>
        public void Close()
        {
            try
            {
                client.Close();
                client.Dispose();
                sslStream.Close();
                sslStream.Dispose();
                thread.Interrupt();
            }
            catch (Exception ex)
            {
                Console.WriteLine("关闭socket出错" + ex.ToString());
            }
        }

    }
}
