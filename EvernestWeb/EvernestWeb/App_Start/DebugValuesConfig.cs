using EvernestFront;

namespace EvernestWeb
{
    public class DebugValuesConfig
    {
        public static void Init()
        {
            Users.AddUser("testing");
            //Process.CreateStream("testing", "coucou");
            //Process.Push("testing", "coucou", "test");
        }
    }
}