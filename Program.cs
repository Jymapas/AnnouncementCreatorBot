namespace AnnouncementCreatorBot
{
    internal class Program
    {
        static void Main()
        {
            var connect = new Connect();
            connect.Start();

            Console.ReadLine();
        }
    }
}