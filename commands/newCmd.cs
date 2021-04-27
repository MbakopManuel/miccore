using System;
using System.Diagnostics;
using System.IO;
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

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            try
            {
                if(string.IsNullOrEmpty(_name)){
                    _name = "Miccore WebApi";
                }

                if(_auth){
                    OutputToConsole($"project with authentication with name {_name}\n\n");
                    // Process.Start("git", $"clone https://github.com/miccore/Micro-dotnet.git {_name} ");
                    // Process.Start("cd", _name);
                    // Process.Start("pwd");
                    // Process.Start("dotnet", "build");
                }else{
                    OutputToConsole($"project without authentication with name {_name}\n\n");
                }

                Task.Delay(500).Wait();
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