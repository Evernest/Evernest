using System;

namespace Cloud14
{
    class Test
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.enqueue("Test1.");
            server.enqueue("Test2.");
            server.getID(0);
            server.getID(1);
            Console.ReadKey();
        }
    }
}