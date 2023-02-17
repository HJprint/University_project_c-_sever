using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using Microsoft.Win32.SafeHandles;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace 连接数据库01
{
    class Program
    {
        public static SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
        public static string connstring = "Data Source=.;Initial Catalog=sqlled;Integrated Security=True";
        public static SqlConnection conn = new SqlConnection(connstring);//建立一个数据库连接
        /// <summary>
        /// 连接数据库
        /// </summary>
        public static void ConnectSql()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                Console.WriteLine("sqlled数据库,连接打开成功");
            }
            else
                Console.WriteLine("sqlled数据库,连接打开失败");
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sqlStr">传输sql语句</param>
        public static void ActionSql(string sqlStr)
        {
            SqlCommand cmd = new SqlCommand(); cmd.Connection = conn;
            cmd.CommandText = sqlStr; cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            Console.WriteLine("已经修改数据库");
        }
       
        static void Main(string[] args)
        {

            //1.创建socket
            Socket tcpSever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint point = new IPEndPoint(IPAddress.Any, 8877);//IPEndPoint是对ip端口号做了一个封装的类
            tcpSever.Bind(point);//向操作系统申请一个可用的端口号做通信

            tcpSever.Listen(1000);   ///参数是最大连接数
            Console.WriteLine("等待GPS小可爱连接。。。。。");
            Socket clienSocket = tcpSever.Accept();///暂停当前线程，直到有一个客户端连接进来，之后进行下面的代码
            Console.WriteLine("GPS小可爱连接过来了");

            //接收GPS信息,并对数据库进行操作
            while (true)
            {
                byte[] data2 = new byte[1024];//创建一个字节数组用来做容器，去承接客户端发送过来的数据
                int length = clienSocket.Receive(data2);
                if (length != 0)
                {
                    string message2 = Encoding.UTF8.GetString(data2, 0, length);
                    Console.WriteLine("接收到了一个GPS的数据：" + message2);
                    //连接数据库
                    ConnectSql();
                    //创建要执行的SQL语句
                    string sqlStr = "update led set location="+ message2.Substring(0, 10) + " where ledname=1;";//获取经度
                    //执行sql语句
                    ActionSql(sqlStr);
                    string sqlStr2 = "update led set location=" + message2.Substring(message2.Length - 9, 9) + " where ledname=2;";//获取维度
                    //执行sql语句
                    ActionSql(sqlStr2);
                    //关闭数据库
                    conn.Close();
                    Console.WriteLine("关闭数据库");
                    Console.WriteLine("-------------我是分割线-------------");
                }
            }
        }
    }
}