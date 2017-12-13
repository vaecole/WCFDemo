using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyTest
{
    class LogHelper
    {
        static LogHelper()
        {

        }
        public static string AppStartPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static TextWriter writer = File.AppendText(AppStartPath + "\\myLog.log");
        public static void Log(string logInfo)
        {
            Console.WriteLine(DateTime.Now.ToString() + " " + logInfo);
            writer.WriteLine(DateTime.Now.ToString() + " " + logInfo);
            writer.Flush();
        }

        public static void Log(Exception ex)
        {
            writer.WriteLine("==================================================");
            writer.WriteLine(DateTime.Now.ToString() + " " + ex.Message);
            writer.WriteLine(DateTime.Now.ToString() + " " + ex.InnerException);
            writer.WriteLine(DateTime.Now.ToString() + " " + ex.StackTrace);
            writer.WriteLine("==================================================");
        }

        public static void Report(string text)
        {
            writer.WriteLine(text);
        }

        internal static void LogDebug(string v)
        {
            Log(v);
        }

        internal static void LogError(string v)
        {
            Log(v);
        }
        internal static void LogError(Exception ex)
        {
            Log(ex.ToString());
        }

        internal static void LogInfo(string v)
        {
            Log(v);
        }
    }
}
