# Miccore .Net 
Tool for dotnet core webapi microservice creation and management or for xamarin project creation.

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

Require .Net core 5.0

Miccore .Net is a CLI which allows a user to create a webapi project with a very specific microservice architecture. it also allows you to create a xamarin project.


## Features

- Create new webapi microservice project architecture
- Create new xamarin mobile project
- Add new microservice in the project architecture
- add service in one microservice
- delete service in microservice
- delete microservice
- build the project and run it


## Tech

Miccore .Net uses a number of open source projects to work properly:

- [Dotnet Core 5.0](https://docs.microsoft.com/) - NET Core 5.0 Runtime enables you to run existing web/server applications. ...
- [Ocelot](https://github.com/ThreeMammals/Ocelot) - Ocelot is a .NET API Gateway
- [Xamarin](https://dotnet.microsoft.com/apps/xamarin) - Open-source mobile app platform for .NET
- [Mysql](https://www.mysql.com/fr/) - MySQL Database Service est un service de base de données entièrement géré pour déployer des applications natives du cloud en utilisant la base de données​ ...
- [Microservice Architecture](https://www.talend.com/fr/resources/guide-microservices/) - application that does only one thing but does it optimally

And of course Miccore .Net itself is open source with a [public repository](https://github.com/MbakopManuel/miccore/tree/main-v2)
 on GitHub.

## Installation

Miccore .Net requires [Dotnet core](https://docs.microsoft.com/) v5.0 or later to run.

Install the dependencies and devDependencies and start the server.

```sh
dotnet tool install --global Miccore.Net
```

## create a new project architecture

You have the possibility to create the new project with or without authentication microservice regarding webapi, but the xamarin project comes with a simple architecture

### First Tab (webapi with auth):

```sh
miccore new --project webapi --name project_name --with-auth
```
or

```sh
miccore new -p webapi -n project_name -wa
```

### Second Tab (webapi without auth):

```sh
miccore new --project webapi --name project_name
```
or

```sh
miccore new -p webapi -n project_name
```

### Second Tab (xamarin project):

```sh
miccore new --project xamarin --name project_name
```
or

```sh
miccore new -p xamarin -n project_name
```

## concerning webapi we can

Miccore .Net easily allows
### add new microservice to architecture

also here, you have possibility to add microservice with or without authentication. the only thing you have to do is to add __--with-auth__ or __-wa__ at the next command if you want it with authentication

```sh
miccore add project --name project_name
```

This will create a web api project in ower microservice architecture and will import it in the ocelot gateway

### delete microservice to architecture


```sh
miccore delete project --name project_name
```

This will create a web api project in ower microservice architecture and will import it in the ocelot gateway

### add new service to a project

```sh
miccore add service --name service_name --project project_name
```
note that the project name is without *.microservice* after it.
This will create a service in the project specified and will do all dependency injection and will set database context.

##  Build webapi architecture project

```sh
miccore build
```

* add after the command __--open__ if you want to serve immediatly after the build.

once the project is built, dist/ folder is created. to launch the project you just have to launch the file *start.sh* included in it

also note that for this to happen you need to install the __pm2__ tool from the node name.

```sh
npm install -g pm2
```

* add aftere command __--docker__ if you want to generate docker file and complete ocelot docker file with all routes.

after it you only have to do __docker-compose up -d --build__ (before it, make sure you had intalled docker and docker-compose in your system)

##  Reference a project in another project

```sh
miccore add reference --from projectNameToReference --to projectNameWhereReferenceWillBeApplied
```
once the command executed, all mapper profile of the project _from will be registered in project _to

##  Run all Migrations

```sh
miccore migrate
``` 
this command based on the dependency tree will execute all the migrations of all the microservices in order of dependency

you can add __-d__ or __--dotnet_exec_path__ to pass the execution path of dotnet-ef . if you don't specify it, miccore will check with the default path

##  Startup project file

once the microservice project is created, you have to go to the startup.cs file and update the database connection elements.


## License

MIT

**Free Software, Hell Yeah!**
