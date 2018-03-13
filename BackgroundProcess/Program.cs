//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.AppService;
using System.IO.Pipes;
using System.IO;
using Windows.Security.Credentials;

namespace BackgroundProcess
{
    class Program
    {
        //static AppServiceConnection connection = null;

        /// <summary>
        /// Creates an app service thread
        /// </summary>
        static void Main(string[] args)
        {
            Thread appServiceThread = new Thread(new ThreadStart(ThreadProc));
            appServiceThread.Start();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*****************************");
            Console.WriteLine("****UWP Full Trust Process****");
            Console.WriteLine("*****************************");
            Console.ReadLine();
        }

        static void WriteCredentials()
        {
            //Write to Password vault
            var vault = new Windows.Security.Credentials.PasswordVault();
            vault.Add(new Windows.Security.Credentials.PasswordCredential("My App", "username", "password"));
            Console.WriteLine("Wrote username and password to UWP Password Vault");            
        }
        

        /// <summary>
        /// Creates the app service connection
        /// </summary>
        static void ThreadProc()
        {

            Console.WriteLine("Entered Namedpipe method");
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut))
            {
                // Connect to the pipe or wait until the pipe is available.
                Console.WriteLine("Attempting to connect to pipe...");
                pipeClient.Connect();

                Console.WriteLine("Connected to pipe.");
                Console.WriteLine("There are currently {0} pipe server instances open.", pipeClient.NumberOfServerInstances);
                StreamWriter sw = new StreamWriter(pipeClient)
                {
                    AutoFlush = true
                };

                StreamReader sr = new StreamReader(pipeClient);


                sw.WriteLine("Hello via named pipe from UWP full trust component");
                Console.WriteLine("Wrote to named pipe...");

                // read the message
                string echo = sr.ReadLine();
                Console.WriteLine("Message recieved: " + echo);
                WriteCredentials();

                sw.WriteLine("Goodbye from UWP full trust component!");
                Console.WriteLine("Sent Goodbye over named pipe");

                

                Console.WriteLine("Press any key to exit...");

                Console.ReadLine();
            }
        }
    }        
}
