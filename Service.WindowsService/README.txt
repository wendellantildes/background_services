Example
https://github.com/dasMulli/dotnet-win32-service

Install
sc.exe create MyService DisplayName= "My Service" binpath= "C:\Program Files\dotnet\dotnet.exe C:\path\to\MyService.dll --run-as-service"