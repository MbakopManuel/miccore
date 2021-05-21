using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using miccore.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace miccore.Utility{
    class InjectionUtility{

        public InjectionUtility(){

        }

        
        /**
        *
        *start service file importation, injection and profile adding
        *
        */

        public void ServiceNameSpacesImportation(string filepath, string projectName, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "/* End Import */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\t//{projectName} namespaces importation",
                $"\tusing {projectName}.Repositories.{serviceName};",
                $"\tusing {projectName}.Services.{serviceName};",
                $"\tusing {projectName}.Operations.{serviceName}.MapperProfiles;",
                $"\tusing {projectName}.Services.{serviceName}.MapperProfiles;",
                $"\n",
            };
            
            pre = pre.Concat(add);
            pre = pre.Concat(post);

            try
            {
                File.WriteAllText(filepath, string.Join('\n', pre));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed to import namespaces in file: {ex.Message}.");
            }
            
        }

        public void ServiceRepositoryServicesInjection(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "            /** End Injection */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\t\t\t\t//{serviceName} dependency injections",
                $"\t\t\t\t_services.TryAddScoped<I{serviceName}Repository, {serviceName}Repository>();",
                $"\t\t\t\t_services.TryAddTransient<I{serviceName}Service, {serviceName}Service>();",
                $"\n",
            };
            
            pre = pre.Concat(add);
            pre = pre.Concat(post);

            try
            {
                File.WriteAllText(filepath, string.Join('\n', pre));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed to inject the dependencies in file: {ex.Message}.");
            }
            
        }

        public void ServiceProfileAdding(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "                /** End Adding Profiles */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\t\t\t\t\t//{serviceName} adding profiles",
                $"\t\t\t\t\tnew {serviceName}Profile(),",
                $"\t\t\t\t\tnew {serviceName}ResponseProfile(),",
                $"\t\t\t\t\tnew Create{serviceName}RequestProfile(),",
                $"\t\t\t\t\tnew Update{serviceName}RequestProfile(),",
                $"\n",
            };
            
            pre = pre.Concat(add);
            pre = pre.Concat(post);

            try
            {
                File.WriteAllText(filepath, string.Join('\n', pre));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed to inject the dependencies in file: {ex.Message}.");
            }
            
        }

        /**
        *
        *end service file importation, injection and profile adding
        *
        */

         /**
        *
        *start DBContext file importation, injection
        *
        */

        public void DBContextNameSpacesImportation(string filepath, string projectName, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "/* End Import */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\t//{projectName} namespaces importation for DBContext",
                $"\tusing {projectName}.Repositories.{serviceName}.DtoModels;",
                $"\n",
            };
            
            pre = pre.Concat(add);
            pre = pre.Concat(post);

            try
            {
                File.WriteAllText(filepath, string.Join('\n', pre));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed to import namespaces DBContext in file: {ex.Message}.");
            }
            
        }
        public void DBContextApplicationInjection(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "        /** End DBContext Adding */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\t//{serviceName} DBContext application injections",
                $"\t\t\tDbSet<{serviceName}DtoModel> IApplicationDbContext.{serviceName}s {{ get; set; }}",
                $"\n",
            };
            
            pre = pre.Concat(add);
            pre = pre.Concat(post);

            try
            {
                File.WriteAllText(filepath, string.Join('\n', pre));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed to inject the DBContext dependencies in file: {ex.Message}.");
            }
            
        }
        public void DBContextIApplicationInjection(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "        /** End Interface DBContext Adding */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\t\t\t//{serviceName} DBContext application injections",
                $"\t\t\tDbSet<{serviceName}DtoModel> {serviceName}s {{ get; set; }}",
                $"\n",
            };
            
            pre = pre.Concat(add);
            pre = pre.Concat(post);

            try
            {
                File.WriteAllText(filepath, string.Join('\n', pre));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed to inject the DBContext in file: {ex.Message}.");
            }
            
        }

      /**
        *
        *end DBContext file importation, injection
        *
        */

         /**
        *
        * start package json project injection
        *
        */

        public void PackageJsonProjectInject(string filepath, string projectName){
            var text = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = Int32.Parse(package.Projects.Last().Port);

            Project project = new Project();
            project.Name = projectName;
            project.Port = (lastport + 1).ToString();

            package.Projects.Add(project);

            string content = "{\n";

            content += $"\t\"name\": \"{package.Name}\",\n";
            content += $"\t\"version\": \"{package.Version}\",\n";
            content += $"\t\"projects\": [\n";

            package.Projects.ForEach(x => {
                content += "\t\t{ \n";
                content += $"\t\t\t\"name\": \"{x.Name}\",\n";
                content += $"\t\t\t\"port\": \"{x.Port}\"\n";
                content += "\t\t}";

                if(!package.Projects.Last().Equals(x)){
                    content += ", \n";
                }
            });
            content += $"\n\t]\n";            
            content += "}";            

            try
            {
                File.WriteAllText(filepath, content);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed package json project injection in file: {ex.Message}.");
            }
            
        }

      /**
        *
        *end  package json project injection
        *
        */

         /**
        *
        * start sh file builder
        *
        */

        public void SHFilesCreationAndInject(string packageFile){
            
            try{
                var text = File.ReadAllText(packageFile);
                Package package = JsonConvert.DeserializeObject<Package>(text);

                Console.WriteLine($" \n******************************************************************************************** \n");
                Console.WriteLine($" building of the solution\n");
                Console.WriteLine($" \n******************************************************************************************** \n");
                var process1 = Process.Start("dotnet", "build");
                process1.WaitForExit();

                if(Directory.Exists("./dist")){
                     var directory = new DirectoryInfo("./dist") { Attributes = FileAttributes.Normal };

                    foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                    {
                        info.Attributes = FileAttributes.Normal;
                    }

                    directory.Delete(true);
                }

                Directory.CreateDirectory("./dist");
                Directory.SetCurrentDirectory("./dist");

                string startText = "";
                package.Projects.ForEach(x => {

                    Console.WriteLine($" \n******************************************************************************************** \n");
                    Console.WriteLine($" building of {x.Name}\n");
                    Console.WriteLine($" \n******************************************************************************************** \n");
                    var process1 = Process.Start("dotnet", $"restore ./{x.Name}/{x.Name}.csproj");
                    process1.WaitForExit();
                    
                    process1 = Process.Start("dotnet", $"publish ./{x.Name}/{x.Name}.csproj -c Release -o ./dist/{x.Name}");
                    process1.WaitForExit();

                    var file = $"./start-{x.Name.ToLower().Split('.')[0]}.sh";
                    File.Create(file);
                    string content = "";

                    if(package.Projects.First().Equals(x)){
                        content = $"cp ./{x.Name}/ocelot.json .;dotnet ./{x.Name}/{x.Name}.dll --urls \"http://localhost:{x.Port}\";";
                    }else{
                        content = $"dotnet ./{x.Name}/{x.Name}.dll --urls \"http://localhost:{x.Port}\";";
                    }

                    File.WriteAllText(file, content);
                    startText += $"pm2 delete {package.Name}-{x.Name};";
                    startText += $"pm2 start {file} --name {package.Name}-{x.Name};";
                });

                var startfile = $"./start.sh";
                File.Create(startfile);
                File.WriteAllText(startfile, startText);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed sh file builder in file: {ex.Message}.");
            }
            
        }

        public void SHFilesBuilding(string packageFile){
            
            try{
                var text = File.ReadAllText(packageFile);
                Package package = JsonConvert.DeserializeObject<Package>(text);

                Console.WriteLine($" \n******************************************************************************************** \n");
                Console.WriteLine($" building of the solution\n");
                Console.WriteLine($" \n******************************************************************************************** \n");
                var process1 = Process.Start("dotnet", "build");
                process1.WaitForExit();


                package.Projects.ForEach(x => {

                    var file = $"./start-{x.Name.ToLower().Split('.')[0]}.sh";
                    File.Create(file);
                    string content = "";

                    if(package.Projects.First().Equals(x)){
                        content = $"cp ./{x.Name}/ocelot.json .;dotnet ./{x.Name}/{x.Name}.dll --urls \"http://localhost:{x.Port}\";";
                    }else{
                        content = $"dotnet ./{x.Name}/{x.Name}.dll --urls \"http://localhost:{x.Port}\";";
                    }

                    File.WriteAllText(file, content);
                   
                });

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed sh file builder in file: {ex.Message}.");
            }
            
        }

      /**
        *
        *end  sh file builder
        *
        */

        



    }
}