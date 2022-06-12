using EthereumForward.Entity.JSON;

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
        public void Init(ForwardItem forward) 
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse("0.0.0.0"), forward.ServerPort);
                listener.Start();
                //新new一个X509Certificate对象（管理证书的），入参分别是：证书的路径，证书密码，如何导入证书
                X509Certificate serverCertificate = new X509Certificate2(Certificate, "weiai", X509KeyStorageFlags.DefaultKeySet);
                Thread thread = new Thread(() => HandleSocket(listener, serverCertificate, forward));
                thread.Start();
                Console.WriteLine($"SSL服务端打开成功，端口号：{forward.ServerPort}");
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
        private void HandleSocket(TcpListener listener, X509Certificate serverCertificate, ForwardItem forward)
        {
            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    listener.Start();
                    SslSocketServiceMessageProcessing sslServer = new SslSocketServiceMessageProcessing();
                    Thread thread;
                    if (forward.ClientAgreement.Equals("SSL"))
                    {
                        thread = new Thread(() => sslServer.ProcessClient(client, serverCertificate, forward));
                        Console.WriteLine($"已经接收到连接请求，IP：{client.Client.RemoteEndPoint.ToString()}协议为：SSL");
                    }
                    else 
                    {
                        thread = new Thread(() => sslServer.ProcessSocket(client, serverCertificate, forward));
                        Console.WriteLine($"已经接收到连接请求，IP：{client.Client.RemoteEndPoint.ToString()}协议为：TCP");
                    }
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
