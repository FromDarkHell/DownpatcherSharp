
using System;
using System.Linq;
using System.Text;
using DownpatcherSharp;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace DownpatcherSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestingGame game = new TestingGame(@"C:\test", "test");

        }
    }

    public class TestingGame : Game
    {
        public TestingGame(string filePath, string gameName) : base(filePath, gameName) { }

        public override string getCurrentPatch()
        {
            return FileVersionInfo.GetVersionInfo(gameDir.FullName + "\\test.exe").FileVersion;
        }
    }
}
