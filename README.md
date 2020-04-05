# NuGetPacksCLI
Console utility for working with NuGet packages from different sources. It allows you to search for packages, shows a list of all the dependencies, and also makes it possible to download packages and all their dependencies to a computer
You can say another client for working with NuGet repositories

#Usage
Clone this repo. Restore packages.
Build project and create package with command 
```
dotnet pack
```
A package will be created in .\nuget folder, which will first need to be copied to the local package repository, and then installed with the command
```
dotnet tool install -g NuGetPacksCLI
```
Further it will be possible to use this tool through the command `ncm`

#Example 
 - List all package versions
   ```
   ncm packs versions -n <packageName>
   ```
 - List meta data of package
   ```
   ncm packs depends -n <packageName>
   ```
 - Find packages which contains this part of name
   ```
   ncm packs find -p | --part <PartOfName>
   ```
 - Download package and all it dependencies
   ```
   ncm packs save -n | --name <packName> (optional) -v | --version <packVersion>
   ```
