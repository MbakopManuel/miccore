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
using YamlDotNet.Serialization;

namespace miccore.Utility{
    class InjectionUtility : miccoreBaseCmd {

        private string _package = @"
        {<br>
            &nbsp;&nbsp;company: {0};<br>
            &nbsp;&nbsp;name: {1};<br>
            &nbsp;&nbsp;version: {2};<br>
            &nbsp;&nbsp;projects: [];<br>
        }
        ";
        private string _project = @"
        <br>&nbsp;&nbsp;&nbsp;&nbsp;{<br>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; name: {0};<br>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; port: {1};<br>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; dockerUrl: {2};<br>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; references: [];<br>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; services: [];<br>
        &nbsp;&nbsp;&nbsp;&nbsp;}<br>
        ";

        public InjectionUtility(ILogger logger){
            _logger = logger;
        }

        /// <summary>
        /// infrastructure db context injection
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="project"></param>
        /// <param name="serviceName"></param>
        public void InfrastructureDbContextInject(string filepath, string project, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "        #endregion");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);
            string[] add = new string[]{
                $"\t\tpublic DbSet<{project}.Core.Entities.{serviceName}> {serviceName}s",
                $"\t\t{{",
                $"\t\t\tget;",
                $"\t\t\tset;",
                $"\t\t}}",
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
                OnException(ex);
                throw new Exception(ex.Message);
            }
        }   

        /// <summary>
        /// infrastructure service injection
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="project"></param>
        /// <param name="serviceName"></param>
        public void InfrastructureServiceInject(string filepath, string project, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "            #endregion");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);
            string[] add = new string[]{
                $"\t\t\tservices.TryAddScoped<{project}.Core.Repositories.I{serviceName}Repository, {project}.Infrastructure.Repositories.{serviceName}Repository>();",
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
                OnException(ex);
                throw new Exception(ex.Message);
            }
        }   
        

        /// <summary>
        /// inject enumation of the service
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="serviceName"></param>
        public void CoreEnumerationInject(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "        #endregion");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);
            string[] add = new string[]{
                $"\t\t[Description(\"{serviceName} not found\")]",
                $"\t\t{serviceName.ToUpper()}_NOT_FOUND,",
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
                OnException(ex);
                throw new Exception(ex.Message);
            }
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


        
        public void ServiceNameSpacesImportationForReference(string packageJson, string filepath, string projectName){
            if(!File.Exists(packageJson)){
                Console.WriteLine("\n\nError: Package file not found\n\n");
                throw new Exception();
            }

            var text1 = File.ReadAllText(packageJson);
            Package package = JsonConvert.DeserializeObject<Package>(text1);
            var project = package.Projects.Where(x => x.Name == $"{projectName}.Api").FirstOrDefault();
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "/* End Import */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string companyName = package.Company;
            string projetName = package.Name;

            project.Services.ForEach(serviceName => {
                string[] add = new string[]{
                    $"\n",
                    $"\t//{companyName}.{projetName}.{projectName} namespaces importation",
                    $"\tusing {companyName}.{projetName}.{projectName}.Api.Operations.{serviceName}.MapperProfiles;",
                    $"\tusing {companyName}.{projetName}.{projectName}.Api.Services.{serviceName}.MapperProfiles;",
                    $"\n",
                };
                
                pre = pre.Concat(add);
            });


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

        public void ServiceProfileAddingForReference(string packageJson, string filepath, string projectName){

            if(!File.Exists(packageJson)){
                Console.WriteLine("\n\nError: Package file not found\n\n");
                throw new Exception();
            }

            var text1 = File.ReadAllText(packageJson);
            Package package = JsonConvert.DeserializeObject<Package>(text1);
            var project = package.Projects.Where(x => x.Name == $"{projectName}.Api").FirstOrDefault();

            Console.WriteLine(string.Join(',', project.references));

            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "                /** End Adding Profiles */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            project.Services.ForEach(serviceName =>{
                string[] add = new string[]{
                    $"\n",
                    $"\t\t\t\t\t//{serviceName} adding profiles",
                    $"\t\t\t\t\tnew {serviceName}Profile(),",
                    $"\t\t\t\t\tnew {serviceName}ResponseProfile(),",
                    $"\n",
                };
                
                pre = pre.Concat(add);
            });
            
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

        /// <summary>
        /// start package json project injection
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="projectName"></param>
        /// <param name="auth"></param>
        public void PackageJsonProjectInject(string filepath, string projectName, bool auth){
            if(!File.Exists(filepath)){
                OutputError("Error: Package file not found");
                throw new Exception();
            }
            var name = projectName.Split('.')[projectName.Split('.').Length - 1];
            var text = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = int.Parse(package.Projects.First().Port);
            int i = 0;
            foreach (var item in package.Projects)
            {
                if(lastport < int.Parse(item.Port)){
                    lastport = int.Parse(item.Port);
                    i++;
                }
            }
            int lasturl = Int32.Parse(package.Projects.ElementAt(i).DockerUrl.Split('.')[3]);
           
            Project project = new Project();
            project.Name = name;
            project.Port = (lastport + 1).ToString();
            project.DockerUrl =     package.Projects.Last().DockerUrl.Split('.')[0]+'.'+
                                    package.Projects.Last().DockerUrl.Split('.')[1]+'.'+
                                    package.Projects.Last().DockerUrl.Split('.')[2]+'.'+
                                    (lasturl+1).ToString();
            project.references = new List<string>(){};
            project.Services = new List<string>(){};
            project.Services.Add(name);
            
            RenameUtility.Rename($".", "5081", project.Port);
            if(auth){
                project.Services.Add("User");
                project.Services.Add("Role");
            }

            package.Projects.Add(project);

            string content = "{\n";

            content += $"\t\"company\": \"{package.Company}\",\n";
            content += $"\t\"name\": \"{package.Name}\",\n";
            content += $"\t\"version\": \"{package.Version}\",\n";
            content += $"\t\"projects\": [\n";

            package.Projects.ForEach(x => {


                string refer = "[";
                x.references.ForEach(y => {
                    refer += $"\"{y.ToString()}\"";
                    if(x.references.Last() != y){
                        refer += ",";
                    }
                });
                refer += "]";

                string serv = "[";
                x.Services.ForEach(y => {
                    serv += $"\"{y}\"";
                    if(x.Services.Last() != y){
                        serv += ",";
                    }
                });
                serv += "]";

                content += "\t\t{ \n";
                content += $"\t\t\t\"name\": \"{x.Name}\",\n";
                content += $"\t\t\t\"port\": \"{x.Port}\",\n";
                content += $"\t\t\t\"dockerUrl\": \"{x.DockerUrl}\",\n";
                content += $"\t\t\t\"references\": {refer},\n";
                content += $"\t\t\t\"services\": {serv}\n";
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
                OnException(ex);
                throw new Exception();
            }
            
        }


        public void PackageJsonRemoveProject(string filepath, string projectName){

            if(!File.Exists(filepath)){
                OutputError("Error: Package file not found");
                throw new Exception();
            }
            var name = projectName.Split('.')[projectName.Split('.').Length - 1];
            var text = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            // get project
            var project = package.Projects.Where(x => x.Name == name).FirstOrDefault();

            if(project is null){
                OutputError("Error: Project not exist");
                throw new Exception();
            }

            // check for references
            var references = package.Projects.Where(x => x.references.Contains(name)).Count();
            if(references > 0){
                OutputError("Error: Project is referenced by other projects");
                throw new Exception();
            }
            
            // remove project
            package.Projects.Remove(project);

            string content = "{\n";

            content += $"\t\"company\": \"{package.Company}\",\n";
            content += $"\t\"name\": \"{package.Name}\",\n";
            content += $"\t\"version\": \"{package.Version}\",\n";
            content += $"\t\"projects\": [\n";

            package.Projects.ForEach(x => {
                string refer = "[";
                x.references.ForEach(y => {
                    refer += $"\"{y.ToString()}\"";
                    if(x.references.Last() != y){
                        refer += ",";
                    }
                });
                refer += "]";

                string serv = "[";
                x.Services.ForEach(y => {
                    serv += $"\"{y}\"";
                    if(x.Services.Last() != y){
                        serv += ",";
                    }
                });
                serv += "]";

                content += "\t\t{ \n";
                content += $"\t\t\t\"name\": \"{x.Name}\",\n";
                content += $"\t\t\t\"port\": \"{x.Port}\",\n";
                content += $"\t\t\t\"dockerUrl\": \"{x.DockerUrl}\",\n";
                content += $"\t\t\t\"references\": {refer},\n";
                content += $"\t\t\t\"services\": {serv}\n";
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
                OnException(ex);
                throw new Exception();
            }
            
        }

  /**
        *
        * start package json reference injection
        *
        */

        public void PackageJsonReferenceInject(string filepath, string projectName, string inject){

            var text = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            Project project = new Project();
            project = package.Projects.Where(x => x.Name == projectName).FirstOrDefault();
            package.Projects.Remove(project);
            project.references.Add(inject.ToLower());
            package.Projects.Add(project);

            string content = "{\n";

            content += $"\t\"company\": \"{package.Company}\",\n";
            content += $"\t\"name\": \"{package.Name}\",\n";
            content += $"\t\"version\": \"{package.Version}\",\n";
            content += $"\t\"projects\": [\n";

            package.Projects.ForEach(x => {


                string refer = "[";
                x.references.ForEach(y => {
                    refer += $"\"{y.ToLower()}\"";
                    if(x.references.Last() != y){
                        refer += ",";
                    }
                });
                refer += "]";

                string serv = "[";
                x.Services.ForEach(y => {
                    serv += $"\"{y}\"";
                    if(x.Services.Last() != y){
                        serv += ",";
                    }
                });
                serv += "]";

                content += "\t\t{ \n";
                content += $"\t\t\t\"name\": \"{x.Name}\",\n";
                content += $"\t\t\t\"port\": \"{x.Port}\",\n";
                content += $"\t\t\t\"dockerUrl\": \"{x.DockerUrl}\",\n";
                content += $"\t\t\t\"references\": {refer},\n";
                content += $"\t\t\t\"services\": {serv}\n";
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
                OnException(ex);
                throw new Exception($"ERROR - Failed package json project injection in file: {ex.Message}.");
            }
            
        }


        public void PackageJsonReferenceServiceInject(string filepath, string projectName, string inject){

            var text = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            Project project = new Project();
            project = package.Projects.Where(x => x.Name == projectName).FirstOrDefault();
            package.Projects.Remove(project);
            project.Services.Add(inject);
            package.Projects.Add(project);
            

            string content = "{\n";


            content += $"\t\"company\": \"{package.Company}\",\n";
            content += $"\t\"name\": \"{package.Name}\",\n";
            content += $"\t\"version\": \"{package.Version}\",\n";
            content += $"\t\"projects\": [\n";

            package.Projects.ForEach(x => {


                string refer = "[";
                x.references.ForEach(y => {
                    refer += $"\"{y.ToLower()}\"";
                    if(x.references.Last() != y){
                        refer += ",";
                    }
                });

                refer += "]";

                string serv = "[";
                x.Services.ForEach(y => {
                    serv += $"\"{y}\"";
                    if(x.Services.Last() != y){
                        serv += ",";
                    }
                });
                serv += "]";

                content += "\t\t{ \n";
                content += $"\t\t\t\"name\": \"{x.Name}\",\n";
                content += $"\t\t\t\"port\": \"{x.Port}\",\n";
                content += $"\t\t\t\"dockerUrl\": \"{x.DockerUrl}\",\n";
                content += $"\t\t\t\"references\": {refer},\n";
                content += $"\t\t\t\"services\": {serv}\n";
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
                OnException(ex);
                throw new Exception($"ERROR - Failed package json project injection in file: {ex.Message}.");
            }
            
        }

         /**
        *
        * start ocelot project injection
        *
        */


        public void OcelotProjectInjection(string filepath, string projectName){

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: Ocelot file not found\n\n");
                throw new Exception();
            }

            var text = File.ReadAllText("./package.json");
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = int.Parse(package.Projects.First().Port);
            int i = 0;
            foreach (var item in package.Projects)
            {
                if(lastport < int.Parse(item.Port)){
                    lastport = int.Parse(item.Port);
                    i++;
                }
            }
            string lasturl = package.Projects.ElementAt(i).DockerUrl;

            var ocelotText = File.ReadAllText(filepath);
            Ocelot ocelot = JsonConvert.DeserializeObject<Ocelot>(ocelotText);

            DownstreamHostAndPort down = new DownstreamHostAndPort();
            down.Host = "localhost";
            down.Port = lastport;

            OcelotObject obj = new OcelotObject();
            obj.DownstreamHostAndPorts = new List<DownstreamHostAndPort>(){down};
            obj.DownstreamPathTemplate = $"/{projectName.ToLower()}";
            obj.DownstreamScheme = "http";
            obj.UpstreamPathTemplate = $"/api/{projectName.ToLower()}";
            obj.UpstreamHttpMethod = new List<string>(){"POST", "GET", "OPTIONS"};
            obj.SwaggerKey = $"{projectName}s";

            OcelotObject objId = new OcelotObject();
            objId.DownstreamHostAndPorts = new List<DownstreamHostAndPort>(){down};
            objId.DownstreamPathTemplate = $"/{projectName.ToLower()}/{{id}}";
            objId.DownstreamScheme = "http";
            objId.UpstreamPathTemplate = $"/api/{projectName.ToLower()}/{{id}}";
            objId.UpstreamHttpMethod = new List<string>(){"GET", "PUT", "DELETE", "OPTIONS"};
            objId.SwaggerKey = $"{projectName}s";

            ConfigVersion conf = new ConfigVersion();
            conf.Name = $"{projectName}s Microservice API";
            conf.Version = "v1";
            conf.Url = $"http://localhost:{lastport}/swagger/v1/swagger.json";

            SwaggerEndPointObject end = new SwaggerEndPointObject();
            end.Config = new List<ConfigVersion>(){conf};
            end.Key = $"{projectName}s";

            ocelot.Routes.Add(obj);
            ocelot.Routes.Add(objId);
            ocelot.SwaggerEndPoints.Add(end);


            string content = "{\n";

            content += $"\t\"Routes\": [\n";
            ocelot.Routes.ForEach(x => {
                content += "\t\t{ \n";
                content += $"\t\t\t\"DownstreamPathTemplate\": \"{x.DownstreamPathTemplate}\",\n";
                content += $"\t\t\t\"DownstreamScheme\": \"{x.DownstreamScheme}\",\n";
                content += $"\t\t\t\"DownstreamHostAndPorts\": [\n";
                
                x.DownstreamHostAndPorts.ForEach(y => {
                    content += $"\t\t\t\t{{\n";
                    content += $"\t\t\t\t\t\"Host\":\"{y.Host}\",\n";
                    content += $"\t\t\t\t\t\"Port\": {y.Port}\n";
                    content += $"\t\t\t\t}}\n";
                });
                
                content += $"\t\t\t],\n";
                content += $"\t\t\t\"UpstreamPathTemplate\": \"{x.UpstreamPathTemplate}\",\n";
                content += $"\t\t\t\"UpstreamHttpMethod\":[ ";

                x.UpstreamHttpMethod.ForEach(u => {
                    content += $"\"{u}\"";
                    if(!x.UpstreamHttpMethod.Last().Equals(u)){
                        content += $", ";
                    }
                });
                content += $" ],\n";

                content += $"\t\t\t\"SwaggerKey\": \"{x.SwaggerKey}\"\n";
                content += "\t\t}";

                if(!ocelot.Routes.Last().Equals(x)){
                    content += ", \n";
                }
            });
            content += $"\n\t],\n";    
            
            content += $"\t\"SwaggerEndPoints\": [\n"; 
            ocelot.SwaggerEndPoints.ForEach(x => {
                content += "\t\t{ \n";
                content += $"\t\t\t\"Key\": \"{x.Key}\",\n";
                content += $"\t\t\t\"Config\": [\n";
                
                x.Config.ForEach(y => {
                    content += $"\t\t\t\t{{\n";
                    content += $"\t\t\t\t\t\"Name\":\"{y.Name}\",\n";
                    content += $"\t\t\t\t\t\"Version\": \"{y.Version}\",\n";
                    content += $"\t\t\t\t\t\"Url\": \"{y.Url}\"\n";
                    content += $"\t\t\t\t}}\n";
                });
                
                content += $"\t\t\t]\n";
               content += "\t\t}";
                if(!ocelot.SwaggerEndPoints.Last().Equals(x)){
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
                Console.WriteLine($"ERROR - Failed ocelot project injection in file: {ex.Message}.");
            }
            
        }
        
        /// <summary>
        /// inject project to docker compose yml
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="projectName"></param>
        public void DockerProjectInjection(string filepath, string projectName){

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: docker compose file not found\n\n");
                throw new Exception();
            }

            var text = File.ReadAllText("./package.json");
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            var pack = package.Projects.Where(x => x.Name == projectName).First();
            int lastport = int.Parse(pack.Port);
            string lasturl = pack.DockerUrl;
            
            var deserialise = new YamlDotNet.Serialization.Deserializer();

            try
            {
                using (var reader = new StreamReader(filepath))
                {
                    var obj = deserialise.Deserialize<Dictionary<object, object>>(reader);
                    var services = (Dictionary<object, object>)obj["services"];
                   
                    var gateway = services["gateway"];
                     
                    string json = JsonConvert.SerializeObject(gateway, Formatting.Indented);
                    
                    ServiceItem dict = new ServiceItem();
                    dict = JsonConvert.DeserializeObject<ServiceItem>(json);
                     Console.WriteLine("\n\narrive ici 1\n\n");
                    dict.container_name = projectName.ToLower();
                    dict.ports.RemoveAt(0);
                    dict.ports.Add(lastport+":"+80);
                    dict.image = ($"{package.Company}.{package.Name}.{projectName}").ToLower()+".image:latest";
                    dict.networks = new Network();
                    dict.networks.static_network = new NetworkItem();
                    dict.networks.static_network.ipv4_address = lasturl;
                    
                     Console.WriteLine("\n\narrive ici 2\n\n");
                    var project = new Project();
                    project = package.Projects.Where(x => x.Name == $"{projectName}").FirstOrDefault();
                    project.references.ForEach(y => {
                        dict.depends_on.Add(y);
                    });

                     Console.WriteLine("\n\narrive ici 3\n\n");
                    services[projectName.ToLower()] = dict; 
                    obj["services"] = services;
                    var serializer = new SerializerBuilder().Build();
                    var yaml = serializer.Serialize(obj);

                     Console.WriteLine("\n\narrive ici 4\n\n");
                    File.WriteAllText(filepath, yaml);
                }
            }
            catch (System.Exception ex)
            {
                OnException(ex);
                throw new Exception();
            }
            
    }


        /// <summary>
        /// remove project to docker compose yml
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="projectName"></param>
        public void DockerProjectRemove(string filepath, string projectName){

            if(!File.Exists(filepath)){
                OutputError("\n\nError: docker compose file not found\n\n");
                throw new Exception();
            }
            try
            {
            
                var deserialise = new YamlDotNet.Serialization.Deserializer();
                
                using(var reader = new StreamReader(filepath))
                {
                    var obj = deserialise.Deserialize<Dictionary<object, object>>(reader);
                    var services = (Dictionary<object, object>) obj["services"];
                    
                    var project = services[projectName];

                    services.Remove(projectName);

                    obj["services"] = services;
                    var serializer = new SerializerBuilder().Build();
                    var yaml = serializer.Serialize(obj);

                    File.WriteAllText(filepath, yaml);
                }
            }
            catch (System.Exception ex)
            {
                OnException(ex);
                throw new Exception();
            }
            
    }


        public void DockerReferenceInjection(string filepath, string projectName){

            if(!File.Exists(filepath)){
                OutputError("Error: docker compose file not found");
                throw new Exception();
            }

            var text = File.ReadAllText("./package.json");
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = int.Parse(package.Projects.First().Port);
            int i = 0;
            foreach (var item in package.Projects)
            {
                if(lastport < int.Parse(item.Port)){
                    lastport = int.Parse(item.Port);
                    i++;
                }
            }
            string lasturl = package.Projects.ElementAt(i).DockerUrl;
            
            var deserialise = new YamlDotNet.Serialization.Deserializer();

            try
            {
                using (var reader = new StreamReader(filepath))
                {
                    var obj = deserialise.Deserialize<Dictionary<object, object>>(reader);
                    var services = (Dictionary<object, object>)obj["services"];
                   
                    var gateway = services[projectName.ToLower()];
                     
                    string json = JsonConvert.SerializeObject(gateway, Formatting.Indented);
                    
                    ServiceItem dict = new ServiceItem();
                    dict = JsonConvert.DeserializeObject<ServiceItem>(json);

                   
                    var project = new Project();
                    project = package.Projects.Where(x => x.Name == $"{projectName}").FirstOrDefault();
                    project.references.ForEach(y => {
                        dict.depends_on.Add(y);
                    });

                    services[projectName.ToLower()] = dict; 
                    obj["services"] = services;
                    var serializer = new SerializerBuilder().Build();
                    var yaml = serializer.Serialize(obj);

                    File.WriteAllText(filepath, yaml);
                }
            }
            catch (System.Exception ex)
            {
                OnException(ex);
                throw new Exception();
            }
            
    }


      /**
        *
        *end  ocelot project injection
        *
        */

         /**
        *
        * start ocelot project delete
        *
        */


        public void ProjectDeletion(string filepath, string projectName){

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: Ocelot file not found\n\n");
                throw new Exception();
            }

           
            try
            {
                //delete project in package json
                var text = File.ReadAllText("./package.json");
                Package package = JsonConvert.DeserializeObject<Package>(text);
                package.Projects = package.Projects.Where(x => x.Name != $"{projectName}.Api").ToList();
                string content = "{\n";


                content += $"\t\"companyName\": \"{package.Company}\",\n";
                content += $"\t\"name\": \"{package.Name}\",\n";
                content += $"\t\"version\": \"{package.Version}\",\n";
                content += $"\t\"projects\": [\n";

                package.Projects.ForEach(x => {
                    content += "\t\t{ \n";
                    content += $"\t\t\t\"name\": \"{x.Name}\",\n";
                    content += $"\t\t\t\"port\": \"{x.Port}\",\n";
                    content += $"\t\t\t\"dockerUrl\": \"{x.DockerUrl}\",\n";
                    content += $"\t\t\t\"references\": {x.references},\n";
                    content += $"\t\t\t\"services\": {x.Services}\n";
                    content += "\t\t}";

                    if(!package.Projects.Last().Equals(x)){
                        content += ", \n";
                    }
                });
                content += $"\n\t]\n";            
                content += "}"; 
                File.WriteAllText("./package.json", content);

                //delete project in ocelot
                var ocelotText = File.ReadAllText(filepath);
                Ocelot ocelot = JsonConvert.DeserializeObject<Ocelot>(ocelotText);
                ocelot.Routes = ocelot.Routes.Where(x => x.SwaggerKey != $"{projectName}s").ToList();
                ocelot.SwaggerEndPoints = ocelot.SwaggerEndPoints.Where(x => x.Key != $"{projectName}s").ToList();

                content = "{\n";

                content += $"\t\"Routes\": [\n";
                ocelot.Routes.ForEach(x => {
                content += "\t\t{ \n";
                content += $"\t\t\t\"DownstreamPathTemplate\": \"{x.DownstreamPathTemplate}\",\n";
                content += $"\t\t\t\"DownstreamScheme\": \"{x.DownstreamScheme}\",\n";
                content += $"\t\t\t\"DownstreamHostAndPorts\": [\n";
                
                x.DownstreamHostAndPorts.ForEach(y => {
                    content += $"\t\t\t\t{{\n";
                    content += $"\t\t\t\t\t\"Host\":\"{y.Host}\",\n";
                    content += $"\t\t\t\t\t\"Port\": {y.Port}\n";
                    content += $"\t\t\t\t}}\n";
                });
                
                content += $"\t\t\t],\n";
                content += $"\t\t\t\"UpstreamPathTemplate\": \"{x.UpstreamPathTemplate}\",\n";
                content += $"\t\t\t\"UpstreamHttpMethod\":[ ";

                x.UpstreamHttpMethod.ForEach(u => {
                    content += $"\"{u}\"";
                    if(!x.UpstreamHttpMethod.Last().Equals(u)){
                        content += $", ";
                    }
                });
                content += $" ],\n";

                content += $"\t\t\t\"SwaggerKey\": \"{x.SwaggerKey}\"\n";
                content += "\t\t}";

                if(!ocelot.Routes.Last().Equals(x)){
                    content += ", \n";
                }
                });
                content += $"\n\t],\n";    
                
                content += $"\t\"SwaggerEndPoints\": [\n"; 
                ocelot.SwaggerEndPoints.ForEach(x => {
                    content += "\t\t{ \n";
                    content += $"\t\t\t\"Key\": \"{x.Key}\",\n";
                    content += $"\t\t\t\"Config\": [\n";
                    
                    x.Config.ForEach(y => {
                        content += $"\t\t\t\t{{\n";
                        content += $"\t\t\t\t\t\"Name\":\"{y.Name}\",\n";
                        content += $"\t\t\t\t\t\"Version\": \"{y.Version}\",\n";
                        content += $"\t\t\t\t\t\"Url\": \"{y.Url}\"\n";
                        content += $"\t\t\t\t}}\n";
                    });
                    
                    content += $"\t\t\t]\n";
                    content += "\t\t}";
                    if(!ocelot.SwaggerEndPoints.Last().Equals(x)){
                        content += ", \n";
                    }

                });
                content += $"\n\t]\n";   

                content += "}";
                File.WriteAllText(filepath, content);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed ocelot project injection in file: {ex.Message}.");
            }
            
        }

      /**
        *
        *end  ocelot project delete
        *
        */


         /**
        *
        * start ocelot project service injection
        *
        */


          public void OcelotProjectServiceInjection(string packageFile, string filepath, string projectName, string serviceName){

            if(!File.Exists(filepath)){
                OutputError("\n\nError: Ocelot file not found\n\n");
                throw new Exception();
            }

            var text = File.ReadAllText(packageFile);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = Int32.Parse(
                                    package.Projects
                                            .Where(x => x.Name == projectName)
                                            .First()
                                            .Port
                                    );

            var ocelotText = File.ReadAllText(filepath);
            Ocelot ocelot = JsonConvert.DeserializeObject<Ocelot>(ocelotText);

            DownstreamHostAndPort down = new DownstreamHostAndPort();
            down.Host = "localhost";
            down.Port = lastport;

            OcelotObject obj = new OcelotObject();
            obj.DownstreamHostAndPorts = new List<DownstreamHostAndPort>(){down};
            obj.DownstreamPathTemplate = $"/{serviceName.ToLower()}";
            obj.DownstreamScheme = "http";
            obj.UpstreamPathTemplate = $"/api/{projectName.ToLower()}/{serviceName.ToLower()}";
            obj.UpstreamHttpMethod = new List<string>(){"POST", "GET", "OPTIONS"};
            obj.SwaggerKey = $"{projectName}s";

            OcelotObject objId = new OcelotObject();
            objId.DownstreamHostAndPorts = new List<DownstreamHostAndPort>(){down};
            objId.DownstreamPathTemplate = $"/{serviceName.ToLower()}/{{id}}";
            objId.DownstreamScheme = "http";
            objId.UpstreamPathTemplate = $"/api/{projectName.ToLower()}/{serviceName.ToLower()}/{{id}}";
            objId.UpstreamHttpMethod = new List<string>(){"GET", "PUT", "DELETE", "OPTIONS"};
            objId.SwaggerKey = $"{projectName}s";

            ocelot.Routes.Add(obj);
            ocelot.Routes.Add(objId);

            string content = "{\n";

            content += $"\t\"Routes\": [\n";
            ocelot.Routes.ForEach(x => {
                content += "\t\t{ \n";
                content += $"\t\t\t\"DownstreamPathTemplate\": \"{x.DownstreamPathTemplate}\",\n";
                content += $"\t\t\t\"DownstreamScheme\": \"{x.DownstreamScheme}\",\n";
                content += $"\t\t\t\"DownstreamHostAndPorts\": [\n";
                
                x.DownstreamHostAndPorts.ForEach(y => {
                    content += $"\t\t\t\t{{\n";
                    content += $"\t\t\t\t\t\"Host\":\"{y.Host}\",\n";
                    content += $"\t\t\t\t\t\"Port\": {y.Port}\n";
                    content += $"\t\t\t\t}}\n";
                });
                
                content += $"\t\t\t],\n";
                content += $"\t\t\t\"UpstreamPathTemplate\": \"{x.UpstreamPathTemplate}\",\n";
                content += $"\t\t\t\"UpstreamHttpMethod\":[ ";

                x.UpstreamHttpMethod.ForEach(u => {
                    content += $"\"{u}\"";
                    if(!x.UpstreamHttpMethod.Last().Equals(u)){
                        content += $", ";
                    }
                });
                content += $" ],\n";

                content += $"\t\t\t\"SwaggerKey\": \"{x.SwaggerKey}\"\n";
                content += "\t\t}";

                if(!ocelot.Routes.Last().Equals(x)){
                    content += ", \n";
                }
            });
            content += $"\n\t],\n";    
            
            content += $"\t\"SwaggerEndPoints\": [\n"; 
            ocelot.SwaggerEndPoints.ForEach(x => {
                content += "\t\t{ \n";
                content += $"\t\t\t\"Key\": \"{x.Key}\",\n";
                content += $"\t\t\t\"Config\": [\n";
                
                x.Config.ForEach(y => {
                    content += $"\t\t\t\t{{\n";
                    content += $"\t\t\t\t\t\"Name\":\"{y.Name}\",\n";
                    content += $"\t\t\t\t\t\"Version\": \"{y.Version}\",\n";
                    content += $"\t\t\t\t\t\"Url\": \"{y.Url}\"\n";
                    content += $"\t\t\t\t}}\n";
                });
                
                content += $"\t\t\t]\n";
               content += "\t\t}";
                if(!ocelot.SwaggerEndPoints.Last().Equals(x)){
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
                OnException(ex);
                throw new Exception($"ERROR - Failed ocelot project service injection in file: {ex.Message}.");
            }
            
        }

      /**
        *
        *end  ocelot project service injection
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
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }

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
                if(!Directory.Exists("./wwwroot")){
                    Directory.CreateDirectory("wwwroot");
                }
                string startText = "";
                package.Projects.ForEach(x => {
                    if(x.Name == "Gateway.WebApi"){
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        Console.WriteLine($" building of {x.Name}\n");
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        var process1 = Process.Start("dotnet", $"restore ../{x.Name}/{x.Name}.csproj");
                        process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }
                        
                        process1 = Process.Start("dotnet", $"publish ../{x.Name}/{x.Name}.csproj -c Release -o ./{x.Name}");
                        process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }

                        var file = $"./start-{x.Name.ToLower().Split('.')[0]}.sh";
                        string content = "";

                        content = $"cp ./{x.Name}/ocelot.json .;dotnet ./{x.Name}/{x.Name}.dll --urls \"http://localhost:{x.Port}\";";
                        

                        File.WriteAllText(file, content);
                        startText += $"pm2 delete {package.Name}-{x.Name};";
                        startText += $"pm2 start {file} --name {package.Name}-{x.Name};";
                    }else{
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        Console.WriteLine($" building of {x.Name}\n");
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        var process1 = Process.Start("dotnet", $"restore ../{x.Name}/{x.Name}/{x.Name}.csproj");
                        process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }
                        
                        process1 = Process.Start("dotnet", $"publish ../{x.Name}/{x.Name}/{x.Name}.csproj -c Release -o ./{x.Name}");
                        process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }

                        var file = $"./start-{x.Name.ToLower().Split('.')[0]}.sh";
                        string content = "";

                        content = $"dotnet ./{x.Name}/{x.Name}.dll --urls \"http://localhost:{x.Port}\";";
                       

                        File.WriteAllText(file, content);
                        startText += $"pm2 delete {package.Name}-{x.Name};";
                        startText += $"pm2 start {file} --name {package.Name}-{x.Name};";
                    }
                    
                });

                var startfile = $"./start.sh";
                File.WriteAllText(startfile, startText);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed sh file builder in file: {ex.Message}.");
            }
            
        }

        /// <summary>
        /// build solution with docker
        /// </summary>
        /// <param name="packageFile"></param>
        /// <param name="cmd"></param>
        public void DockerFilesCreationAndInject(string packageFile, string cmd)
        {
            var process = new Process();
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            try
            {
                
                var text = File.ReadAllText(packageFile);
                Package package = JsonConvert.DeserializeObject<Package>(text);
                var prefix = $"{package.Company}.{package.Name}";
                var ocelotText = File.ReadAllText($"./{prefix}.Gateway.WebApi/ocelot.json");
                Ocelot ocelot = JsonConvert.DeserializeObject<Ocelot>(ocelotText);
                ocelot.Routes = new List<OcelotObject>();
                ocelot.SwaggerEndPoints = new List<SwaggerEndPointObject>();
                
                // create dist file
                if (Directory.Exists("./dist"))
                {
                    var directory = new DirectoryInfo("./dist") { Attributes = FileAttributes.Normal };

                    foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                    {
                        info.Attributes = FileAttributes.Normal;
                    }

                    directory.Delete(true);
                }
                Directory.CreateDirectory("./dist");

                // generate ocelot file
                OutputToConsole($"Ocelot file generation ...");
                
                package.Projects.Where(x => x.Name != "Gateway.WebApi").ToList().ForEach(x =>
                {

                    var ocelotProject = File.ReadAllText($"./{prefix}.{x.Name}/configuration.json");
                    Ocelot config = JsonConvert.DeserializeObject<Ocelot>(ocelotProject);

                    if(cmd == "build"){
                        var routes = config.Routes.ToList();
                        foreach (var item in routes)
                        {
                            item.DownstreamHostAndPorts[0].Host = x.DockerUrl.ToString();
                            item.DownstreamHostAndPorts[0].Port = 80;
                        }
                        var name = x.Name + 's';
                        var conf = ocelot.SwaggerEndPoints.Where(y => y.Key == name).FirstOrDefault();
                        if (conf != null)
                        {
                            conf.Config[0].Url = $"http://{x.DockerUrl}:80/swagger/v1/swagger.json";
                        }
                    }
                    ocelot.Routes.AddRange(config.Routes);
                    ocelot.SwaggerEndPoints.AddRange(config.SwaggerEndPoints);
                });
                OcelotFileWrite(ocelot, $"./{prefix}.Gateway.WebApi/ocelot.json");


                // build the solution
                OutputToConsole($"Build the solution\n");
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = $"build";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                //Build images
                //string of sh file
                string bash = "#!/bin/bash \n\n\n";
                package.Projects.ForEach(x =>
                {

                    if (x.Name == "Gateway.WebApi")
                    {
                        // restore
                        restoreGateway($"{prefix}.{x.Name}", process);
                       
                        // publish
                        publishGateway($"{prefix}.{x.Name}", process);

                    }
                    else
                    {
                        // restore
                        restoreProject($"{prefix}.{x.Name}", process);
                       
                        // publish
                        publishProject($"{prefix}.{x.Name}", process);

                    }

                     // build image
                    buildImage($"{prefix}.{x.Name}", process);
                    
                    // save image in tar file
                    saveImage($"{prefix}.{x.Name}", process);

                    // add from bash
                    bash += $"# load {x.Name} Image \n";
                    bash += $"docker load --input {prefix.ToLower()}.{x.Name.ToLower()}.image.tar\n\n";

                });

                if(File.Exists("./Dockerfile")){
                    // build migration file
                    buildAndSaveMigrationImage($"{prefix}", process);

                    // add from bash
                    bash += $"# load Migration Image \n";
                    bash += $"docker load --input {prefix.ToLower()}.migration.image.tar\n\n";
                }

                // copy docker compose file to dist
                OutputToConsole($"Save docker compose ...");
                process.StartInfo.FileName = "cp";
                process.StartInfo.Arguments = $"docker-compose.yml ./dist/docker-compose.yml";
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    OutputError(process.StandardError.ReadToEnd());
                    throw new Exception(process.StandardError.ReadToEnd());
                }

                // write bash file
                OutputToConsole($"Generate docker image load sh filee ...");
                var startfile = $"./dist/load-images.sh";
                File.WriteAllText(startfile, bash);

            }
            catch (System.Exception ex)
            {
                OnException(ex);
                OutputError($"ERROR - Failed Build: {ex.Message}.");
                throw new Exception(ex.Message);
            }

        }



        public void DockerFilesCreationAndInjectOut(string packageFile, string OutFolder){
             
            try{
                if(Directory.Exists(OutFolder)){
                     var directory = new DirectoryInfo(OutFolder) { Attributes = FileAttributes.Normal };

                    foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                    {
                        info.Attributes = FileAttributes.Normal;
                    }

                    directory.Delete(true);
                }

                Directory.CreateDirectory(OutFolder);
               
                var text = File.ReadAllText(packageFile);
                Package package = JsonConvert.DeserializeObject<Package>(text);
                var ocelotText = File.ReadAllText("./Gateway.WebApi/ocelot.json");
                Ocelot ocelot = JsonConvert.DeserializeObject<Ocelot>(ocelotText);

                Console.WriteLine($" \n******************************************************************************************** \n");
                Console.WriteLine($" Ocelot docker file generation ...\n");
                Console.WriteLine($" \n******************************************************************************************** \n");
                
                package.Projects.ForEach(x => {
                    
                    var routes = ocelot.Routes.Where(y => y.DownstreamHostAndPorts[0].Port.ToString() == x.Port)
                                            .ToList();
                    foreach (var item in routes)
                    {
                        item.DownstreamHostAndPorts[0].Host = x.DockerUrl.ToString();
                        item.DownstreamHostAndPorts[0].Port = 80;
                    }
                    var name = x.Name.Split('.')[0]+'s';
                    var conf = ocelot.SwaggerEndPoints.Where(y => y.Key == name).FirstOrDefault();
                    if(conf != null){
                        conf.Config[0].Url = $"http://{x.DockerUrl}:80/swagger/v1/swagger.json";
                    }
                    
                });
                OcelotFileWrite(ocelot, "./Gateway.WebApi/ocelot.docker.json");


                Console.WriteLine($" \n******************************************************************************************** \n");
                Console.WriteLine($" building of the solution\n");
                Console.WriteLine($" \n******************************************************************************************** \n");
                var process1 = Process.Start("dotnet", "build");
                process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }

                Directory.SetCurrentDirectory(OutFolder);
                
                package.Projects.ForEach(x => {

                    if(x.Name == "Gateway.WebApi"){
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        Console.WriteLine($" building of {x.Name}\n");
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        var process1 = Process.Start("dotnet", $"restore ../{x.Name}/{x.Name}.csproj");
                        process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }
                        
                        process1 = Process.Start("dotnet", $"publish ../{x.Name}/{x.Name}.csproj -c Release -o ./{x.Name}");
                        process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }

                        var content = $"FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build\n";
                        content += $"WORKDIR /app";
                        content += $"COPY ./{x.Name}/. .";
                        content += $"RUN mv ocelot.docker.json ocelot.json";
                        content += $"ENTRYPOINT [\"dotnet\", \"{x.Name}.dll\"]";
                        File.WriteAllText($"./Dockerfile.{x.Name.Split('.')[0]}", content);
                    
                    }else{
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        Console.WriteLine($" building of {x.Name}\n");
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        var process1 = Process.Start("dotnet", $"restore ../{x.Name}/{x.Name}/{x.Name}.csproj");
                        process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }
                        
                        process1 = Process.Start("dotnet", $"publish ../{x.Name}/{x.Name}/{x.Name}.csproj -c Release -o ./{x.Name}");
                        process1.WaitForExit();
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }

                        var content = $"FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build\n";
                        content += $"WORKDIR /app";
                        content += $"COPY ./{x.Name}/. .";
                        content += $"ENTRYPOINT [\"dotnet\", \"{x.Name}.dll\"]";
                        File.WriteAllText($"./Dockerfile.{x.Name.Split('.')[0]}", content);
        
                    }
                    
                });

                File.Copy("../package.json", "./package.json");

                var content = $"FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build\n";
                content += $"WORKDIR /app";
                content += $"COPY ./ .";
                content += $"FROM mcr.microsoft.com/dotnet/sdk:5.0";
                content += $"WORKDIR /app";
                content += $"COPY --from=build /app/ .";
                content += $"RUN dotnet tool install --global Miccore.Net";
                content += $"RUN dotnet tool install --global dotnet-ef";
                content += $"RUN /root/.dotnet/tools/miccore build --docker";
                content += $"CMD /root/.dotnet/tools/miccore migrate -d /root/.dotnet/tools/dotnet-ef";
                File.WriteAllText($"./Dockerfile.Migration", content);


            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"ERROR - Failed sh file builder in file: {ex.Message}.");
            }
            
        }

        public void OcelotFileWrite(Ocelot ocelot, string filepath){
            string content = "{\n";

            content += $"\t\"Routes\": [\n";
            ocelot.Routes.ForEach(x => {
                content += "\t\t{ \n";
                content += $"\t\t\t\"DownstreamPathTemplate\": \"{x.DownstreamPathTemplate}\",\n";
                content += $"\t\t\t\"DownstreamScheme\": \"{x.DownstreamScheme}\",\n";
                content += $"\t\t\t\"DownstreamHostAndPorts\": [\n";
                
                x.DownstreamHostAndPorts.ForEach(y => {
                    content += $"\t\t\t\t{{\n";
                    content += $"\t\t\t\t\t\"Host\":\"{y.Host}\",\n";
                    content += $"\t\t\t\t\t\"Port\": {y.Port}\n";
                    content += $"\t\t\t\t}}\n";
                });
                
                content += $"\t\t\t],\n";
                content += $"\t\t\t\"UpstreamPathTemplate\": \"{x.UpstreamPathTemplate}\",\n";
                content += $"\t\t\t\"UpstreamHttpMethod\":[ ";

                x.UpstreamHttpMethod.ForEach(u => {
                    content += $"\"{u}\"";
                    if(!x.UpstreamHttpMethod.Last().Equals(u)){
                        content += $", ";
                    }
                });
                content += $" ],\n";

                content += $"\t\t\t\"SwaggerKey\": \"{x.SwaggerKey}\"\n";
                content += "\t\t}";

                if(!ocelot.Routes.Last().Equals(x)){
                    content += ", \n";
                }
            });
            content += $"\n\t],\n";    
            
            content += $"\t\"SwaggerEndPoints\": [\n"; 
            ocelot.SwaggerEndPoints.ForEach(x => {
                content += "\t\t{ \n";
                content += $"\t\t\t\"Key\": \"{x.Key}\",\n";
                content += $"\t\t\t\"Config\": [\n";
                
                x.Config.ForEach(y => {
                    content += $"\t\t\t\t{{\n";
                    content += $"\t\t\t\t\t\"Name\":\"{y.Name}\",\n";
                    content += $"\t\t\t\t\t\"Version\": \"{y.Version}\",\n";
                    content += $"\t\t\t\t\t\"Url\": \"{y.Url}\"\n";
                    content += $"\t\t\t\t}}\n";
                });
                
                content += $"\t\t\t]\n";
               content += "\t\t}";
                if(!ocelot.SwaggerEndPoints.Last().Equals(x)){
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
                OnException(ex);
                OutputError($"ERROR - Failed ocelot project service injection in file: {ex.Message}.");
                throw new Exception(ex.Message);
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
                if (process1.ExitCode != 0)
                {
                    throw new Exception(process1.StandardError.ReadToEnd());
                }


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