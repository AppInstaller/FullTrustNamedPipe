# FullTrustNamedPipe
This is an example of how a UWP will a full trust component can communicate with a classic win32 app over named pipes.

This project contains the following
1. UWP with an App Service
2. This UWP also includes a FullTrust executable - BackgroundProcess.exe
3. A Classic Win32 app - Win32ConsoleApp

Here is the flow that is demonstrated with this example. The Win32ConsoleApp invokes the UWP app via an App Service which is hosted in the same process. The AppService is launched in the background which returns the App specific hardware ID (ASHWID) as a message over Appservices. It then launches the full trust background process. Once the UWP full trust and Win32 process are alive, they communicate via a Namedpipe.

How to deploy?
Set the platfrom to x64 for the UWP app and hit deploy. This will also build the BackgroundProcess and copy over the exe to its Appx directory. Next select the Win32ConsoleApp project and hit f5.
