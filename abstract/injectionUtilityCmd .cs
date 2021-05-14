using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace miccore{
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
                $"\n\t//{projectName} namespaces importation",
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

        public void ServiceNameSpacesImportationWithAuth(string filepath, string projectName, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "/* End Import */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\n\t//{projectName} namespaces importation",
                $"\n\tusing {projectName}.Repositories.{serviceName};",
                $"\n\tusing {projectName}.Services.{serviceName};",
                $"\n\tusing {projectName}.Operations.{serviceName}.MapperProfiles;",
                $"\n\tusing {projectName}.Services.{serviceName}.MapperProfiles;",
                $"\n",
                $"\n",
                $"\n\tusing {projectName}.Repositories.Role;",
                $"\n\tusing {projectName}.Services.Role;",
                $"\n\tusing {projectName}.Operations.Role.MapperProfiles;",
                $"\n\tusing {projectName}.Services.Role.MapperProfiles;",
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

        public void ServiceRepositoryServicesInjectionWithAuth(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "            /** End Injection */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\n\t//{serviceName} dependency injections",
                $"\n\t\t\t\t_services.TryAddScoped<I{serviceName}Repository, {serviceName}Repository>();",
                $"\n\t\t\t\t_services.TryAddTransient<I{serviceName}Service, {serviceName}Service>();",
                $"\n",
                $"\n",
                $"\n\t\t\t\t_services.TryAddScoped<IRoleRepository, RoleRepository>();",
                $"\n\t\t\t\t_services.TryAddTransient<IRoleService, RoleService>();",
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
                $"\n\t\t\t\t\t//{serviceName} adding profiles",
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

        public void ServiceProfileAddingWithAuth(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "            /** End Injection */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\n\t//{serviceName} adding profiles",
                $"\n\t\t\t\t\tnew {serviceName}Profile(),",
                $"\n\t\t\t\t\tnew Login{serviceName}RequestProfile(),",
                $"\n\t\t\t\t\tnew {serviceName}ResponseProfile(),",
                $"\n\t\t\t\t\tnew Create{serviceName}RequestProfile(),",
                $"\n\t\t\t\t\tnew Update{serviceName}RequestProfile()",
                $"\n",
                $"\n",
                $"\n\t\t\t\t\tnew RoleProfile(),",
                $"\n\t\t\t\t\tnew RoleResponseProfile(),",
                $"\n\t\t\t\t\tnew CreateRoleRequestProfile(),",
                $"\n\t\t\t\t\tnew UpdateRoleRequestProfile()",
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
                $"\n\t//{projectName} namespaces importation for DBContext",
                $"\n\tusing {projectName}.Repositories.{serviceName}.DtoModels;",
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


        public void DBContextNameSpacesImportationWithAuth(string filepath, string projectName, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "/* End Import */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\n\t//{projectName} namespaces importation for DBContext",
                $"\n\tusing {projectName}.Repositories.{serviceName}.DtoModels;",
                $"\n\tusing {projectName}.Repositories.Role.DtoModels;",
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
                $"\n\t//{serviceName} DBContext application injections",
                $"\n\t\t\tDbSet<{serviceName}DtoModel> IApplicationDbContext.{serviceName}s {{ get; set; }}",
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

        public void DBContextApplicationInjectionWithAuth(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "        /** End DBContext Adding */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\n\t//{serviceName} DBContext application injections",
                $"\n\t\t\tDbSet<{serviceName}DtoModel> IApplicationDbContext.{serviceName}s {{ get; set; }}",
                $"\n\t\t\tDbSet<RoleDtoModel> IApplicationDbContext.Roles {{ get; set; }}",
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

        public void DBContextIApplicationInjection(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "        /** End Interface DBContext Adding */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\n\t//{serviceName} DBContext application injections",
                $"\n\t\t\tDbSet<{serviceName}DtoModel> {serviceName}s {{ get; set; }}",
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

        public void DBContextIApplicationInjectionWithAuth(string filepath, string serviceName){
            var text = File.ReadAllLines(filepath);
            int i = Array.IndexOf(text, "        /** End Interface DBContext Adding */");
            var pre = text.Take(i - 1);
            var post = text.Skip(i-1);

            string[] add = new string[]{
                $"\n",
                $"\n\t//{serviceName} DBContext application injections",
                $"\n\t\t\tDbSet<{serviceName}DtoModel> {serviceName}s {{ get; set; }}",
                $"\n\t\t\tDbSet<RoleDtoModel> Roles {{ get; set; }}",
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

    }
}