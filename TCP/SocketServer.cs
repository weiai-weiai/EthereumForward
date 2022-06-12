using EthereumForward.Entity.JSON;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EthereumForward.TCP
{
    internal class SocketServer
    {
        /// <summary>
        /// 开启Socket监听
        /// </summary>
        public void Init(ForwardItem forward)
        {
            try
            {
                Socket serviceSocketListener; //Socke监听处理请求
                serviceSocketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serviceSocketListener.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), forward.ServerPort)); //IP和端口应该是可配置
                serviceSocketListener.Listen();
                Thread handleSocket = new Thread(() => HandleSocket(serviceSocketListener,forward));
                handleSocket.Start();
                Console.WriteLine($"TCP打开成功，端口号：{forward.ServerPort}");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// 监听链接
        /// </summary>
        private void HandleSocket(Socket socket, ForwardItem forward)
        {
            while (true)
            {
                try
                {
                    ServiceMessageProcessing serviceMessage= new ServiceMessageProcessing();
                    Socket currSocket = socket.Accept();
                    //此处判断协议，无法解析的协议统统归为TCP
                    if (forward.ClientAgreement.Equals("SSL"))
                    {
                        serviceMessage.SslProcessSocket(currSocket, forward);
                        Console.WriteLine($"已经接收到连接请求，IP：{currSocket.RemoteEndPoint.ToString()}协议为：SSL");
                    }
                    else 
                    {
                        serviceMessage.ProcessSocket(currSocket, forward);
                        Console.WriteLine($"已经接收到连接请求，IP：{currSocket.RemoteEndPoint.ToString()}协议为：TCP");
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现错误{ex.ToString()}");
                }
            }
        }
    }
}