using EthereumForward.JSON;

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
        SocketClient client = null;
        Thread thread;
        /// <summary>
        /// 处理Socket信息
        /// </summary>
        public void ProcessSocket(Socket server, ForwardItem forward)
        {
            client = new SocketClient(forward.ClientPort, forward.ClientIp);
            client.Start();
            client.srverClose = new SocketClient.SrverCloseDelegate(Close);
            client.srverSend = new SocketClient.SrverSendDelegate(Send);
            socket = server;
            thread = new Thread(() => Receive(server));
            thread.Start();
        }
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
                client.Close();
            }
        }

        public void Receive(Socket socket)
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
