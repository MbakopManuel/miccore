using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace miccore
{

    [HelpOption("--help | -h | -?")]
    abstract class miccoreBaseCmd{

        
        protected readonly string _template_url = "https://github.com/miccore/templates.git";
        protected readonly string _source_with_auth = "template/micro-dotnet-with-auth";
        protected readonly string _source_without_auth = "template/micro-dotnet-without-auth";
        protected readonly string _source_user_microservice = "template/user.microservice";
        protected readonly string _source_sample_microservice = "template/sample.microservice";
        protected readonly string _source_samples_services = "template/samples-services";


        protected ILogger _logger;  
        protected IConsole _console;

        protected virtual Task<int> OnExecute(CommandLineApplication app)
        {            
            return Task.FromResult(0);
        }

          protected void OnException(Exception ex)
        {
            OutputError($"\n{ex.Message}\n\n");
            _logger.LogError(ex.Message);
            _logger.LogDebug(ex, ex.Message);
        }

        protected void OutputError(string message)
        {
            _console.BackgroundColor = ConsoleColor.Red;
            _console.ForegroundColor = ConsoleColor.White;
            _console.Error.WriteLine(message);
            _console.ResetColor();
        }

        protected void OutputToConsole(string data)
        {
            _console.BackgroundColor = ConsoleColor.Black;
            _console.ForegroundColor = ConsoleColor.White;
            _console.Out.Write(data);
            _console.ResetColor();
        }

        protected void runClone(string name, string source){
           
            var process1 = Process.Start("git", $"clone -b {source} {_template_url} {name} ");
            process1.WaitForExit();
            
            OutputToConsole($" \n******************************************************************************************** \n");
            OutputToConsole($" building of the solution\n");
            OutputToConsole($" \n******************************************************************************************** \n");

            Directory.SetCurrentDirectory($"./{name}/");
            process1 = Process.Start("dotnet", "build");
            process1.WaitForExit();


            OutputToConsole($" \n******************************************************************************************** \n");
            OutputToConsole($" git initialization \n");
            OutputToConsole($" \n******************************************************************************************** \n\n");

            deleteFolder("./.git/");
            // var directory = new DirectoryInfo("./.git/") { Attributes = FileAttributes.Normal };

            // foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            // {
            //     info.Attributes = FileAttributes.Normal;
            // }

            // directory.Delete(true);

            process1 = Process.Start("git", "init");
            process1.WaitForExit();

            process1 = Process.Start("git", "branch -m master main");
            process1.WaitForExit();
            
        }

        protected void runOnlyClone(string name, string source){
           
            var process1 = Process.Start("git", $"clone -b {source} {_template_url} {name} ");
            process1.WaitForExit();
            
        }

        protected void deleteFolder(string path){

            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);

        }

        protected void setNormalFolder(string path){

            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

        }

        
        

    }
    
}