using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Collections.Generic;

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
            Subpatches = new List<Patch>();
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
            Subpatches = new List<Patch>();
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

        /// <summary>
        /// A list that contains all of the other Patch objects that should be applied in order to apply this patch.
        /// </summary>
        private List<Patch> Subpatches { get; set; }
        #endregion
        
        /// <summary>
        /// A function that patches the game to the current Patch object.
        /// </summary>
        public void PatchGame()
        {
            foreach(Patch subpatch in Subpatches) { // Apply all of the subpatches of this current patch
                try { 
                    subpatch.PatchGame(); // Apply it
                }
                catch(Exception ex) { 
                    Helpers.writeErrorToDisk(game.gameName, ex); // Log if error
                }
            }

            try
            {
                foreach (FileInfo file in patchDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    string[] fileNames = null;
                    bool deleteFiles = file.Name.Equals("filesToDelete.patch");
                    bool addFiles = file.Name.Equals("filesToAdd.patch");
                    if (deleteFiles || addFiles) fileNames = File.ReadAllLines(file.FullName);
                    else fileNames = new string[] { file.FullName };

                    if (addFiles) { // Adding files that weren't there in the previous patch
                        foreach (string fileName in fileNames) {
                            foreach (FileInfo patchFile in patchDirectory.GetFiles("*.*", SearchOption.AllDirectories)) {
                                if (Regex.IsMatch(patchFile.Name, fileName, RegexOptions.Compiled)) {
                                    string outFile = game.gameDir.FullName + patchFile.FullName.Replace(patchDirectory.FullName, "");
                                    Directory.CreateDirectory(Path.GetDirectoryName(outFile)); // Make directory if doesn't exist
                                    File.Copy(patchFile.FullName, outFile, true);
                                }
                            }
                        }
                        continue;
                    }

                    foreach (FileInfo gameFile in game.gameDir.EnumerateFiles("*.*", SearchOption.AllDirectories))
                    {
                        foreach (string fileName in fileNames)
                        {
                            if (deleteFiles && Regex.IsMatch(gameFile.Name, fileName, RegexOptions.Compiled)) { // Deleting files
                                gameFile.Delete();
                            }
                            else if (!deleteFiles) { // Replacing files
                                if (gameFile.FullName.Replace(game.gameDir.FullName, "").Equals(fileName.Replace(patchDirectory.FullName, ""))) {
                                    File.Copy(file.FullName, gameFile.FullName, true);
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

        /// <summary>
        /// A function that adds to the list of patches that should be applied in order to apply this current patch.
        /// </summary>
        /// <param name="p">The Patch object that needs to be applied for this patch to be applied</param>
        public void AddSubpatch(Patch subpatch) {
            Subpatches.Add(subpatch);
        }

        /// <summary>
        /// Removes from the list of patches that need to be applied in order to patch to this current patch
        /// </summary>
        /// <param name="sub">The Patch object that doesn't need to be applied for this patch to be applied</param>
        public void RemoveSubpatch(Patch sub) {
            Subpatches.Remove(sub);
        }

    }
}
