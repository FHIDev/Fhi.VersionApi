namespace Fhi.VersionApi
{
    public class VersionDTO
    {
        public string Environment { get; set; } = "";

        public string System { get; set; } = "";

        public string Component { get; set; } = "";

        public string Version { get; set; } = "";

        public string Status { get; set; } = "";

        public DateTime Date { get; set; }
    }
}