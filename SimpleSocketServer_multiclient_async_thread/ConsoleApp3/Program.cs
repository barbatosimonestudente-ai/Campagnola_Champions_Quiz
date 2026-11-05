using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SynchronousSocketListener
{

    // Incoming data from the client.  
    public static string data = null;

    public static void StartListening()
    {


        // Establish the local endpoint for the socket.  
        // Dns.GetHostName returns the name of the   
        // host running the application.  
        IPAddress ipAddress = IPAddress.Any;
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        Console.WriteLine("Timeout : {0}",listener.ReceiveTimeout);

        // Bind the socket to the local endpoint and   
        // listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.  
            while (true)
            {
                
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();

                ClientManager clientThread = new ClientManager(handler);
                Thread t = new Thread(new ThreadStart(clientThread.doClient));
                t.Start();

            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static int Main(String[] args)
    {
        StartListening();
        return 0;
    }

    
}

public class ClientManager
{

    Socket clientSocket;
    byte[] bytes = new Byte[1024];
    String data = "";

    public ClientManager(Socket clientSocket)
    {
        this.clientSocket = clientSocket;
    }

    public void doClient()
    {

        do
        {
            // An incoming connection needs to be processed.  
            data = "";
            do
            {
                int bytesRec = clientSocket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            }while (data.IndexOf("$") == -1);

            // Show the data on the console.  


            // cosa deve fare 

            string[] arrayDati;
            string[] messaggio = data.Split('-');
            if (messaggio[1] == "celsius")
            {
                Console.WriteLine(messaggio[0] + " celsius");
            } else if (messaggio[1] == "kelvin")
            {
                int numero = int.Parse(messaggio[0]);
                numero -= 273;
                Console.WriteLine(numero + " celsius");
            } else
            {
                int numero = int.Parse(messaggio[0]);
                numero -= 32;
                numero *= 5;
                numero = numero / 9;

                Console.WriteLine(numero + " celsius");

            }
            

            // Echo the data back to the client.  
            byte[] msg = Encoding.ASCII.GetBytes(data);

            clientSocket.Send(msg);

        } while (data != "Quit$");

        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        data = "";

    }
}
