using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace miccore.Utility{
    class RenameUtility{

        public RenameUtility(){

        }

        public void Rename(string projectPath, string oldName,string newName)
        {
        
            // var ignoreFile = projectPath + @"\IgnoreExtensions.RenameUtility";
            var ignoreExts = new List<string>();

            // if (!string.IsNullOrEmpty(ignoreFile))
            // {
            //     try
            //     {
            //         var ignoreFileText = File.ReadAllText(ignoreFile).Split(",");
            //         ignoreExts.AddRange(ignoreFileText);
            //     }
            //     catch
            //     {
            //         Console.WriteLine($"ERROR - Failed to read IgnoreExtensions.RenameUtility file.");
            //     }
            // }

            Renamer(projectPath, oldName, newName, ignoreExts);
        }

        
        protected void Renamer(string source, string search, string replace, ICollection<string> ignoreExts)
        {
            var files = Directory.GetFiles(source);

            foreach (var filePath in files)
            {
                ReplaceFileText(search, replace, filePath, ignoreExts);

                var fileIdx = filePath.LastIndexOf('\\');

                if (fileIdx == -1) // is Linux machine
                {
                    fileIdx = filePath.LastIndexOf('/');
                }

                var fileName = filePath.Substring(fileIdx);
                var ext = filePath.Split(".").Last();

                if (ignoreExts.Contains(ext) || !fileName.Contains(search)) continue;

                ReplaceFileName(search, replace, fileName, filePath, fileIdx);
            }

            var subdirectories = Directory.GetDirectories(source);
            foreach (var subdirectory in subdirectories)
            {
                Renamer(subdirectory, search, replace, ignoreExts);

                var folderNameIdx = subdirectory.LastIndexOf('\\') + 1;

                if (folderNameIdx == -1) // is Linux machine
                {
                    folderNameIdx = subdirectory.LastIndexOf('/') + 1;
                }

                var folderName = subdirectory.Substring(folderNameIdx);

                if (!folderName.ToLower().Contains(search.ToLower())) continue;

                ReplaceFolderName(search, replace, subdirectory, folderNameIdx, folderName);
            }
        }

        protected void ReplaceFolderName(string search, string replace, string subdirectory, int folderNameIdx, string folderName)
        {
            Console.WriteLine($"Replacing {search} with {replace} in folder name: {folderName}");

            var newDirectory = subdirectory.Substring(0, folderNameIdx) +
                               folderName.Replace(search, replace, StringComparison.OrdinalIgnoreCase);
            try
            {
                if (subdirectory != newDirectory)
                    Directory.Move(subdirectory, newDirectory);
            }
            catch
            {
                Console.WriteLine($"ERROR - Failed to rename folder: {subdirectory}.");
            }
        }

        protected void ReplaceFileName(string search, string replace, string filename, string filepath, int fileindex)
        {
            Console.WriteLine($"Replacing {search} with {replace} in file name: {filepath}");

            var startIndex = filename.IndexOf(search, StringComparison.OrdinalIgnoreCase);
            var endIndex = startIndex + search.Length;
            var newName = filename.Substring(0, startIndex);
            newName += replace;
            newName += filename.Substring(endIndex);

            var fileAddress = filepath.Substring(0, fileindex);
            fileAddress += newName;

            try
            {
                File.Move(filepath, fileAddress);
            }
            catch
            {
                Console.WriteLine($"ERROR - Failed to rename file: {filepath}.");
            }
        }

        protected static void ReplaceFileText(string search, string replace, string filepath, ICollection<string> ignoreExts)
        {
            var text = File.ReadAllText(filepath);
            var ext = filepath.Split(".").Last();
            if (ignoreExts.Contains(ext) || !text.Contains(search)) return;

            Console.WriteLine($"Replacing {search} with {replace} in file: {filepath}");

            text = text.Replace(search, replace);
            try
            {
                File.WriteAllText(filepath, text);
            }
            catch
            {
                Console.WriteLine($"ERROR - Failed to replace text in file: {filepath}.");
            }
        }

       

    }
}