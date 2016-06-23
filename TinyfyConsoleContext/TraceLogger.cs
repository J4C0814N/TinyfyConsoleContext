using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyfyConsoleContext
{
    public static class TraceLogger
    {
        static TextLogTraceListener textLogTraceListener = new TextLogTraceListener(AppDomain.CurrentDomain.BaseDirectory+"Application.log");

        public static void CloseWriter()
        {
            textLogTraceListener.Close();
        }

        public static void Error(string message, string module)
        {
            WriteEntry(message, "ERROR", module);
        }

        public static void Error(Exception e, string module)
        {
            WriteEntry(e.Message + Environment.NewLine + e.StackTrace, "ERROR", module);
        }
        public static void DisplayError(string message, string module)
        {
            WriteEntry(message, "ERROR", module);
            Console.WriteLine(message);
        }
        public static void DisplayError(Exception e, string module)
        {
            WriteEntry(e.Message + Environment.NewLine + e.StackTrace, "ERROR", module);
            Console.WriteLine(e.Message);
        }

        public static void Warning(string message, string module)
        {
            WriteEntry(message, "WARNING", module);
        }
        public static void DisplayWarning(string message, string module)
        {
            WriteEntry(message, "WARNING", module);
            Console.WriteLine(message);
        }


        public static void Info(string message, string module)
        {
            WriteEntry(message, "INFO", module);
        }

        public static void DisplayInfo(string message, string module)
        {
            WriteEntry(message, "INFO", module);
            Console.WriteLine(message);
        }


        private static void WriteEntry(string message, string type, string module)
        {
            textLogTraceListener.WriteLine(string.Format("[{0}] [{1}] [{2}] {3}",
                                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                  type,
                                  module,
                                  message));
        }

    }
}
