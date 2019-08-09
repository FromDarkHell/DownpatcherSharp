using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownpatcherSharp
{
    public static class Helpers
    {
        /// <summary>
        /// A function that writes the given error to disk at 'C:\{gameName}Patcher.log'.
        /// </summary>
        /// <param name="gameName">The name of the game</param>
        /// <param name="ex">The exception to write to disk</param>
        public static void writeErrorToDisk(string gameName, Exception ex)
        {
            using (StreamWriter errorWriter = new StreamWriter(string.Format(@"C:\{0}Patcher.log", gameName), false))
            {
                errorWriter.WriteLine("-----------------------------------------------------------------------------");
                errorWriter.WriteLine("Date : " + DateTime.Now.ToString());
                errorWriter.WriteLine();

                while (ex != null)
                {
                    errorWriter.WriteLine(ex.GetType().FullName);
                    errorWriter.WriteLine("Message : " + ex.Message);
                    errorWriter.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
    }
}
