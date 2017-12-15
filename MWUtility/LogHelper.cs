using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWUtility
{
    public class LogHelper
    {
        public static string AppStartPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static TextWriter writer = File.AppendText(AppStartPath + $"\\{DateTime.Now.ToString("yyyy-MM-dd")}.log");
        public static void Log(string logInfo, string logLevel)
        {
            Console.WriteLine($"[{logLevel}] {DateTime.Now.ToString("HH:mm:ss")} {logInfo}");
            writer.WriteLine($"[{logLevel}] {DateTime.Now.ToString("HH:mm:ss")} {logInfo}");
            writer.Flush();
        }

        static TextWriter tempWriter;
        public static void TempDebug(string msg)
        {
            if (tempWriter == null)
            {
                tempWriter = File.AppendText(AppStartPath + $"\\TempDebug.log");
            }
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} {msg}");
            tempWriter.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} {msg}");
            tempWriter.Flush();
        }

        public static void Debug(string v)
        {
            Log(v, "Debug");
        }

        public static void Error(string v)
        {
            Log(v, "Error");
        }
        public static void Error(Exception ex)
        {
            Log(ex.ToString(), "Error");
        }

        public static void Info(string v)
        {
            Log(v, "Info");
        }
    }
}
