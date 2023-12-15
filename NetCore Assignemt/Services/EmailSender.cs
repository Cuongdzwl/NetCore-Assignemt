using Humanizer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetCore_Assignemt.Controllers;
using NetCore_Assignemt.Data;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using SendGrid.Helpers.Mail.Model;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace NetCore_Assignemt.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly AppDbContext _context;

        public EmailSender(ILogger<EmailSender> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async System.Threading.Tasks.Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiInstance = new TransactionalEmailsApi();
            // Sender
            string SenderName = "FPT Book Supporter";
            string SenderEmail = "cuongnguyen.public@gmail.com";
            SendSmtpEmailSender Email = new SendSmtpEmailSender(SenderName, SenderEmail);
            // Reciever
            var user = await _context.Users.Where(c => c.Email == email).FirstOrDefaultAsync();
            string name = user.UserName.ToString();
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(email, name);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);
            string HtmlContent = htmlMessage;
            string Subject = subject;

            // Other
            object? Headers = null;
            string? TextContent = null;
            long? TemplateId = null;
            List<SendSmtpEmailBcc>? Bcc = null;
            List<SendSmtpEmailCc>? Cc = null;
            SendSmtpEmailReplyTo? ReplyTo = null;
            List<SendSmtpEmailAttachment>? Attachment = null;
            List<string>? Tags = null;
            List<SendSmtpEmailMessageVersions>? messageVersiopns = null;
            object? Params = null;
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, Bcc, Cc, HtmlContent, TextContent, Subject, ReplyTo, Attachment, Headers, TemplateId, Params, messageVersiopns, Tags);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                _logger.LogInformation(result.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString()); 
            }
        }
    }
}
