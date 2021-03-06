﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFDemo
{
    public static class StringHelper
    {

        /// <summary>
        /// 超过一定字符串长度后裁切并加特定符号(默认省略号: "...")
        /// </summary>
        /// <param name="str">要裁切的字符串</param>
        /// <param name="suspensionLength">裁切长度</param>
        /// <param name="suspensionSymbol">代替多余字符的符号</param>
        /// <returns></returns>
        public static string Suspension(this string str, int suspensionLength, string suspensionSymbol = "...")
        {
#if INTER_VERSION
            suspensionLength*=2;
#endif
            return (str ?? string.Empty).Length > suspensionLength ? str.Substring(0, suspensionLength) + suspensionSymbol : str;
        }

        /// <summary>
        /// 将字符串中的换行和多余的空格替换为一个空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimInner(this string str)
        {
            if (str == null)
                return null;
            str = str.Replace("\r\n", " ");
            string[] charArray = str.Split(' ');
            StringBuilder sb = new StringBuilder();
            foreach (var item in charArray)
            {
                sb.Append(item + " ");
            }
            return sb.ToString();
        }

        public static string ToMosaicString(this string str, int mosaicPercentage = 40, char mosaicSymbol = '*')
        {
            if (mosaicPercentage <= 0)
            {
                return str;
            }
            if (mosaicPercentage >= 100)
            {
                mosaicPercentage = 50;
            }

            int startMax = 100 - mosaicPercentage;
            int mosaicStart = (startMax / 2) * str.Length / 100;
            string resStr = str.Substring(0, mosaicStart);

            int mosaicLength = mosaicPercentage * str.Length / 100;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mosaicLength; i++)
            {
                sb.Append(mosaicSymbol);
            }

            resStr += sb.ToString();

            resStr += str.Substring(mosaicStart + mosaicLength);
            return resStr;
        }

        public static string Replace()
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

        public static string tmpltPath1 = @"C:\Users\marko\Documents\Visual Studio 2017\Projects\WCFDemo\WCFDemo\bin\Debug\1.txt";
        public static string tmpltPath2 = @"C:\Users\marko\Documents\Visual Studio 2017\Projects\WCFDemo\WCFDemo\bin\Debug\2.txt";
        public static string str = @"1:软件介绍
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
