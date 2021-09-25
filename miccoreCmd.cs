using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading.Tasks;

namespace miccore
{
    [Command(Name = "miccore", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase )]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand( typeof(newCmd),
                 typeof(addCmd),
                 typeof(deleteCmd),
                 typeof(buildCmd),
                 typeof(migrateCmd)
                )]
    class miccoreCmd : miccoreBaseCmd {

        public miccoreCmd(ILogger<miccoreCmd> logger, IConsole console)
        {
            _logger = logger;
            _console = console;
        }     

         protected override Task<int> OnExecute(CommandLineApplication app)
        {
            // this shows help even if the --help option isn't specified
            app.ShowHelp();            
            return Task.FromResult(0);
        }

        private static string GetVersion()
            => typeof(miccoreCmd).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}