using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace miccore
{

    [HelpOption("--help | -h | -?")]
    abstract class miccoreBaseCmd{

        protected readonly string _clean_sample = "https://github.com/miccore/clean-architecture-sample.git";
        protected readonly string _clean_auth = "https://github.com/miccore/clean-architecture-auth.git";
        protected readonly string _clean_project = "https://github.com/miccore/clean-architecture-project.git";
        protected readonly string _clean_items = "https://github.com/miccore/clean-architecture-items.git";
       
        protected ILogger _logger;  
        protected IConsole _console;

        protected virtual Task<int> OnExecute(CommandLineApplication app)
        {            
            return Task.FromResult(0);
        }

        /// <summary>
        /// output exception
        /// </summary>
        /// <param name="ex"></param>
        protected void OnException(Exception ex)
        {
            // Console.WriteLine(ex.Message);
            _logger.LogError(ex.Message);
            _logger.LogTrace(ex, ex.Message);
        }

        /// <summary>
        /// output error
        /// </summary>
        /// <param name="message"></param>
        protected void OutputError(string message)
        {
            // Console.WriteLine(message);
            _logger.LogError(message);
        }

       /// <summary>
       /// output data to console
       /// </summary>
       /// <param name="data"></param>
        protected void OutputToConsole(string data)
        {
            // Console.WriteLine(data);
           _logger.LogInformation(data);
        }

        /// <summary>
        /// run a clone of project, build and reinitialize the git remote
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
        protected void runClone(string name, string source){
           
            var process1 = new Process();
            process1.StartInfo.FileName = "git";
            process1.StartInfo.Arguments = $"clone {source} {name}";
            process1.StartInfo.RedirectStandardError = true;
            process1.StartInfo.RedirectStandardOutput = false;
            process1.StartInfo.CreateNoWindow = true;
            process1.StartInfo.UseShellExecute = false;
            process1.Start();
            process1.WaitForExit();
            if (process1.ExitCode != 0)
            {   
                OutputError(process1.StandardError.ReadToEnd());
                throw new Exception(process1.StandardError.ReadToEnd());
            }
            
            OutputToConsole($"building of the solution");

            Directory.SetCurrentDirectory($"./{name}/");

            OutputToConsole($"git initialization");
            deleteFolder("./.git/");

            process1.StartInfo.FileName = "git";
            process1.StartInfo.Arguments = $"init";
            process1.Start();
            process1.WaitForExit();
            if (process1.ExitCode != 0)
            {
                OutputError(process1.StandardError.ReadToEnd());
                throw new Exception(process1.StandardError.ReadToEnd());
            }

            process1.StartInfo.FileName = "git";
            process1.StartInfo.Arguments = $"branch -m master main";
            process1.Start();
            process1.WaitForExit();
            if (process1.ExitCode != 0)
            {   
                OutputError(process1.StandardError.ReadToEnd());
                throw new Exception(process1.StandardError.ReadToEnd());
            }
            
            
        }

        /// <summary>
        /// run a clone of project, build and reinitialise git repository
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
         protected void runCloneProject(string name, string source){
            var process1 = new Process();
            process1.StartInfo.FileName = "git";
            process1.StartInfo.Arguments = $"clone {source} {name}";
            process1.StartInfo.RedirectStandardError = true;
            process1.StartInfo.RedirectStandardOutput = false;
            process1.StartInfo.CreateNoWindow = true;
            process1.StartInfo.UseShellExecute = false;
            process1.Start();
            process1.WaitForExit();
            if (process1.ExitCode != 0)
            {
                OutputError(process1.StandardError.ReadToEnd());
                throw new Exception(process1.StandardError.ReadToEnd());
            }
            
            OutputToConsole($"building of the solution");
            
            Directory.SetCurrentDirectory($"./{name}/");
            
            deleteFolder("./.git/");
            
        }


        /// <summary>
        /// run a clone of project
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
        protected void runOnlyClone(string name, string source){
            var process1 = new Process();
            process1.StartInfo.FileName = "git";
            process1.StartInfo.Arguments = $"clone {source} {name}";
            process1.StartInfo.RedirectStandardError = true;
            process1.StartInfo.RedirectStandardOutput = false;
            process1.StartInfo.CreateNoWindow = true;
            process1.StartInfo.UseShellExecute = false;
            process1.Start();
            process1.WaitForExit();
            if (process1.ExitCode != 0)
            {
                OutputError(process1.StandardError.ReadToEnd());
                throw new Exception(process1.StandardError.ReadToEnd());
            }
            
        }


        /// <summary>
        /// delete folder
        /// </summary>
        /// <param name="path"></param>
        protected void deleteFolder(string path){

            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);

        }


        /// <summary>
        /// set attribute of a folder as normal
        /// </summary>
        /// <param name="path"></param>
        protected void setNormalFolder(string path){

            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

        }


        /// <summary>
        /// copy directory to another 
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        protected void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                OutputError("Source directory does not exist or could not be found: "
                    + sourceDirName);
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            
            // If the destination directory doesn't exist, create it. 
            if(!Directory.Exists(destDirName)){
                Directory.CreateDirectory(destDirName);
            }     

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }



        }

        /// <summary>
        /// change file mod
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="permissions"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        protected bool Chmod(string filePath, string permissions = "700", bool recursive = false)
        {
            string cmd =$"chmod {permissions} {filePath}";

            if (recursive)
                cmd = $"chmod -R {permissions} {filePath}";

            try
            {
                var process1 = new Process();
                process1.StartInfo.FileName = "/bin/bas";
                process1.StartInfo.Arguments = $"-c \"{cmd}\"";
                process1.StartInfo.RedirectStandardError = true;
                process1.StartInfo.RedirectStandardOutput = false;
                process1.StartInfo.CreateNoWindow = true;
                process1.StartInfo.UseShellExecute = false;
                process1.Start();
                process1.WaitForExit();
                return process1.ExitCode == 0;
            }
            catch (Exception e)
            {
                OnException(e);
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// build project
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <param name="process"></param>
        protected void restoreProject(string ProjectName, Process process){
            OutputToConsole($"restore {ProjectName.Split('.').Last()}");
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"restore ./{ProjectName}/src/{ProjectName}.Api/{ProjectName}.Api.csproj";
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                OutputError(process.StandardError.ReadToEnd());
                throw new Exception(process.StandardError.ReadToEnd());
            }
        }

        /// <summary>
        /// restore gateway
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <param name="process"></param>
        protected void restoreGateway(string ProjectName, Process process){
            OutputToConsole($"restore {ProjectName.Split('.').Last()}");
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"restore ./{ProjectName}/{ProjectName}.csproj";
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                OutputError(process.StandardError.ReadToEnd());
                throw new Exception(process.StandardError.ReadToEnd());
            }
        }

        /// <summary>
        /// publish gateway
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <param name="process"></param>
        protected void publishGateway(string ProjectName, Process process){
            OutputToConsole($"publish {ProjectName.Split('.').Last()}");
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"publish ./{ProjectName}/{ProjectName}.csproj -c Release";
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                OutputError(process.StandardError.ReadToEnd());
                throw new Exception(process.StandardError.ReadToEnd());
            }
        }

        /// <summary>
        /// publish project
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <param name="process"></param>
        protected void publishProject(string ProjectName, Process process){
            OutputToConsole($"Publish {ProjectName.Split('.').Last()}");
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"publish ./{ProjectName}/src/{ProjectName}.Api/{ProjectName}.Api.csproj -c Release";
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                OutputError(process.StandardError.ReadToEnd());
                throw new Exception(process.StandardError.ReadToEnd());
            }
        }

        /// <summary>
        /// build docker image
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <param name="process"></param>
        protected void buildImage(string ProjectName, Process process){
            OutputToConsole($"Build {ProjectName.Split('.').Last()} Docker Image");
            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments = $"build -q ./{ProjectName} -t {ProjectName.ToLower()}.image";
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                // OutputError(process.StandardError.ReadToEnd());
                throw new Exception(process.StandardError.ReadToEnd());
            }
        }

        /// <summary>
        /// save docker image
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <param name="process"></param>
        protected void saveImage(string ProjectName, Process process){
            OutputToConsole($"Save {ProjectName.Split('.').Last()} Docker Image");
            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments = $"save --output ./dist/{ProjectName.ToLower()}.image.tar {ProjectName.ToLower()}.image";
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                // OutputError(process.StandardError.ReadToEnd());
                throw new Exception(process.StandardError.ReadToEnd());
            }
        }
    }


    
    
}