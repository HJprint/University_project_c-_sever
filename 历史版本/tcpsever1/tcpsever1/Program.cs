using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace tcpsever1
{
    class Program
    {
        static void SqlLed()
        {

        }
        static void Main(string[] args)
        {
                //1.创建socket
                Socket tcpSever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EndPoint point = new IPEndPoint(IPAddress.Any, 7788);//IPEndPoint是对ip端口号做了一个封装的类
                tcpSever.Bind(point);//向操作系统申请一个可用的端口号做通信

                tcpSever.Listen(1000);   ///参数是最大连接数
                Console.WriteLine("开始监听");
                Socket clienSocket = tcpSever.Accept();///暂停当前线程，直到有一个客户端连接进来，之后进行下面的代码
                Console.WriteLine("有个小可爱连接过来了");

                while (true)
                {
                        byte[] data2 = new byte[1024];//创建一个字节数组用来做容器，去承接客户端发送过来的数据
                        int length = clienSocket.Receive(data2);
                        if (length != 0)
                        {
                            string message2 = Encoding.UTF8.GetString(data2, 0, length);
                            Console.WriteLine("接收到了一个客户端的数据：" + message2);
                        }
                }
                
        }
    }
}
