namespace AnnouncementCreatorBot
{
    public class Announcement
    {
        public Announcement() { }
        public string tournamentName { get; init; }
        public List<string> editors {get; init; }
        public string eventDate { get; init; }
        public string regTime {get; init; }
        public string startTime { get; init; }
        public string narrator { get; init; }
        public string narratorsSex { get; init; }
        public int payment { get; init; }
        public int toursCount { get; init; }
        public int questionsInTour { get; init; }
    }
}
