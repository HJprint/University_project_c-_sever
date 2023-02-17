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
        public static SqlConnection cola = new SqlConnection(connstring);//再建立一个数据库连接
        static void Main(string[] args)
        {
            //string message = "116.654357,39.999001";
            //Console.WriteLine(message.Substring(message.Length - 9, 9));
            //Console.ReadKey();
            //发送灯的地址
            cola.Open();//打开数据库
            SqlCommand cmm = cola.CreateCommand();
            //创建查询语句
            cmm.CommandText = "SELECT * FROM led WHERE ledname=1;";
            //从数据库中读取数据流存入reader中
            SqlDataReader reader2 = cmm.ExecuteReader();
            string sum = null;
            while (reader2.Read())//逐个读取数据库中的内容
            {
                string location = reader2.GetString(reader2.GetOrdinal("location"));
            }
            cola.Close();//关闭数据库
            Console.WriteLine("向网页发送了灯的地址:" + sum);
            Console.ReadKey();
        }
    }
}