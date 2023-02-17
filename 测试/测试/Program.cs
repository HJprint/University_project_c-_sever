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

namespace 测试
{
    class Program
    {
        public static SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
        public static string connstring = "Data Source=.;Initial Catalog=sqlled;Integrated Security=True";
        public static SqlConnection conm = new SqlConnection(connstring);//再建立一个数据库连接
        static void Main(string[] args)
        {
            string stat, stat2 = null;
            int i = 1;
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
                if (i++ % 2 == 0)//每隔一次循环,对stat2赋值一次
                {
                    stat2 = stat;
                    if (i == 100) i = 1;
                }
                conm.Close();//关闭数据库
                if (stat2 != stat)//当两次查询灯的状态不一样时,向网页传输字符串
                {
                    Console.WriteLine("向网页发送了灯的状态:" + stat);
                }

            }
        }
    }
}
