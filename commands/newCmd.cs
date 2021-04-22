using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentFTP;
using McMaster.Extensions.CommandLineUtils;
using miccore.project;
using miccore.service;
using Microsoft.Extensions.Logging;

namespace miccore
{
    [Command(Name = "new", Description = "new Web Microservice Api architecture with ocelot gateway")]
    class newCmd : miccoreBaseCmd
    {

        [Option("--with-auth | -wa",
                Description = "create a project with authentication or not, if specified your project will be created with an authentication microservice and otherwise it will be created with a sample microservice called sample",
                ShowInHelpText = true
                )]
        public bool _auth {get;}

        [Option(" --name | -n",
                CommandOptionType.SingleValue,
                Description = "The name of the project",
                ShowInHelpText = true)]
        public string _name {get; set; }


        
        public newCmd(ILogger<newCmd> logger, IConsole console){
            _logger = logger;
            _console = console;
        }

        // PercentDone callback method.
        public void handlePercentDone(int pctDone, out bool abort)
            {
            // Application code goes here.
            abort = false;
            }

        // ProgressInfo callback method.
        public void handleProgressInfo(string name, string value)
            {
            // Application code goes here.
            }

         protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                if(string.IsNullOrEmpty(_name)){
                    _name = "Miccore WebApi";
                }

                if(_auth){
                    OutputToConsole($"project with authentication with name {_name}\n\n");
                    // Process.Start("git", $"c$lone https://github.com/miccore/Micro-dotnet.git {_name} ");
                    Process.Start("cd", _name);
                    Process.Start("pwd");
                    Process.Start("dotnet", "build");
                }else{
                    OutputToConsole($"project without authentication with name {_name}\n\n");
                    // Process.Start("mkdir", _name);

                   // create an FTP client
                    FtpClient client = new FtpClient("165.227.47.207");

                    // specify the login credentials, unless you want to use the "anonymous" user account
                    client.Credentials = new NetworkCredential("root", "c@merO0n");

                    // begin connecting to the server
                    client.Connect();

                    // get a list of files and directories in the "/htdocs" folder
                    foreach (FtpListItem item in client.GetListing("/")) {
                        
                        OutputToConsole(item.FullName);
                        // if this is a file
                        // if (item.Type == FtpFileSystemObjectType.File){
                            
                        //     // get the file size
                        //     long size = client.GetFileSize(item.FullName);
                        //     OutputToConsole(size.ToString());
                            
                        //     // calculate a hash for the file on the server side (default algorithm)
                        //     FtpHash hash = client.GetChecksum(item.FullName);
                        // }
                        
                        // // get modified date/time of the file or folder
                        // DateTime time = client.GetModifiedTime(item.FullName);
                        
                    }
                    

                    // PluginFtp.DownloadFile(client, file, Task);
                    // Task.InfoFormat("[PluginFTPS] file {0} downloaded from {1}.", file.Path, Server);
        
                    // client.Disconnect();

                    //  Chilkat.Ftp2 ftp = new Chilkat.Ftp2();

                    // Chilkat.Ftp2.PercentDone percentDone = new Chilkat.Ftp2.PercentDone(handlePercentDone);
                    // ftp.setPercentDoneCb(percentDone);

                    // Chilkat.Ftp2.ProgressInfo progressInfo = new Chilkat.Ftp2.ProgressInfo(handleProgressInfo);
                    // ftp.setProgressInfoCb(progressInfo);

                    // ftp.Hostname = "dev-metzenserver.devandco.ca";
                    // ftp.Username = "root";
                    // ftp.Password = "c@merO0n";

                    // // Connect and login to the FTP server.
                    // bool success = ftp.Connect();
                    // if (success != true) {
                    //     OutputToConsole("connexion bug\n\n");
                    //     OutputToConsole(ftp.LastErrorText);
                    //     return 0;
                    // }

                    // success = ftp.LoginAfterConnectOnly();
                    // if (success != true) {
                    //     OutputToConsole("login bug\n\n");
                    //         OutputToConsole(ftp.LastErrorText);
                    //     return 0;
                    // }

                    // success = ftp.ChangeRemoteDir("/var/www/metzen/");
                    // if (success != true) {
                    //     OutputToConsole("change directory bug\n\n");
                    //     OutputToConsole(ftp.LastErrorText);
                    //     return 0;
                    // }
                    
                    // string localFilename = _name;
                    // string remoteFilename = "server-api";

                    // // Ensure that we get PercentDone callbacks.
                    // ftp.AutoGetSizeForProgress = true;

                    // // Download the file.
                    // success = ftp.GetFile(remoteFilename, localFilename);
                    // if (success != true) {
                    //     OutputToConsole("get file bug\n\n");
                    //     OutputToConsole(ftp.LastErrorText);
                    //     return 0;
                    // }

                    // success = ftp.Disconnect();

                    //     OutputToConsole("File Downloaded!");
                }

                var http = new HttpClient();
                await http.GetAsync("http://google.com");
                
                return 0;
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }
        

    }
}