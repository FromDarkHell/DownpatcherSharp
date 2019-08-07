using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownpatcherSharp
{
    public abstract class Game
    {
        #region Properties
        public List<Patch> patches;
        public string currentVersion;

        public DirectoryInfo gameDir;
        public string gameName;
        #endregion

        #region Constructors
        public Game(string filePath, string gameName)
        {
            currentVersion = getCurrentPatch();
            gameDir = new DirectoryInfo(filePath);
            initalizePatches();
        }
        #endregion

        #region Functions
        private void initalizePatches()
        {
            DirectoryInfo basePatchDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Patch Data\\");
            foreach (DirectoryInfo patchDirectory in basePatchDirectory.GetDirectories())
            {
                patches.Add(new Patch(this, patchDirectory.Name));
            }
        }

        public void setCurrentPatch(Patch patch)
        {
            patch.PatchGame();
        }

        public void setCurrentPatch(string versionNumber)
        {
            foreach (Patch patch in patches)
            {
                if (patch.versionNumber == versionNumber) { patch.PatchGame(); break; }
            }
        }

        public string[] getPatches()
        {
            return patches.Select(o => o.versionNumber).ToArray();
        }

        public abstract string getCurrentPatch();


        #endregion
    }
}
