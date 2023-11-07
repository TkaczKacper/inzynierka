namespace server.Responses.Strava.ActivityStreams
{
    public class Streams
    {
        public string? type { get; set; }
        public object[]? data { get; set; }
        public string? series_type { get; set; }
        public int original_size { get; set; }
        public string? resolution { get; set; }
    }
}
