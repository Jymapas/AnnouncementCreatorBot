namespace AnnouncementCreatorBot
{
    internal class Program
    {
        // const string Base = "https://api.rating.chgk.net";
        
        static void Main()
        {
            // var request = new GetRequest($"{Base}/tournaments/8147");
            // request.Run();
            // Console.ReadLine();

            var connect = new Connect();
            connect.Start();

            Console.ReadLine();
        }
    }
}