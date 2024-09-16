namespace TulosAPI.Services
{
    public class MailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool EnableSsl { get; set; }
        public string ApiToken { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
