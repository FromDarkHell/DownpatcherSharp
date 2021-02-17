using System.Linq;
using System.IO;
using System;

namespace PatchCreator
{
    class Program
    {
        // https://stackoverflow.com/questions/7931304/comparing-two-files-in-c-sharp
        private static bool FileCompare(string file1, string file2) {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2) {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length) {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        static void Main(string[] args)
        {
            //string oldPath = args[0];
            //string newPath = args[1];

            string newPath = @"G:\Oblivion Downpatcher\Oblivion - 1.1.425 (Beta)";
            string oldPath = @"J:\Oblivion Patcher Files\Oblivion";

            DirectoryInfo oldDir = new DirectoryInfo(oldPath);
            DirectoryInfo newDir = new DirectoryInfo(newPath);

            FileInfo[] oldFiles = oldDir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            FileInfo[] newFiles = newDir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            string exclusiveToOld = "";
            string exclusiveToNew = "";
            string similarToBoth = "";
            string differingFiles = "";

            Console.WriteLine("Checking Files...");

            foreach (FileInfo oldFile in oldFiles)
            {
                Console.WriteLine("Checking " + oldFile.Name);
                foreach (FileInfo newFile in newDir.GetFiles(oldFile.Name))
                {
                    if (oldFile.Name.Equals(newFile.Name))
                    {
                        bool compare = FileCompare(oldFile.FullName, newFile.FullName);
                        if(!compare) {
                            differingFiles += (oldFile.Name) + ", ";
                        }
                    }
                }
                Console.Clear();
            }
            oldFiles.Except(newFiles).ToList().ForEach(b => exclusiveToOld += (b + ", "));
            newFiles.Except(oldFiles).ToList().ForEach(b => exclusiveToNew += (b + ", "));

            Console.WriteLine("Differing: " + differingFiles);
            Console.WriteLine("\n\n");
            Console.WriteLine("New Files (Exclusive): " + exclusiveToNew);
            Console.WriteLine("\n\n");
            Console.WriteLine("Old Files (Exclusive): " + exclusiveToOld);

            Console.ReadLine();
        }
    }
}
