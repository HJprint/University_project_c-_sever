using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;//使用Socket
using System.Net;//使用IPAddress
using System.Threading;

namespace serve
{

    class Program
    {
        /// <summary>
        /// 判断客户端是否断开
        /// </summary>
        /// <param name="tcpClient">传入一个Socket</param>
        static void Process(Socket tcpClient)
        {
            while (true)
            {
                Thread.Sleep(2000);
                if (!tcpClient.Connected)
                {
                    Console.WriteLine("客户端已经与服务器断开");
                }
            }
        }

        /// <summary>
        /// 等待客户端连接
        /// </summary>
        /// <param name="connectSocket">新的Socket</param>
        /// <param name="tcpServer"></param>
        static void Connect(Socket connectSocket, Socket tcpServer)
        {
            while (!connectSocket.Connected)
            {
                tcpServer.Shutdown(SocketShutdown.Both);//关闭tcpServer
                tcpServer.Close();//释放
                EndPoint point = new IPEndPoint(IPAddress.Any, 7788);
                tcpServer.Bind(point);
                Console.WriteLine("等待客户端连接");
                tcpServer.Listen(100);
                connectSocket = tcpServer.Accept();//连接客户端
                string message = "连接好了呢,亲";//定义一个字符串
                byte[] date = Encoding.UTF8.GetBytes(message);//对字符串做编码,得到一个字符串的字节数组
                connectSocket.Send(date);//向客户端发送第一个数据                           
            }
        }

        /// <summary>
        /// 向客户端发送数据
        /// </summary>
        /// <param name="connectSocket">传入一个Socket</param>
        static void SendCustomer(Socket connectSocket, Socket tcpServer, string n)
        {
            while (true)
            {
                try//捕获客户端断开异常
                {
                    Console.WriteLine("可以向客户端发送数据了哟：");
                    string customer = "向" + n + "号客户端发送了:";
                    string severMessage = Console.ReadLine();//服务端输入字符串
                    byte[] severDate = Encoding.UTF8.GetBytes(severMessage);//对字符串做编码，得到一个字符串的字节数组
                    byte[] severCustomer = Encoding.UTF8.GetBytes(customer);
                    connectSocket.Send(severCustomer);
                    connectSocket.Send(severDate);//向客户端发送数据
                }
                catch { Connect(connectSocket, tcpServer); }
            }
        }

        /// <summary>
        /// 接收顾客信息
        /// </summary>
        /// <param name="connectSocket">传入客户端连接的Socket</param>
        /// <param name="tcpServer"></param>
        static void ReciveCustomer(Socket connectSocket, Socket tcpServer, string n)
        {
            while (true)//死循环，可以一直接收客户发送的信息
            {
                try//捕获客户端断开异常
                {
                    byte[] dateCustomer = new byte[1024];//创建一个字符串数组,用来接收客户传入的数据
                    int length = connectSocket.Receive(dateCustomer);
                    string messageCustomer = Encoding.UTF8.GetString(dateCustomer, 0, length);//把字节解析为字符串
                    Console.WriteLine();
                    Console.WriteLine("亲," + n + "号客户端传入了一个数据:" + messageCustomer);//在服务端打印出客户输入的数据  
                }
                catch { Connect(connectSocket, tcpServer); }
            }
        }

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="connectSocket"></param>
        /// <param name="tcpServer"></param>
        static void Listen(Socket connectSocket, Socket tcpServer)
        {
            tcpServer.Listen(100);//参数是最大连接数
            Console.WriteLine("服务器启动，客户端1可以连接了");//服务端显示
            connectSocket = tcpServer.Accept();//暂停当前线程,直到有一个客户连接过来
        }
        static void Main(string[] args)
        {
            //创建socket1
            Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//内网,流做通信,tcp做通信协议//环境配置
            EndPoint point = new IPEndPoint(IPAddress.Any, 7788);//封装ip和端口//本机的所有IP地址//端口和IP地址绑定
            tcpServer.Bind(point);

            //开始监听1
            tcpServer.Listen(100);//参数是最大连接数
            Console.WriteLine("服务器启动，客户端1可以连接了");//服务端显示
            Socket connectSocket = tcpServer.Accept();//暂停当前线程,直到有一个客户连接过来           

            //向客户端发送一个数据,判断是否连接成功
            string message = "连接好了呢,亲";//定义一个字符串
            byte[] date = Encoding.UTF8.GetBytes(message);//对字符串做编码,得到一个字符串的字节数组
            connectSocket.Send(date);//向客户端发送第一个数据       

            //向客户端发送数据的线程
            Action<Socket, Socket, string> action1 = SendCustomer;//向客户端1发送数据            
            action1.BeginInvoke(connectSocket, tcpServer, "1", null, null);

            //接收客户端的线程
            Action<Socket, Socket, string> action3 = ReciveCustomer;//接收客户端1          
            action3.BeginInvoke(connectSocket, tcpServer, "1", null, null);
        }
    }
}
