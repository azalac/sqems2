/* FILE : ClientProgram.cs
*PROJECT : EMS-2 (TEST CLIENT)
*PROGRAMMER : Blake Ribble
*FIRST VERSION : 2019-03-21
*DESCRIPTION : This test application allows the user to enter a health card number and get a response back
*/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using EMS_Security;

namespace Client
{
    class ClientProgram
    {
        private IPAddress ipAddress;
        private IPEndPoint endpoint;

        /* METHOD : Main
           DESCRIPTION : Calls the method which handles the incoming information
           PARAMETERS : string[] args
           RETURNS : none
        */

        public ClientProgram()
        {
            // Default port to connect to
            int port = 2019;

            // For client to connect (in our case EMS), get the IP of service and put it into a text file
            ipAddress = IPAddress.Parse(IPAddressFromTextFile());

            // Create an endpoint
            endpoint = new IPEndPoint(ipAddress, port);


        }

        /* METHOD : HandleClient
           DESCRIPTION : Method that allows user to enter a health card number and get a response back
           PARAMETERS : string[] args
           RETURNS : none
        */
        public string ValidateHCN(string HCN)
        {
            //Create a byte buffer which will be used later on by the server
            byte[] byteBuffer = new byte[1024];

            //Another byte buffer which is used when sending a message to server
            byte[] msg;

            //int which will hold the number of bytes that were sent
            int bytesSent;

            //int which will hold the number of bytes that were recieved from server
            int bytesRec;

            //Create a new client socket connection
            Socket clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Connect to the server through the endpoint
            clientSocket.Connect(endpoint);

            // Encrypt the message
            string encryptedMsg = Encryption.Encrypt(HCN);

            //Send information over
            msg = Encoding.ASCII.GetBytes(encryptedMsg);

            // Send the message
            bytesSent = clientSocket.Send(msg);

            // Sleep so the client doesn't catch the message right away
            Thread.Sleep(10);

            //Recieve response from server
            bytesRec = clientSocket.Receive(byteBuffer);

            //Print data to result
            string response = Encoding.ASCII.GetString(byteBuffer, 0, bytesRec);

            string endMessage = Encryption.Encrypt("BYE");

            //Send information over
            msg = Encoding.ASCII.GetBytes(endMessage);

            // Send the message
            bytesSent = clientSocket.Send(msg);

            clientSocket.Close();

            return response;

        }

        /* METHOD : IPAddressFromTextFile
           DESCRIPTION : Method that gets the first line of a text file containing the IP and return it
           PARAMETERS : void
           RETURNS : string
        */

        private string IPAddressFromTextFile()
        {
            //Allows for the writing of the logs in the executable file
            string filePath = "../../HCVIPAddress.txt";

            // Read all lines into a string array
            string[] contentArray = File.ReadAllLines(filePath);

            // Return the first line
            return contentArray[0];
        }
    }
}

