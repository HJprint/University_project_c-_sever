using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace thread1
{
    class Program
    {    
        public static bool flage = false;
        public static string message;
        static void Main(string[] args)
        {
            
            Tcpsever_Connect tcpsever_Connect = new Tcpsever_Connect(7788);//硬件进程
            Tcpsever_Connect tcpsever_Connect1 = new Tcpsever_Connect(7889);//网页进程
            Console.ReadKey();
        }
    }

    class Tcpsever_Connect
    {
        Socket clienSocket;//接收硬件
        public Tcpsever_Connect( int port)
        {
            Action< int> action = TcpConnect;
            action.BeginInvoke( port, null, null);

        }

        public void TcpConnect( int port)
        {
                //1.创建socket
                Socket tcpSever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EndPoint point = new IPEndPoint(IPAddress.Any, port);//IPEndPoint是对ip端口号做了一个封装的类
                tcpSever.Bind(point);//向操作系统申请一个可用的端口号做通信
                                     //开始监听（等待客户端连接）
                tcpSever.Listen(1000);
                Console.WriteLine("开始监听硬件");
                clienSocket = tcpSever.Accept();//暂停当前线程，直到有一个客户端连接进来，之后进行下面的代码

                //使用返回的soket跟客户端做通信
                string message = "硬件连接成功";
                byte[] data = Encoding.UTF8.GetBytes(message);///对字符串做编码，得到一个字符串的字节数组
                clienSocket.Send(data);
                Console.WriteLine("向硬件发送了一个数据");
            
        }

        //接收客服端的数据
        public void receive_Data()
        {
            byte[] data2 = new byte[1024];//创建一个字节数组用来做容器，去承接客户端发送过来的数据
            int length = clienSocket.Receive(data2);
            string message2 = Encoding.UTF8.GetString(data2, 0, length);
            Console.WriteLine("接收到了一个客户端的数据：" + message2);
        }

        ///向客户端发送数据
        public void send_Data(string message1)
        {
            byte[] data1 = Encoding.UTF8.GetBytes(message1);//对字符串做编码，得到一个字符串的字节数组
            clienSocket.Send(data1);
            Console.WriteLine("向客户端发送了一个数据:" + message1);
        }

    }
}

