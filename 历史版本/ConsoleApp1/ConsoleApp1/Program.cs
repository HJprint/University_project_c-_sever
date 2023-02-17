using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        /// <summary>
        /// 向服务器发送数据
        /// </summary>
        /// <param name="tcpClient1">传入一个Socket</param>
        static void SendMessage(Socket tcpClient1)
        {
            while (true)
            {
                
                Console.Write("输入数据：");
                string message2 = Console.ReadLine();
                tcpClient1.Send(Encoding.UTF8.GetBytes(message2));//吧字符串转换成数组吗，然后发送到服务端  
            }
        }
        static void Main(string[] args)
        {


            //创建一个Socket
            Socket tcpClient1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//配置环境
            //IPAddress iPAddress = IPAddress.Parse("118.31.54.195");//服务器
            IPAddress iPAddress = IPAddress.Parse("10.32.2.77");
            EndPoint point = new IPEndPoint(iPAddress, 8877);//绑定服务器的IP地址和端口号
            tcpClient1.Connect(point);

            //向服务器发送数据的线程
            Action<Socket> action = SendMessage;
            action.BeginInvoke(tcpClient1, null, null);

            //接收服务端的数据
            while (true)
            {
                byte[] data = new byte[1024];
                int length = tcpClient1.Receive(data);
                string message = Encoding.UTF8.GetString(data, 0, length);//只把接收到的数据做一个转化
                Console.WriteLine("从服务端接收了一个数据\n" + message);
            }
        }
    }
}
