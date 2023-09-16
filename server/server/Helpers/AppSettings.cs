namespace server.Helpers
{
    public class AppSettings
    {
        public required string Secret { get; set; }

        // time to live for refresh token, inactive tokens are
        // automatically deleted from the database after this time
        public int RefreshTokenTTL { get; set; }
    }
}
