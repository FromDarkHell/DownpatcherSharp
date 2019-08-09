using System;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;

namespace DownpatcherSharp
{
    public class Patch
    {
        /// <summary>
        /// An initializer for the 'Patch' type.
        /// </summary>
        /// <param name="game">The game object</param>
        /// <param name="versionNumber">The version number/name of the patch</param>
        public Patch(Game game, string versionNumber)
        {
            this.game = game;
            this.versionNumber = versionNumber;
            patchDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Patch Data\\" + versionNumber);
        }
        /// <summary>
        /// An initializer for the 'Patch' type.
        /// </summary>
        /// <param name="game">The game object</param>
        /// <param name="versionNumber">The version number/name of the patch</param>
        /// <param name="folderName">The folder name of the patch in the "Patch Data" </param>
        public Patch(Game game, string versionNumber, string folderName)
        {
            this.game = game;
            this.versionNumber = versionNumber;
            patchDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Patch Data\\" + folderName);
        }

        #region Properties
        private Game game { get; set; }
        /// <summary>
        /// The DirectoryInfo object of the current patch
        /// </summary>
        public DirectoryInfo patchDirectory { get; set; }
        /// <summary>
        /// The version number associated with the current patch, used to display data (or in some cases, the folder name of the patch folder)
        /// </summary>
        public string versionNumber { get; set; }
        #endregion
        /// <summary>
        /// A function that patches the game to the current Patch object.
        /// </summary>
        public void PatchGame()
        {
            try
            {
                foreach (FileInfo file in patchDirectory.GetFiles())
                {
                    string[] fileNames = null;
                    bool deleteFiles = file.Name.Equals("filesToDelete.patch");
                    bool addFiles = file.Name.Equals("filesToAdd.patch");
                    if (deleteFiles || addFiles) fileNames = File.ReadAllLines(file.FullName);
                    else fileNames = new string[] { file.Name };
                    foreach (FileInfo gameFile in game.gameDir.GetFiles())
                    {
                        foreach (string fileName in fileNames)
                        {
                            if (deleteFiles && Regex.IsMatch(gameFile.Name, fileName, RegexOptions.Compiled)) gameFile.Delete();
                            else if (!deleteFiles && gameFile.Name.Equals(fileName)) File.Copy(file.FullName, gameFile.FullName, true);
                            else if (addFiles)
                            {
                                foreach (FileInfo patchFile in patchDirectory.GetFiles())
                                {
                                    if (Regex.IsMatch(patchFile.Name, fileName, RegexOptions.Compiled))
                                        File.Copy(patchFile.FullName, gameFile.FullName.Replace(gameFile.Name, "") + patchFile.Name, true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.writeErrorToDisk(game.gameName, ex);
            }
        }

    }
}
