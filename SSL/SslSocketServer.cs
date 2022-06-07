using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EthereumForward.SSL
{
    internal class SslSocketServer
    {
        string Certificate = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "user-rsa.pfx";      //这是证书的路径 后缀必须为pfx，不然会报错
        /// <summary>
        /// 开启SSL服务端
        /// </summary>
        public void Init() 
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse("0.0.0.0"), 901);
                listener.Start();
                //新new一个X509Certificate对象（管理证书的），入参分别是：证书的路径，证书密码，如何导入证书
                X509Certificate serverCertificate = new X509Certificate2(Certificate, "weiai", X509KeyStorageFlags.DefaultKeySet);
                Thread thread = new Thread(() => HandleSocket(listener, serverCertificate));
                thread.Start();
                Console.WriteLine("打开成功");
            }
            catch (Exception ex) 
            {
                Console.WriteLine("SSL服务端打开时出现错误"+ ex.ToString());
            }
        }
        /// <summary>
        /// 等待客户端连接
        /// </summary>
        /// <param name="listener">服务端的TcpListener</param>
        /// <param name="serverCertificate">证书对象</param>
        private void HandleSocket(TcpListener listener, X509Certificate serverCertificate)
        {
            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    listener.Start();
                    SslSocketServiceMessageProcessing sslServer = new SslSocketServiceMessageProcessing();
                    Thread thread = new Thread(() => sslServer.ProcessClient(client, serverCertificate));
                    thread.Start();
                    Console.WriteLine($"socket服务端已接收到请求，IP：{(client.Client.RemoteEndPoint as IPEndPoint).Address.ToString()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
