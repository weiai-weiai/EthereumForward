using EthereumForward.Entity.JSON;
using EthereumForward.SSL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EthereumForward.TCP
{
    internal class ServiceMessageProcessing
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Thread thread;
        SslSocketClient sslClient;
        SocketClient client;
        /// <summary>
        /// 开启TCP客户端
        /// </summary>
        public void ProcessSocket(Socket server, ForwardItem forward)
        {
            client = new SocketClient(forward.ClientPort, forward.ClientIp);
            client.Start();
            client.srverClose = new SocketClient.SrverCloseDelegate(Close);
            client.srverSend = new SocketClient.SrverSendDelegate(Send);
            socket = server;
            thread = new Thread(() => Receive(server, client));
            thread.Start();
        }
        /// <summary>
        /// 开启SSL客户端
        /// </summary>
        /// <param name="server"></param>
        /// <param name="forward"></param>
        public void SslProcessSocket(Socket server, ForwardItem forward)
        {
            sslClient = new SslSocketClient(forward.ClientPort, forward.ClientIp);
            sslClient.Start();
            sslClient.srverClose = new SslSocketClient.SrverCloseDelegate(Close);
            sslClient.srverSend = new SslSocketClient.SrverSendDelegate(Send);
            socket = server;
            thread = new Thread(() => Receive(server , sslClient));
            thread.Start();
        }
        /// <summary>
        /// 给客户端发送消息
        /// </summary>
        /// <param name="str"></param>
        public void Send(string str)
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Send(Encoding.Default.GetBytes(str));
                }
                else 
                {
                    throw new Exception("客户端已断开");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送给客户端出现错误：" + ex.ToString());
                Close();
                if (client != null) 
                {
                    client.Close();
                }
                if (sslClient != null)
                {
                    sslClient.Close();
                }
            }
        }
        /// <summary>
        /// Tcp接收方法
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="client"></param>
        public void Receive(Socket socket, SocketClient client)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bufferLong = socket.Receive(buffer);
                    if (bufferLong == 0)
                    {
                        throw new Exception("接收信息错误，关闭socket");
                    }
                    Console.WriteLine("矿机：" + Encoding.Default.GetString(buffer, 0, bufferLong));
                    log4net.ILog loginfo = log4net.LogManager.GetLogger("");
                    loginfo.Info($"矿机{Encoding.Default.GetString(buffer, 0, bufferLong)}");
                    client.Send(Encoding.Default.GetString(buffer, 0, bufferLong));
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
        /// SSL接收方法
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sslClient"></param>
        public void Receive(Socket socket , SslSocketClient sslClient)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bufferLong = socket.Receive(buffer);
                    if (bufferLong == 0)
                    {
                        throw new Exception("接收信息错误，关闭socket");
                    }
                    Console.WriteLine("矿机：" + Encoding.Default.GetString(buffer, 0, bufferLong));
                    sslClient.Send(Encoding.Default.GetString(buffer, 0, bufferLong));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务端接收出现错误：" + ex.ToString());
                Close();
                sslClient.Close();
            }
        }
        /// <summary>
        /// 关闭方法
        /// </summary>
        public void Close()
        {
            try
            {
                socket.Close();
                socket.Dispose();
                thread.Interrupt();
            }
            catch (Exception ex)
            {
                Console.WriteLine("关闭socket出错" + ex.ToString());
            }
        }
    }
}
