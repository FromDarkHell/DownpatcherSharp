using System;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;

namespace DownpatcherSharp
{
    public class Patch
    {
        public Patch(Game game, string versionNumber)
        {
            this.game = game;
            this.versionNumber = versionNumber;
            patchDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Patch Data\\" + versionNumber);
        }
        #region Properties
        public Game game { get; set; }
        public DirectoryInfo patchDirectory { get; set; }

        public string versionNumber { get; set; }
        #endregion

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
