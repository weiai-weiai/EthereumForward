using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EthereumForward.TCP
{
    public class SocketClient
    {
        public delegate void SrverCloseDelegate();                                                      //关闭服务端SOCKET的委托
        public SrverCloseDelegate srverClose = null;                                                           //关闭服务端SOCKET的委托
        public delegate void SrverSendDelegate(string str );                                            //给客户端发送消息的委托
        public SrverSendDelegate srverSend = null;                                                             //给客户端发送消息的委托
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);    //socket对象
        Thread thread = null;                                                                           //线程，用于监听消息
        int port = 0;
        string domain = "";
        public SocketClient(int port, string domain) 
        {
            this.port = port;
            this.domain = domain;
        }
        /// <summary>
        /// 客户端的初始化
        /// </summary>
        public void Start()
        {
            //这个地方进行DNS解析，不过好像过时了
            //测试能用管他是不是过时呢
            IPHostEntry ipHostInfo = Dns.Resolve(domain);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            client.Connect(remoteEP);
            thread = new Thread(() => Receive(client));
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
                client.Send(Encoding.Default.GetBytes(str));
            }
            catch (Exception ex) 
            {
                Console.WriteLine("发送给服务端出现错误："+ex.ToString());
                Close();
                srverClose();
            }
        }
        /// <summary>
        /// 接收信息的方法，并发送给服务端
        /// </summary>
        /// <param name="socket">socket对象</param>
        public void Receive(Socket socket)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bufferLong = socket.Receive(buffer);
                    if (srverSend != null)
                    {
                        if (bufferLong == 0)
                        {
                            throw new Exception("接收信息错误，关闭socket");
                        }
                        Console.WriteLine("矿池" + Encoding.Default.GetString(buffer, 0, bufferLong));
                        log4net.ILog loginfo = log4net.LogManager.GetLogger("");
                        loginfo.Info($"矿池:{Encoding.Default.GetString(buffer, 0, bufferLong)}");

                        srverSend(Encoding.Default.GetString(buffer, 0, bufferLong));
                    }
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine("客户端接收出现错误：" + e.ToString());
                Close();
                srverClose();
            }
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
                thread.Interrupt();
            }
            catch (Exception ex) 
            {
                Console.WriteLine("关闭socket出错"+ex.ToString());
            }
        }
    }
}
