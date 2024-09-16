using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace TulosAPI.Services
{
    public interface IEmailSender
    {
        Task<bool> SendMailAsync(MailData mailData);
    }

public class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        private readonly HttpClient _httpClient;

        public EmailSender(IOptions<MailSettings> mailSettingsOptions, IHttpClientFactory httpClientFactory)
        {
            _mailSettings = mailSettingsOptions.Value;
            _httpClient = httpClientFactory.CreateClient("MailTrapApiClient");
        }

        public async Task<bool> SendMailAsync(MailData mailData)
        {
            var apiEmail = new
            {
                From = new { Email = _mailSettings.FromEmail, Name = _mailSettings.FromEmail },
                To = new[] { new { Email = mailData.ToEmail, Name = mailData.ToName } },
                Subject = mailData.Subject,
                Text = mailData.Body
            };

            var httpResponse = await _httpClient.PostAsJsonAsync("send", apiEmail);

            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            if (response != null && response.TryGetValue("success", out object? success) && success is bool boolSuccess && boolSuccess)
            {
                return true;
            }

            return false;
        }
    }
}
