using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
using System.IO;

namespace DownpatcherSharp
{
    /// <summary>
    /// An abstract class for the game you're wanting to patch.
    /// If you want to patch [x] game, create a new class that inherits this class and override getCurrentPatch().
    /// </summary>
    public abstract class Game
    {
        #region Properties
        public List<Patch> patches = new List<Patch>();
        public DirectoryInfo gameDir;
        public string currentVersion;
        public string gameName;

        #endregion

        #region Constructors
        public Game(string filePath, string gameName)
        {
            this.gameName = gameName;
            setCurrentFilePath(filePath);
            initalizePatches();
            currentVersion = getCurrentPatch();
        }
        #endregion

        #region Functions
        private void initalizePatches()
        {
            DirectoryInfo basePatchDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Patch Data\\");
            foreach (DirectoryInfo patchDirectory in basePatchDirectory.GetDirectories())
            {
                string patchName = patchDirectory.Name;
                // Match: "110_to_131"
                if (Regex.IsMatch(patchName, "[0-9]*_to_[0-9]*", RegexOptions.Compiled))
                    patchName = string.Join(".", Regex.Split(patchName.Replace("_", " "), "[0-9]", RegexOptions.Compiled));
                // Match: "1.1.0_to_1.3.1"
                else if (Regex.IsMatch(patchName, @"(([0-9]\.).*)_to_(([0-9]\.).*)", RegexOptions.Compiled)) patchName = patchName.Replace("_", " ");
                // We changed the patch version to something more displayable, we should make sure to specify the directory on disk.
                if (!patchName.Equals(patchDirectory.Name)) patches.Add(new Patch(this, patchName, patchDirectory.Name));
                // We didn't change anything so the patch version == directory name.
                else patches.Add(new Patch(this, patchName));
            }
        }

        /// <summary>
        /// Set the current patch given a Patch object.
        /// </summary>
        /// <param name="patch">The patch object that you want to patch the game to</param>
        public void setCurrentPatch(Patch patch)
        {
            patch.PatchGame();
        }

        /// <summary>
        /// Patch the game to a specific version.
        /// </summary>
        /// <param name="versionNumber">A string representation of the patch version you want to patch to.</param>
        public void setCurrentPatch(string versionNumber)
        {
            foreach (Patch patch in patches)
            {
                if (patch.versionNumber == versionNumber) { patch.PatchGame(); break; }
            }
        }

        /// <summary>
        /// Return all patches' version number in array form.
        /// </summary>
        /// <returns></returns>
        public string[] getPatches()
        {
            return patches.Select(o => o.versionNumber).ToArray();
        }

        /// <summary>
        /// An abstract function that gives you the current patch of the game. You'll need to override this in the inherited 'Game' class.
        /// </summary>
        /// <returns>The current patch</returns>
        public abstract string getCurrentPatch();

        #region File Path Stuff

        /// <summary>
        /// A function to set the current file path to a specific file path.
        /// </summary>
        /// <param name="filePath">The string representation of the file path of the game</param>
        public void setCurrentFilePath(string filePath)
        {
            this.gameDir = new DirectoryInfo(filePath);
            // If we couldn't write the key to the registry, it doesn't *really* matter.
            try
            {
                RegistryKey rkey = Registry.CurrentUser.CreateSubKey("DownpatcherSharp");
                rkey.SetValue(gameName, filePath);
            }
            catch { }
        }

        public string getRegistryFilePath()
        {
            try
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey("DownpatcherSharp");
                if (rkey != null)
                    return ((string)rkey.GetValue(gameName));
            }
            catch { }

            return null;
        }
        #endregion

        #endregion
    }
}
