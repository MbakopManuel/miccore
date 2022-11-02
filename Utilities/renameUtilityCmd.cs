using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Serilog;

namespace miccore.Utility{
    static class RenameUtility{


        /// <summary>
        /// rename recursively in folder
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public static void Rename(string projectPath, string oldName,string newName)
        {
        
            var ignoreExts = new List<string>();

            Renamer(projectPath, oldName, newName, ignoreExts);
        }

        /// <summary>
        /// renamer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <param name="ignoreExts"></param>
        public static void Renamer(string source, string search, string replace, ICollection<string> ignoreExts)
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

        /// <summary>
        /// replace folder name
        /// </summary>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <param name="subdirectory"></param>
        /// <param name="folderNameIdx"></param>
        /// <param name="folderName"></param>
        public static void ReplaceFolderName(string search, string replace, string subdirectory, int folderNameIdx, string folderName)
        {
            Log.Information($"Replacing {search} with {replace} in folder name: {folderName}");

            var newDirectory = subdirectory.Substring(0, folderNameIdx) +
                               folderName.Replace(search, replace, StringComparison.OrdinalIgnoreCase);
            try
            {
                if (subdirectory != newDirectory)
                    Directory.Move(subdirectory, newDirectory);
            }
            catch
            {
                Log.Error($"ERROR - Failed to rename folder: {subdirectory}.");
            }
        }
        
        /// <summary>
        /// replace file name
        /// </summary>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <param name="filename"></param>
        /// <param name="filepath"></param>
        /// <param name="fileindex"></param>
        public static void ReplaceFileName(string search, string replace, string filename, string filepath, int fileindex)
        {
            Log.Information($"Replacing {search} with {replace} in file name: {filepath}");

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
                Log.Error($"ERROR - Failed to rename file: {filepath}.");
            }
        }

        /// <summary>
        /// replace file text
        /// </summary>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <param name="filepath"></param>
        /// <param name="ignoreExts"></param>
        public static void ReplaceFileText(string search, string replace, string filepath, ICollection<string> ignoreExts)
        {
            var text = File.ReadAllText(filepath);
            var ext = filepath.Split(".").Last();
            if (ignoreExts.Contains(ext) || !text.Contains(search)) return;

            Log.Information($"Replacing {search} with {replace} in file: {filepath}");

            text = text.Replace(search, replace);
            try
            {
                File.WriteAllText(filepath, text);
            }
            catch
            {
                Log.Error($"ERROR - Failed to replace text in file: {filepath}.");
            }
        }

       

    }
}