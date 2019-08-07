using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            //string oldPath = args[0];
            //string newPath = args[1];

            string newPath = @"G:\Wolfenstein Youngblood";
            string oldPath = @"G:\Wolfenstein Youngblood - Copy";

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
                        if (oldFile.Length != newFile.Length) differingFiles += (oldFile.Name + ", ");
                        else
                        {
                            FileStream oldFileStream = oldFile.OpenRead();
                            FileStream newFileStream = newFile.OpenRead();
                            int[] original = new int[oldFileStream.Length];
                            int[] newArr = new int[oldFileStream.Length];
                            while (oldFileStream.Position != oldFileStream.Length)
                            {
                                original[oldFileStream.Position] = oldFileStream.ReadByte();
                                newArr[newFileStream.Position] = newFileStream.ReadByte();
                                //if (oldFileStream.ReadByte() != newFileStream.ReadByte()) { differingFiles += (oldFile.Name + ", "); break; }
                            }
                            if (original.SequenceEqual(newArr))
                            {
                                differingFiles += (oldFile.Name + ", ");
                                break;
                            }
                        }
                        break;
                    }
                }
                Console.Clear();
            }
            oldFiles.Except(newFiles).ToList().ForEach(b => exclusiveToOld += (b + ", "));
            newFiles.Except(oldFiles).ToList().ForEach(b => exclusiveToNew += (b + ", "));

            Console.WriteLine("Differing: " + differingFiles);
            Console.WriteLine("New Files (Exclusive): " + exclusiveToNew);
            Console.WriteLine("Old Files (Exclusive): " + exclusiveToOld);
        }
    }
}
