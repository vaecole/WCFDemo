using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Configuration;

namespace mw.Image2Url
{
    class Program
    {
        static void Main(string[] args)
        {
            Image2Url("ruleTemplateGroup");
            Image2Url("ruleTemplate");
        }

        private static void Image2Url(string entityName)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            SqlCommand com = null;
            try
            {
                //构造数据库连接字符串，使用IP地址，数据库名称，用户名，密码替换XXX
                string connStr = ConfigurationManager.ConnectionStrings["OctopusSQLServer"].ConnectionString;
                conn = new SqlConnection(connStr);
                conn.Open();

                //编写SQL语句
                string sqlText = $"select Id, Icon, Image from {entityName}";
                com = new SqlCommand(sqlText, conn);
                dr = com.ExecuteReader();
                var folderPath = $".\\{entityName}Icon\\";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                while (dr.Read())
                {
                    var id = int.Parse(dr["Id"].ToString());
                    var picBytes = (byte[])dr["Icon"];
                    var url = dr["Image"]?.ToString();
                    url = D3PictureServiceWrapper.UploadPicture(picBytes, $"bazhuayu/{entityName}/{id}");
                    var upateSql = $"update {entityName} set Image = '{url}' where Id={id}";
                    var updateCmd = new SqlCommand(upateSql, conn);
                    Console.WriteLine(id.ToString() + url);
                    updateCmd.ExecuteNonQuery();
                    using (BinaryWriter sw = new BinaryWriter(File.Create(Path.Combine(folderPath, $"{id}.png"))))
                    {
                        sw.Write(picBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
            finally
            {
                if (dr != null)
                {
                    //关闭SqlDataReader
                    dr.Close();
                }
                if (conn != null)
                {
                    //关闭数据库
                    conn.Close();
                }
            }
        }
    }
}
