
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
            TestingGame game = new TestingGame(Directory.GetCurrentDirectory() + "\\testingGame", "test");
            Console.WriteLine("Current Patch: " + game.getCurrentPatch());
            string availablePatches = string.Join(", ", game.getPatches());
            Console.WriteLine("Available Patches: " + availablePatches);
            foreach (string patch in game.getPatches())
            {
                Console.WriteLine("----");
                Console.WriteLine("Patching To: " + patch);
                game.setCurrentPatch(patch);

                Console.WriteLine("Current Patch: " + game.getCurrentPatch());
            }
            Console.WriteLine("----");

            Console.WriteLine("Reading Registry Key: " + game.getRegistryFilePath().ToString());

            Console.WriteLine("Waiting...");
            Console.ReadLine();
        }
    }

    public class TestingGame : Game
    {
        public TestingGame(string filePath, string gameName) : base(filePath, gameName) { }

        public override string getCurrentPatch()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(gameDir.FullName + "\\test.exe");
            return versionInfo.ProductVersion;
        }
    }
}
