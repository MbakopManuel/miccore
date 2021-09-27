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

         public void ServiceNameSpacesImportationForReference(string packageJson, string filepath, string projectName){
              if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: Package file not found\n\n");
                return;
            }

            var text1 = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text1);
            var project = package.Projects.Where(x => x.Name == $"{projectName}.Microservice").FirstOrDefault();

            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "/* End Import */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            project.Services.ForEach(serviceName => {
                string[] add = new string[]{
                    $"\n",
                    $"\t//{projectName} namespaces importation",
                    $"\tusing {projectName}.Operations.{serviceName}.MapperProfiles;",
                    $"\tusing {projectName}.Services.{serviceName}.MapperProfiles;",
                    $"\n",
                };
                
                pre = pre.Concat(add);
                pre = pre.Concat(post);
            });
            

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

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: Package file not found\n\n");
                return;
            }

            var text1 = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text1);
            var project = package.Projects.Where(x => x.Name == $"{projectName}.Microservice").FirstOrDefault();

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
                pre = pre.Concat(post);
            });
            
           

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

        public void PackageJsonProjectInject(string filepath, string projectName, bool auth){

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: Package file not found\n\n");
                return;
            }

            var text = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = Int32.Parse(package.Projects.Last().Port);
            int lasturl = Int32.Parse(package.Projects.Last().DockerUrl.Split('.')[3]);

            Project project = new Project();
            project.Name = projectName;
            project.Port = (lastport + 1).ToString();
            project.DockerUrl =     package.Projects.Last().DockerUrl.Split('.')[0]+'.'+
                                    package.Projects.Last().DockerUrl.Split('.')[1]+'.'+
                                    package.Projects.Last().DockerUrl.Split('.')[2]+'.'+
                                    (lasturl+1).ToString();
            project.references = new List<string>(){};

            if(auth){
                RenameUtility rename = new RenameUtility();
                rename.Rename($".", "44373", project.Port);
            }

            package.Projects.Add(project);

            string content = "{\n";

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
                Console.WriteLine($"ERROR - Failed package json project injection in file: {ex.Message}.");
            }
            
        }

  /**
        *
        * start package json reference injection
        *
        */

        public void PackageJsonReferenceInject(string filepath, string projectName, string inject){

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: Package file not found\n\n");
                return;
            }

            var text = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            
            Project project = new Project();
            project = package.Projects.Where(x => x.Name == $"{projectName}.Microservice").FirstOrDefault();
            package.Projects.Remove(project);
            project.references.Add(inject.ToLower());
            package.Projects.Add(project);
            

            string content = "{\n";

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
                Console.WriteLine($"ERROR - Failed package json project injection in file: {ex.Message}.");
            }
            
        }


        public void PackageJsonReferenceServiceInject(string filepath, string projectName, string inject){

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: Package file not found\n\n");
                return;
            }

            var text = File.ReadAllText(filepath);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            
            Project project = new Project();
            project = package.Projects.Where(x => x.Name == $"{projectName}.Microservice").FirstOrDefault();
            package.Projects.Remove(project);
            project.Services.Add(inject);
            package.Projects.Add(project);
            

            string content = "{\n";

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
                Console.WriteLine($"ERROR - Failed package json project injection in file: {ex.Message}.");
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
                return;
            }

            var text = File.ReadAllText("./package.json");
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = Int32.Parse(package.Projects.Last().Port);
            string lasturl = package.Projects.Last().DockerUrl;

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

    public void DockerProjectInjection(string filepath, string projectName){

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: docker compose file not found\n\n");
                return;
            }

            var text = File.ReadAllText("./package.json");
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = Int32.Parse(package.Projects.Last().Port);
            string lasturl = package.Projects.Last().DockerUrl;
            
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

                    dict.container_name = projectName.ToLower();
                    dict.ports.RemoveAt(0);
                    dict.ports.Add(lastport+":"+80);
                    dict.build.context = "./"+projectName+".Microservice";
                    dict.build.dockerfile = "Dockerfile."+projectName;
                    dict.networks.static_network.ipv4_address = lasturl;
                    
                    var project = new Project();
                    project = package.Projects.Where(x => x.Name == $"{projectName}.Microservice").FirstOrDefault();
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
                Console.WriteLine($"ERROR - Failed docker project injection in file: {ex.Message}.");
            }
            
    }


    public void DockerReferenceInjection(string filepath, string projectName){

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: docker compose file not found\n\n");
                return;
            }

            var text = File.ReadAllText("./package.json");
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = Int32.Parse(package.Projects.Last().Port);
            string lasturl = package.Projects.Last().DockerUrl;
            
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
                    project = package.Projects.Where(x => x.Name == $"{projectName}.Microservice").FirstOrDefault();
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
                Console.WriteLine($"ERROR - Failed docker project injection in file: {ex.Message}.");
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
                return;
            }

           
            try
            {
                //delete project in package json
                var text = File.ReadAllText("./package.json");
                Package package = JsonConvert.DeserializeObject<Package>(text);
                package.Projects = package.Projects.Where(x => x.Name != $"{projectName}.Microservice").ToList();
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

            if(!File.Exists(packageFile)){
                Console.WriteLine("\n\nError: package file not found\n\n");
                return;
            }

            if(!File.Exists(filepath)){
                Console.WriteLine("\n\nError: Ocelot file not found\n\n");
                return;
            }

            var text = File.ReadAllText(packageFile);
            Package package = JsonConvert.DeserializeObject<Package>(text);
            
            int lastport = Int32.Parse(
                                    package.Projects
                                            .Where(x => x.Name == $"{projectName}.Microservice")
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
                Console.WriteLine($"ERROR - Failed ocelot project service injection in file: {ex.Message}.");
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
                        
                        process1 = Process.Start("dotnet", $"publish ../{x.Name}/{x.Name}.csproj -c Release -o ./{x.Name}");
                        process1.WaitForExit();

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
                        
                        process1 = Process.Start("dotnet", $"publish ../{x.Name}/{x.Name}/{x.Name}.csproj -c Release -o ./{x.Name}");
                        process1.WaitForExit();

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

        public void DockerFilesCreationAndInject(string packageFile){
             
            try{
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
                
                package.Projects.ForEach(x => {

                    if(x.Name == "Gateway.WebApi"){
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        Console.WriteLine($" building of {x.Name}\n");
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        var process1 = Process.Start("dotnet", $"restore ./{x.Name}/{x.Name}.csproj");
                        process1.WaitForExit();
                        
                        process1 = Process.Start("dotnet", $"publish ./{x.Name}/{x.Name}.csproj -c Release");
                        process1.WaitForExit();

                    }else{
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        Console.WriteLine($" building of {x.Name}\n");
                        Console.WriteLine($" \n******************************************************************************************** \n");
                        var process1 = Process.Start("dotnet", $"restore ./{x.Name}/{x.Name}/{x.Name}.csproj");
                        process1.WaitForExit();
                        
                        process1 = Process.Start("dotnet", $"publish ./{x.Name}/{x.Name}/{x.Name}.csproj -c Release");
                        process1.WaitForExit();

                    }
                    
                });


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
                Console.WriteLine($"ERROR - Failed ocelot project service injection in file: {ex.Message}.");
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