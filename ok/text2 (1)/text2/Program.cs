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
using System.Windows.Forms;

namespace 连接数据库01
{
    class Program
    {
        public static SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
        public static string connstring = "Data Source=.;Initial Catalog=sqlled;Integrated Security=True";
        public static SqlConnection conn = new SqlConnection(connstring);//建立一个数据库连接
        public static SqlConnection conm = new SqlConnection(connstring);//再建立一个数据库连接
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

        /// <summary>
        /// 发送选择的灯的模式1/2/3
        /// </summary>
        /// <param name="clienSocket"></param>
        public static void TcpConnect(Socket clienSocket)
        {
            while (true)
            {
                conm.Open();//打开数据库
                SqlCommand cmd = conm.CreateCommand();
                //创建查询语句
                cmd.CommandText = "SELECT * FROM led WHERE LEDNAME=1;";
                //从数据库中读取数据流存入reader中
                SqlDataReader reader = cmd.ExecuteReader();
                //从reader中读取下一行数据,如果没有数据,reader.Read()返回flase
                while (reader.Read())
                {
                    try
                    {
                        string choice = reader.GetString(reader.GetOrdinal("choice"));
                        //格式输出数据
                        if (choice == "1" || choice == "2" || choice == "3")
                        {
                            byte[] data1 = Encoding.UTF8.GetBytes(choice);//对字符串做编码，得到一个字符串的字节数组
                            clienSocket.Send(data1);
                            Console.WriteLine("向硬件发送了一个数据:" + choice);
                            string sqlStr = "update led set choice=0 where ledname=1;";//发送数据后重新覆盖choice的值                   
                            ActionSql(sqlStr);//执行sql语句
                        }
                    }
                    catch { break; }
                }
                conm.Close();//关闭数据库
            }
        }
        static void Main(string[] args)
        {

            //1.创建socket
            Socket tcpSever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint point = new IPEndPoint(IPAddress.Any, 7788);//IPEndPoint是对ip端口号做了一个封装的类
            tcpSever.Bind(point);//向操作系统申请一个可用的端口号做通信

            tcpSever.Listen(1000);   ///参数是最大连接数
            Console.WriteLine("等待硬件小可爱连接。。。。。");
            Socket clienSocket = tcpSever.Accept();///暂停当前线程，直到有一个客户端连接进来，之后进行下面的代码
            Console.WriteLine("硬件小可爱连接过来了");

            ////发送网页选择的节假日模式1/2/3
            //Action<Socket> action = TcpConnect;
            //action.BeginInvoke(clienSocket, null, null);

            //接收故障灯的编号,并对数据库进行操作
            while (true)
            {
                try
                {
                    byte[] data2 = new byte[1024];//创建一个字节数组用来做容器，去承接客户端发送过来的数据
                    int length = clienSocket.Receive(data2);
                    if (length != 0)
                    {
                        string message2 = Encoding.UTF8.GetString(data2, 0, length);
                        Console.WriteLine("接收到了一个硬件的数据：" + message2);
                        //连接数据库
                        ConnectSql();
                        //创建要执行的SQL语句
                        string sqlStr = "update led set ledstat=0 where ledname=" + message2 + ";";
                        //执行sql语句
                        ActionSql(sqlStr);
                        //关闭数据库
                        conn.Close();
                        Console.WriteLine("关闭数据库");
                        Console.WriteLine("-------------我是分割线-------------");
                    }
                }
                catch { break; }
            }
            Application.Exit();
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}