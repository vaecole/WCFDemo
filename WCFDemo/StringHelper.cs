using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFDemo
{
    class StringHelper
    {
        public string Replace()
        {
            string res = null;
            string t1 = File.OpenText(tmpltPath1).ReadToEnd();
            string t2 = File.OpenText(tmpltPath2).ReadToEnd();
            StringBuilder sb = new StringBuilder();
            string[] lines = str.Replace("\r\n", "|").Split('|');
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains(":"))
                {
                    sb.Append(t1.Replace("$1", lines[i].Substring(lines[i].IndexOf(":") + 1)));
                    StringBuilder sbInner = new StringBuilder();
                    i++;
                    for (; i < lines.Count() && !lines[i].Contains(":"); i++)
                    {
                        sbInner.Append(t2.Replace("$2", lines[i]));
                    }
                    sb.Replace("$inner", sbInner.ToString());
                    i--;
                }
            }

            res = sb.ToString();
            return res;
        }

        public string tmpltPath1 = @"C:\Users\marko\Documents\Visual Studio 2017\Projects\WCFDemo\WCFDemo\bin\Debug\1.txt";
        public string tmpltPath2 = @"C:\Users\marko\Documents\Visual Studio 2017\Projects\WCFDemo\WCFDemo\bin\Debug\2.txt";
        public string str = @"1:软件介绍
八爪鱼介绍
版本选择攻略
2:新手入门
模式介绍
自定义模式入门操作
3:基础教程
规则排错教程
云采集
AJAX
登录
工具箱
功能点说明
4:进阶教程
验证码识别
XPath
特殊翻页
数据导出
5:流程步骤设置详解
提取数据
循环
判断条件
鼠标移动到此元素上
6:实战教程
";
    }
}
