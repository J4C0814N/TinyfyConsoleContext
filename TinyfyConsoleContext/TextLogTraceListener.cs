using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyfyConsoleContext
{
    class TextLogTraceListener : TextWriterTraceListener
    {
        private string logFileLocation = string.Empty;

        private StreamWriter traceWriter;

        public TextLogTraceListener(string filePath)
        {
            logFileLocation = filePath;
            bool Append = AppendLog(logFileLocation);
            traceWriter = new StreamWriter(filePath, Append);
            traceWriter.AutoFlush = true;
        }

        public override void Write(string message)
        {
            traceWriter.Write(message);
        }

        public override void WriteLine(string message)
        {
            traceWriter.WriteLine(message);
        }

        public override void Close()
        {
            traceWriter.Close();
        }

        /// <summary>
        /// Determines whether the log should be emptied or appended
        /// If the file passed is a week or older it will be overwritten
        /// </summary>
        /// <param name="filePath">Full log file location</param>
        /// <returns>True Append log, False Empty and overwrite log</returns>
        private bool AppendLog(string filePath)
        {
            DateTime Today = DateTime.Now;
            DateTime LogCreated = File.GetCreationTime(filePath);

            int DaysDifference = (Today - LogCreated).Days;

            if (DaysDifference >= 7) // magic number: 7 days = 1 Week
            {
                return false;
            }

            return true;

        }

    }
}
