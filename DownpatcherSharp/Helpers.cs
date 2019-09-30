using System.IO;
using System;

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
            string filePath = Path.Combine(Path.GetTempPath(), string.Format("{0}Patcher.log", gameName));
            using (StreamWriter errorWriter = File.CreateText(filePath))
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
