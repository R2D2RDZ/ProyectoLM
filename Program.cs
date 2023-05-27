using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

class HttpServer
{
    public static HttpListener listener;
    public static string url = "http://localhost:4352/";
    public static int requestCount;

    public static void Main(string[] args)
    {
        // Create a Http server and start listening for incoming connections
        listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        Console.WriteLine("Listening for connections on {0}", url);

        // Handle requests
        Task listenTask = HandleIncomingConnections();
        listenTask.GetAwaiter().GetResult();

        // Close the listener
        listener.Close();
    }

    public static async Task HandleIncomingConnections()
    {
        bool runServer = true;

        // While a user hasn't visited the `shutdown` url, keep on handling requests
        while (runServer)
        {
            // Will wait here until we hear from a connection
            HttpListenerContext ctx = await listener.GetContextAsync();

            // Peel out the requests and response objects
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse resp = ctx.Response;

            // Print out some info about the request
            Console.WriteLine("Request #: {0}", ++requestCount);
            Console.WriteLine(req.Url.ToString());
            Console.WriteLine(req.HttpMethod);
            Console.WriteLine(req.UserHostName);
            Console.WriteLine(req.UserAgent);
            Console.WriteLine();
            if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/login"))
            {
                Console.WriteLine("Shutdown requested");
                runServer = false;
            }
        }
    }
}