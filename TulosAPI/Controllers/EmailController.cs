using Microsoft.AspNetCore.Mvc;
using TulosAPI.Services;

namespace TulosAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly EmailSender _emailSender;

        public EmailController(EmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("SendMailAsync")]
        public async Task<bool> SendMailAsync(MailData mailData)
        {
            return await _emailSender.SendMailAsync(mailData);
        }
    }
}
