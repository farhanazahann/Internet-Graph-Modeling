//Names: Khushi Chauhan (0722283); Farhana (0691212); Anubhav Mehandru()
//Create the internet from a logical and physical perspective using adjacency matrix and adjacency lists using a web graph and a server graph
// References:

//Libraries
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// A Server graph created using adjacency matrix that is undirected and unweighted
public class ServerGraph
{
    // 3 marks
    public class WebServer
    {
        //properties
        public string Name;
        public List<WebPage> P; 

        //constructor
        public WebServer(string name)
        {
            Name = name;
            P = new List<WebPage>();
        }
    }

    private WebServer[] V;
    private bool[,] E;
    private int NumServers;

    // 2 marks
    // Create an empty server graph
    public ServerGraph()
    {
        V = new WebServer[2];
        E = new bool[2, 2];
        NumServers = 1;

        //intitialized the first server
        WebServer first_server = new WebServer("A");
        V[0] = first_server;
    }

    // 2 marks
    // Return the index of the server with the given name; otherwise return -1
    public int FindServer(string name)
    {
        for (int i=0; i<NumServers; i++)
        {
            if (V[i].Name == name)
                return i;
        }
        return -1;
    }

    // 3 marks
    // Double the capacity of the server graph with the respect to web servers
    private void DoubleCapacity()
    {
        int newCapacity = V.Length * 2;
        Array.Resize(ref V, newCapacity);
        bool[,] newE = new bool[newCapacity, newCapacity];

        for (int i = 0; i < NumServers; i++)
        {
            // Resize the List<WebPage> associated with each WebServer
            V[i].P.Capacity = newCapacity;

            // Copy existing connections to the new matrix
            for (int j = 0; j < NumServers; j++)
            {
                newE[i, j] = E[i, j];
            }
        }

        E = newE;
    }

    // 3 marks
    // Add a server with the given name and connect it to the other server
    // Return true if successful; otherwise return false
    public bool AddServer(string name, string other)
    {
        // check if the server already exist
        if (FindServer(name) != -1)
            return false;

        // check if the other server exist
        int other_index = -1;
        if ((other_index = FindServer(other)) == -1)
            return false;

        // check if there is a need to double the capacity of the server list
        if (NumServers < V.Length)
            DoubleCapacity();

        // add the server
        WebServer new_server = new WebServer(name);
        int index = NumServers;
        V[index] = new_server;
        NumServers++;

        //add an edge between name and other
        E[other_index,index] = true;
        E[index,other_index] = true;

        Console.WriteLine($"Server {name} is added successfully.");
        return true;
    }

    // 3 marks
    // Add a webpage to the server with the given name
    // Return true if successful; otherwise return false
    public bool AddWebPage(WebPage w, string name)
    {
        //check if the server exist
        int index = -1;
        if ((index = FindServer(name)) == -1)
            return false;

        //adding the webpage to the server
        V[index].P.Add(w);
        Console.WriteLine($"Web Page {w.Name} is added successfully to {name}.");
        return true;
    }

    // 4 marks
    // Remove the server with the given name by assigning its connections
    // and webpages to the other server
    // Return true if successful; otherwise return false
    public bool RemoveServer(string name, string other)
    {
        // Check if the server exists
        int server_index = FindServer(name);
        if (server_index == -1)
        {
            Console.WriteLine($"Error: Server '{name}' doesn't exist");
            return false;
        }

        // Check if the other server exists
        int other_index = FindServer(other);
        if (other_index == -1)
        {
            Console.WriteLine($"Error: Server '{other}' doesn't exist");
            return false;
        }

        // Remove the server
        WebServer server_to_remove = V[server_index];

        // Set the server at server_index to null
        V[server_index] = null!;

        // Remove all connections to this server
        bool connectionsRemoved = false;
        for (int i = 0; i < NumServers; i++)
        {
            // Remove connections from other servers to the server being removed
            if (E[i, server_index])
            {
                E[i, server_index] = false;
                E[server_index, i] = false;

                // If the other server is specified, connect the current server to the other server
                if (i != other_index && other_index != -1)
                {
                    E[i, other_index] = true;
                    E[other_index, i] = true;
                    connectionsRemoved = true;
                }
            }
        }

        if (!connectionsRemoved)
        {
            Console.WriteLine("Warning: No connections found for the server.");
        }

        // Remove the server from the list of servers
        NumServers--;

        if (server_index != NumServers)
        {
            // Replace the removed server with the last server
            if (server_index < NumServers)
            {
                V[server_index] = V[NumServers];
            }

            // replace the edges of the removed with the last server's edges
            for (int j = NumServers; j >= 0; j--)
            {
                if (j == server_index)
                {
                    E[server_index, j] = false;

                }
                E[server_index, j] = E[NumServers, j];
                E[j, server_index] = E[j, NumServers];
            }
        }

        Console.WriteLine($"Server {name} is removed successfully.");
        return true;
    }


    // 3 marks (Bonus)
    // Remove the webpage from the server with the given name
    // Return true if successful; otherwise return false
    /* To remove a web page from the server with the given name.
   Whenever a new web page is added or removed, the corresponding
   method in WebGraph is first called which in turn, calls the
   corresponding add or remove method in ServerGraph using the parameter S.*/
    public bool RemoveWebPage(string webpage, string name)
    {
        // Check if the server exists
        int server_index = FindServer(name);
        if (server_index == -1)
        {
            Console.WriteLine($"Error: Server '{name}' doesn't exist");
            return false;
        }

        // Find the webpage in the server's list of webpages
        WebPage page_to_remove = V[server_index].P.Find(page => page.Name == webpage)!;
        if (page_to_remove != null)
        {
            // Webpage found, remove it from the server's list
            V[server_index].P.Remove(page_to_remove);
            Console.WriteLine($"Web Page {webpage} is removed successfully.");
            return true;  // Webpage successfully removed
        }

        // Webpage not found in the server
        Console.WriteLine($"Error: Webpage '{webpage}' not found in server '{name}'");
        return false;
    }

    // 3 marks
    // Add a connection from one server to another
    // Return true if successful; otherwise return false
    // Note that each server is connected to at least one other server
    public bool AddConnection(string from, string to)
    {
        // Check if both servers exist in the graph
        int from_index = FindServer(from);
        int to_index = FindServer(to);

        if (from_index != -1 && to_index != -1)
        {
            // Add the connection in the adjacency matrix
            E[from_index, to_index] = true;
            E[to_index, from_index] = true;

            Console.WriteLine($"Connections {from} - {to} is added successfully.");
            return true; // Connection successfully added
        }
        else
        {
            // Display an error message for servers that do not exist
            if (from_index == -1)
            {
                Console.WriteLine($"Error: Server '{from}' doesn't exist");
            }
            if (to_index == -1)
            {
                Console.WriteLine($"Error: Server '{to}' doesn't exist");
            }

            return false; // Connection not added
        }
    }

    // 10 marks
    // Return all servers that would disconnect the server graph into
    // two or more disjoint graphs if ever one of them would go down
    // Hint: Use a variation of the depth-first search
    public void DFS(int server,ref HashSet<int>visited)
    {
        visited.Add(server);
        for(int k = 0; k<NumServers; k++)
        {
            if (!visited.Contains(k) && E[k,server] == true)
            {
                DFS(k,ref visited);
            }
        }
    }
    public string[] CriticalServers()
    {
        // array of string of servers that are critical
        var critical_server = new List<string>();

        // removing a single server at a time and check if there are multiple components in the graph
        for(int i=0; i<NumServers; i++)
        {
            //Using the hash set instead of a standard bool array because it was giving a null value error which i couldn't fix
            //instantiate a hash set to check if the server is visited in the DFS
            // only the visited servers are added to the set
            HashSet<int> ?visited = new HashSet<int>();

            // marking the current server as visited as if it does not exist in the graph and hence it does not participate
            // in the DFS
            visited.Add(i);

            int num_components = 0;

            // applying DFS to the graph without the server i
            for(int j=0; j<NumServers; j++)
            {
                if (!visited.Contains(j) && E[i,j]==true)
                {
                    DFS(j, ref visited);
                    num_components++;
                }
            }

            //if multiple components are found, the name of the server is added to the list of critical servers
            if (num_components > 1)
            {
                critical_server.Add(V[i].Name);
            }
        }
        Console.WriteLine();
        Console.WriteLine("Critical Servers: ");
        return critical_server.ToArray();
    }

    // 6 marks
    // Return the shortest path from one server to another
    // Hint: Use a variation of the breadth-first search
    public int ShortestPath(string from, string to)
    {
        // Check if both servers exist
        int from_index = FindServer(from);
        int to_index = FindServer(to);

        if (from_index == -1 || to_index == -1)
        {
            Console.WriteLine("Error: One or more servers do not exist.");
            return -1;
        }

        // Initialization of distances dictionary
        Dictionary<WebServer, int> distances = new Dictionary<WebServer, int>();
        for (int i = 0; i < NumServers; i++)
        {
            distances[V[i]] = int.MaxValue;
        }

        // Set distance for the starting server to 0
        distances[V[from_index]] = 0;

        // Queue for BFS traversal
        Queue<WebServer> queue = new Queue<WebServer>();
        queue.Enqueue(V[from_index]);

        // BFS traversal
        while (queue.Count > 0)
        {
            WebServer current = queue.Dequeue();

            // Explore adjacent servers
            for (int i = 0; i < NumServers; i++)
            {
                if (E[FindServer(current.Name), i] && distances[current] + 1 < distances[V[i]])
                {
                    distances[V[i]] = distances[current] + 1;
                    queue.Enqueue(V[i]);

                    // Check if the destination server is reached
                    if (V[i].Name == to)
                    {
                        Console.WriteLine($"Shortest path from {from} to {to} is: ");
                        // Return the shortest path distance
                        return distances[V[i]];
                    }
                }
            }
        }

        // Check if the destination server is reachable
        Console.WriteLine("Error: No path found between the servers.");
        return -1;
    }


    // 4 marks
    // Print the name and connections of each server as well as
    // the names of the webpages it hosts
    public void PrintGraph()
    {
        //heading for the server graph
        Console.WriteLine("Server Graph: ");

        //Print the name of each server in the graph with their connectons:
        for (int i = 0; i < NumServers; i++)
        {
            Console.WriteLine($"Server: {V[i].Name}");

            //Print the connections for the server

            Console.WriteLine("Connections: ");

            for (int j = 0; j < NumServers; j++)
            {
                if (E[i, j] == true)
                    Console.Write($"{V[j].Name}, ");
            }
            Console.WriteLine();

            Console.WriteLine("Web Pages: ");

            foreach (WebPage web in V[i].P)
            {
                Console.Write($"{web.Name}, ");
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }

}

// WebPage class
// 5 marks
public class WebPage
{
    public string Name { get; set; }
    public string Server { get; set; }
    public List<WebPage> E { get; set; }

    // Constructor
    public WebPage(string name, string host)
    {
        Name = name;
        Server = host;
        E = new List<WebPage>();
    }

    // Find the index of a linked page by its name
    public int FindLink(string name)
    {
        for (int i = 0; i < E.Count; i++)
        {
            if (E[i].Name == name)
                return i;
        }
        return -1;
    }
}

// 2 marks
// Create an empty WebGraph
// WebGraph class
public class WebGraph
{
    private List<WebPage> P;

    public WebGraph()
    {
        P = new List<WebPage>();
    }
    public List<WebPage> GetPages()
    {
        return P;
    }

    // 2 marks
    // Return the index of the webpage with the given name; otherwise return -1
    private int FindPage(string name)
    {
        for (int i = 0; i < P.Count; i++)
        {
            if (P[i].Name == name)
                return i;
        }
        return -1;
    }
    // 4 marks
    // Add a webpage with the given name and store it on the host server
    // Return true if successful; otherwise return false
    public bool AddPage(string name, string host, ServerGraph S)
    {
        if (FindPage(name) != -1)
            return false;

        WebPage newPage = new WebPage(name, host);
        P.Add(newPage);

        // Add the webpage to the corresponding server in the ServerGraph
        S.AddWebPage(newPage, host);

        return true;
    }
    // 8 marks
    // Remove the webpage with the given name, including the hyperlinks
    // from and to the webpage
    // Return true if successful; otherwise return false
    public bool RemovePage(string name, ServerGraph S)
    {
        int page_index = FindPage(name);

        if (page_index == -1)
            return false;

        // Remove hyperlinks to the webpage
        for (int i = 0; i < P.Count; i++)
        {
            int link_index = P[i].FindLink(name);
            if (link_index != -1)
                P[i].E.RemoveAt(link_index);
        }

        // Remove the webpage from the ServerGraph
        S.RemoveWebPage(name, P[page_index].Server);

        // Remove the webpage from the WebGraph
        P.RemoveAt(page_index);

        return true;
    }

    // 3 marks
    // Add a hyperlink from one webpage to another
    // Return true if successful; otherwise return false
    public bool AddLink(string from, string to)
    {
        int from_index = FindPage(from);
        int to_index = FindPage(to);

        if (from_index != -1 && to_index != -1)
        {
            P[from_index].E.Add(P[to_index]);
            return true;
        }

        return false;
    }

    // 3 marks
    // Remove a hyperlink from one webpage to another
    // Return true if successful; otherwise return false
    public bool RemoveLink(string from, string to)
    {
        int from_index = FindPage(from);
        int to_index = FindPage(to);

        if (from_index != -1 && to_index != -1)
        {
            // Check if the link exists before attempting to remove
            if (P[from_index].E.Contains(P[to_index]))
            {
                P[from_index].E.Remove(P[to_index]);
                return true;  // Link successfully removed
            }
            else
            {
                // Link doesn't exist
                Console.WriteLine($"Error: Link from {from} to {to} does not exist.");
            }
        }

        return false;  // Link removal unsuccessful
    }
    // 6 marks
    // Return the average length of the shortest paths from the webpage with
    // given name to each of its hyperlinks
    // Hint: Use the method ShortestPath in the class ServerGraph
    public float AvgShortestPaths(string name, ServerGraph S)
    {
        // Find the index of the specified webpage in the WebGraph
        int page_index = FindPage(name);

        if (page_index == -1)
        {
            // Webpage not found in the WebGraph
            Console.WriteLine($"Error: Webpage '{name}' not found in the WebGraph.");
            return -1;
        }

        // Retrieve the specified webpage
        WebPage webpage = P[page_index];
        int total_Paths = 0;    // Total number of valid paths found
        int total_Length = 0;   // Total length of all valid paths combined

        int link_index = 0;
        while (link_index < webpage.E.Count)
        {
            // Iterate through each hyperlink of the specified webpage
            WebPage linked_Page = webpage.E[link_index];

            // Calculate the shortest path length from the specified webpage to its hyperlink
            int path_Length = S.ShortestPath(name, linked_Page.Name);

            if (path_Length != -1)
            {
                // If a valid path is found, update totalPaths and totalLength
                total_Paths++;
                total_Length += path_Length;
            }

            // Move to the next hyperlink
            link_index++;
        }

        // Check if there are valid paths
        if (total_Paths > 0)
        {
            // Calculate and return the average length of the valid paths
            return (float)total_Length / total_Paths;
        }
        else
        {
            // No valid paths found, return 0 to avoid division by zero
            return 0;
        }
    }

    // 3 marks
    // Print the name and hyperlinks of each webpage
    public void PrintGraph()
    {
        Console.WriteLine("Web Graph:");

        foreach (WebPage page in P)
        {
            Console.WriteLine($"WebPage: {page.Name}");
            Console.Write("Hyperlinks: ");
            foreach (WebPage link in page.E)
            {
                Console.Write($"{link.Name}, ");
            }
            Console.WriteLine("\n");
        }
    }
}

//Test Class
public class assignment1
{
    static void Main()
    {
        // Create an instance of ServerGraph
        ServerGraph serverGraph = new ServerGraph();

        // Create an instance of WebGraph
        WebGraph webGraph = new WebGraph();

        // Add servers to the ServerGraph
        serverGraph.AddServer("B", "A");
        serverGraph.AddServer("C", "A");
        serverGraph.AddServer("D", "B");
        serverGraph.PrintGraph();

        // testing the critical server methos
        string[] critical = serverGraph.CriticalServers();
        foreach(string s in critical)
        {
            Console.WriteLine(s);
        }

        //adding other connections in the graph
        serverGraph.AddConnection("D", "A");
        serverGraph.PrintGraph();

        //adding web pages to the servers
        WebPage page1 = new WebPage("page1", "A");
        serverGraph.AddWebPage(page1,"A");
        WebPage page2 = new WebPage("page2", "B");
        serverGraph.AddWebPage(page2, "B");
        WebPage page3 = new WebPage("page3", "D");
        serverGraph.AddWebPage(page3, "D");
        webGraph.AddPage("page4", "C", serverGraph);
        webGraph.AddPage("page5", "B", serverGraph);
        serverGraph.PrintGraph();

        //removing web pages from the server
        serverGraph.RemoveWebPage("page2", "B");
        serverGraph.PrintGraph();

        // finding the shortest path between two servers
        Console.WriteLine(serverGraph.ShortestPath("C", "D"));

        //remove server
        serverGraph.RemoveServer("A", "B");
        serverGraph.PrintGraph();

        //add hyperlinks to the server
        webGraph.AddLink("Page1", "Page2");
        webGraph.AddLink("Page2", "Page3");
        webGraph.AddLink("Page3", "Page1");
        webGraph.AddLink("page6", "page7");
        webGraph.AddLink("page7", "page8");
        webGraph.PrintGraph();

        //remove web page from the server
        webGraph.RemovePage("Page1", serverGraph);
        serverGraph.RemoveWebPage("page4", "C");
        serverGraph.PrintGraph();
        webGraph.PrintGraph();

        //removing hyperlink form the server
        Console.WriteLine("\nRemoving Hyperlink (page6 to page7):");
        webGraph.RemoveLink("page6", "page7");
        webGraph.PrintGraph();

        /*//average shortest distance to the hyperlinks of the webpage
        // Access ServerGraph's shortest paths
        int shortestPathAtoB = serverGraph.ShortestPath("A", "B");
        Console.WriteLine($"Shortest Path from A to B: {shortestPathAtoB}");

        // Calculate and print the average shortest paths using a for loop
        List<WebPage> pages = webGraph.GetPages();
        int pageCount = pages.Count;
        for (int i = 0; i < pageCount; i++)
        {
            WebPage page = pages[i];
            float avgShortestPaths = webGraph.AvgShortestPaths(page.Name, serverGraph);
            Console.WriteLine($"Average Shortest Paths for {page.Name}: {avgShortestPaths}");
        }*/
        Console.ReadKey();
    }
}



