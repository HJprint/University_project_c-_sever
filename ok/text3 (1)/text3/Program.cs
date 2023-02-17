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
        public static SqlConnection conm = new SqlConnection(connstring);//再建立一个数据库连接
        public static SqlConnection cola = new SqlConnection(connstring);//再建立一个数据库连接
        public static SqlConnection colo = new SqlConnection(connstring);//再建立一个数据库连接
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
        /// 获取副灯的状态
        /// </summary>
        /// <param name="clienSocket"></param>
        public static string Ledstat()
        {
            string stat, stat2 = null;
            int i = 1;
            conm.Open();//打开数据库
            SqlCommand cmd = conm.CreateCommand();
            //创建查询语句
            cmd.CommandText = "SELECT * FROM led";
            //从数据库中读取数据流存入reader中
            SqlDataReader reader = cmd.ExecuteReader();
            //从reader中读取下一行数据,如果没有数据,reader.Read()返回flase
            stat = null;//归0
            while (reader.Read())//逐个读取数据库中的内容
            {
                string ledstat = reader.GetString(reader.GetOrdinal("stat"));
                stat += ledstat;//连接灯的状态为一个字符串
            }
            if (i++ % 2 == 0)//每隔一次循环,对stat2赋值一次
            {
                stat2 = stat;
                if (i == 100) i = 1;
            }
            conm.Close();//关闭数据库
            if (stat2 != stat)//当两次查询灯的状态不一样时,返回副灯现在状态
            {
                return stat;
            }
            else { return stat2; }//返回原状态            
        }

        static void Main(string[] args)
        {

            //1.创建socket
            Socket tcpSever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint point = new IPEndPoint(IPAddress.Any, 7889);//IPEndPoint是对ip端口号做了一个封装的类
            tcpSever.Bind(point);//向操作系统申请一个可用的端口号做通信

            tcpSever.Listen(1000);   ///参数是最大连接数
            Console.WriteLine("等待网页小可爱连接。。。。。");
            Socket clienSocket = tcpSever.Accept();///暂停当前线程，直到有一个客户端连接进来，之后进行下面的代码
            Console.WriteLine("网页小可爱连接过来了");

            #region 经纬度
            ////找到灯的经度
            //cola.Open();//打开数据库
            //SqlCommand cmm = cola.CreateCommand();
            ////创建查询语句
            //cmm.CommandText = "SELECT * FROM led WHERE ledname=1;";
            ////从数据库中读取数据流存入reader中
            //SqlDataReader reader2 = cmm.ExecuteReader();
            //string location1 = null;
            //while (reader2.Read())//逐个读取数据库中的内容
            //{
            //    location1 = reader2.GetString(reader2.GetOrdinal("location"));
            //}
            //cola.Close();//关闭数据库

            ////找到灯的维度
            //colo.Open();//打开数据库
            //SqlCommand cnm = colo.CreateCommand();
            ////创建查询语句
            //cnm.CommandText = "SELECT * FROM led WHERE ledname=2;";
            ////从数据库中读取数据流存入reader中
            //SqlDataReader reader3 = cnm.ExecuteReader();
            //string location2 = null;
            //while (reader3.Read())//逐个读取数据库中的内容
            //{
            //    location2 = reader3.GetString(reader3.GetOrdinal("location"));
            //}
            //cola.Close();//关闭数据库

            ////发送经纬度
            //string location = location1 + "," + location2;
            //byte[] data1 = Encoding.UTF8.GetBytes(location);//对字符串做编码，得到一个字符串的字节数组
            //clienSocket.Send(data1);
            //Console.WriteLine("向网页发送了灯的地址:" + location);
            #endregion;
            string stat;
            while (true)
            {
                conm.Open();//打开数据库
                SqlCommand cmd = conm.CreateCommand();
                //创建查询语句
                cmd.CommandText = "SELECT * FROM led";
                //从数据库中读取数据流存入reader中
                SqlDataReader reader = cmd.ExecuteReader();
                //从reader中读取下一行数据,如果没有数据,reader.Read()返回flase
                stat = null;//归0
                while (reader.Read())//逐个读取数据库中的内容
                {
                    string ledstat = reader.GetString(reader.GetOrdinal("ledstat"));
                    stat += ledstat;//连接灯的状态为一个字符串               
                }
                byte[] data1 = Encoding.UTF8.GetBytes(stat + Ledstat());//对字符串做编码，得到一个字符串的字节数组
                clienSocket.Send(data1);
                Console.WriteLine("向网页发送了灯的状态:" + stat + Ledstat());
            }

            #region 接收网页的指令,存储在choice中
            //while (true)
            //{
            //    byte[] data2 = new byte[1024];//创建一个字节数组用来做容器，去承接客户端发送过来的数据
            //    int length = clienSocket.Receive(data2);
            //    if (length != 0)
            //    {
            //        string message2 = Encoding.UTF8.GetString(data2, 0, length);
            //        Console.WriteLine("接收到了一个网页指令：" + message2);
            //        if (message2 == "1" || message2 == "2" || message2 == "3")
            //        {
            //            //连接数据库
            //            ConnectSql();
            //            //创建要执行的SQL语句
            //            string sqlStr = "update led set choice=" + message2 + " where ledname=1;";
            //            //执行sql语句
            //            ActionSql(sqlStr);
            //            //关闭数据库
            //            conn.Close();
            //            Console.WriteLine("关闭数据库");
            //            Console.WriteLine("----------我是分割线----------");
            //        }
            //        else { Console.WriteLine("错误请求"); Console.WriteLine("----------我是分割线----------"); }
            //    }
            //}
            #endregion
        }
    }
}