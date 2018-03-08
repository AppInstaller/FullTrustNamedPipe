using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Win32ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // connect to app service and wait until the connection gets closed
            var appServiceExit = new AutoResetEvent(false);
            Talktoappservice();
            appServiceExit.WaitOne();
        }

        private async static void Talktoappservice()
        {
            string PFN = "Microsoft.AppServiceBridge_8wekyb3d8bbwe";               

            AppServiceConnection connection = new AppServiceConnection();
            connection.PackageFamilyName = PFN;
            connection.AppServiceName = "CommunicationService";
            Console.WriteLine("Opening connection...");

            var status = await connection.OpenAsync();

            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection established - waiting for requests");
                    Console.WriteLine();
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not installed.");
                    return;
                case AppServiceConnectionStatus.AppUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not available.");
                    return;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName));
                    return;
                case AppServiceConnectionStatus.Unknown:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                    return;
            }

            ValueSet valueSet = new ValueSet();
            valueSet.Add("Request", "DeviceID");

            if (connection != null)
            {

                AppServiceResponse response = await connection.SendMessageAsync(valueSet);
                var msg = "Received response: " + response.Message["Response"] as string;
                if (msg != null)
                {
                    Console.WriteLine(msg);
                    NamedPipe();
                }
                else
                {
                    Console.WriteLine(string.Format("Something is wrong."));
                }
            }
        }

        static void NamedPipe()
        {
            bool isRunning = true;

            while (isRunning)
            {
                //Create pipe instance
                NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("testpipe", PipeDirection.InOut, 4);
                Console.WriteLine("[ECHO DAEMON] NamedPipeServerStream thread created.");

                //wait for connection
                Console.WriteLine("[ECHO DAEMON] Wait for a client to connect");
                pipeServer.WaitForConnection();

                Console.WriteLine("[ECHO DAEMON] Client connected.");
                try
                {
                    // Stream for the request. 
                    StreamReader sr = new StreamReader(pipeServer);
                    // read the message
                    string echo = sr.ReadLine();
                    Console.WriteLine("[ECHO DAEMON] Request message: " + echo);

                    // Stream for the response. 
                    StreamWriter sw = new StreamWriter(pipeServer)
                    {
                        AutoFlush = true
                    };
                    // Write response to the stream.
                    sw.WriteLine("[ECHO]: " + "Hello from Classic Win32 via named pipe");

                    // Wait for goodbye
                    echo = sr.ReadLine();
                    Console.WriteLine("[ECHO DAEMON] Recieved message: " + echo);

                    pipeServer.Disconnect();

                    isRunning = false;
                }
                catch (IOException e)
                {
                    Console.WriteLine("[ECHO DAEMON]ERROR: {0}", e.Message);
                }

                pipeServer.Close();
                Console.WriteLine("[ECHO DAEMON] NamedPipeServerStream closed.");
            }
        }
    }
}
