# Remote Application Access Station

RAAS allow users of Microsoft Windows operating system to control virtual computers through Microsoft RemoteApp. The program integrates with the shell and is accessible through Microsoft File Explorer and through shortcuts on the desktop and the start-menu. By using RAAS in an administrator controlled environment, users can install programs on virtual computers while still having the benefits of an administrator controlled environment on their local computers.



## Features

- Shortcuts can be automatically created on the desktop and the start-menu for programs on each server

- Users can create their own RemoteApp shortcuts for files and programs they want to open on desired server

- Each server can get its own namespace extension to reach from Microsoft File Explorer, in order to browse files

- Files on servers can be opened through RemoteApp in the namespace extension of Microsoft File Explorer or through its network location

- Visualizations can be configured to let the user know which server the visible applications are executed on

- Startup programs on each server can be started automatically with RemoteApp which lets users interface with startup programs such as antivirus protection.

- A Search & Run application is provided for each server which functions as an replacement for Windows search



## Compile Instructions

Windows 10 SDK, version 10.0.26100.0 needs to be installed
Visual Studio 2022 needs to be intalled
.NET CLI needs to be install
.NET Framework 4.8 development tools needs to be installed
.NET 8.0 SDK needs to be installed
WiX Toolset v5.0.2 needs to be installed

- dotnet tool install --global wix --version 5.0.2

Paths for msbuild.exe, MakeAppX.exe and SignTool.exe need to be added to windows path variable

To build RAAS Client:

- Start build.bat in raasclient folder

This will create "RAASClient_[version]_x64.msi" in raasclient folder

To build RAAS Server:

- Start build.bat in raasserver folder

This will create "RAASServer_[version]_x64.msi" in raasserver folder