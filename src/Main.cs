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
            server.enqueue("Test3.");
            server.getRange(0, 3);
            Console.ReadKey();
        }
    }
}