using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using SlackAPI;
//using PagerDuty.ApiClient;
//using OpsGenie.Client;
//using VictorOps.Client;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using PagerDuty;

public class NotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation("Email sent to {ToEmail} with status code {StatusCode}", toEmail, response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
        }
    }

    public void SendSms(string toPhoneNumber, string message)
    {
        try
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:PhoneNumber"];

            TwilioClient.Init(accountSid, authToken);

            var messageResource = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(fromNumber),
                to: new Twilio.Types.PhoneNumber(toPhoneNumber)
            );

            _logger.LogInformation("SMS sent to {ToPhoneNumber} with SID {Sid}", toPhoneNumber, messageResource.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {ToPhoneNumber}", toPhoneNumber);
        }
    }

    public void SendWhatsApp(string toWhatsAppNumber, string message)
    {
        try
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:WhatsAppNumber"];

            TwilioClient.Init(accountSid, authToken);

            var messageResource = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(fromNumber),
                to: new Twilio.Types.PhoneNumber("whatsapp:" + toWhatsAppNumber)
            );

            _logger.LogInformation("WhatsApp message sent to {ToWhatsAppNumber} with SID {Sid}", toWhatsAppNumber, messageResource.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending WhatsApp message to {ToWhatsAppNumber}", toWhatsAppNumber);
        }
    }

    public void SendVoiceMessage(string toPhoneNumber, string messageUrl)
    {
        try
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:PhoneNumber"];

            TwilioClient.Init(accountSid, authToken);

            var call = CallResource.Create(
                to: new Twilio.Types.PhoneNumber(toPhoneNumber),
                from: new Twilio.Types.PhoneNumber(fromNumber),
                url: new Uri(messageUrl)
            );

            _logger.LogInformation("Voice call initiated to {ToPhoneNumber} with SID {Sid}", toPhoneNumber, call.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating voice call to {ToPhoneNumber}", toPhoneNumber);
        }
    }

    /*
    public async Task SendSlackMessageAsync(string message)
    {
        try
        {
            var webhookUrl = _configuration["Slack:WebhookUrl"];

            var slackClient = new SlackClient(webhookUrl);
            slackClient.PostMessage(response => _logger.LogInformation("Slack message response: {Response}", response), message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Slack message");
        }
    }

    public async Task SendPagerDutyEventAsync(string description)
    {
        try
        {
            var apiKey = _configuration["PagerDuty:ApiKey"];
            var routingKey = _configuration["PagerDuty:RoutingKey"];

            var client = new PagerDutyClient(apiKey);
            var pdEvent = new PagerDutyEvent
            {
                RoutingKey = routingKey,
                EventAction = "trigger",
                Payload = new Payload
                {
                    Summary = description,
                    Severity = "info"
                }
            };

            await client.EnqueueEventAsync(pdEvent);

            _logger.LogInformation("PagerDuty event sent: {Description}", description);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending PagerDuty event");
        }
    }

    public async Task SendOpsGenieAlertAsync(string message)
    {
        try
        {
            var apiKey = _configuration["OpsGenie:ApiKey"];

            var client = new OpsGenieClient(apiKey);
            var alert = new CreateAlertRequest
            {
                Message = message,
                Alias = "example-alert",
                Description = "Alert description"
            };

            await client.CreateAlertAsync(alert);

            _logger.LogInformation("OpsGenie alert sent: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OpsGenie alert");
        }
    }

    public async Task SendVictorOpsAlertAsync(string message)
    {
        try
        {
            var apiKey = _configuration["VictorOps:ApiKey"];

            var client = new VictorOpsClient(apiKey);
            var alert = new VictorOpsAlert
            {
                MessageType = "CRITICAL",
                EntityId = "example-entity",
                StateMessage = message
            };

            await client.SendAlertAsync(alert);

            _logger.LogInformation("VictorOps alert sent: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending VictorOps alert");
        }
    }
    */
    public async Task SendTelegramMessageAsync(string message)
    {
        try
        {
            var botToken = _configuration["Telegram:BotToken"];
            var chatId = _configuration["Telegram:ChatId"];

            var botClient = new TelegramBotClient(botToken);
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html
            );

            _logger.LogInformation("Telegram message sent to chat ID {ChatId}", chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Telegram message");
        }
    }

    public async Task SendPushNotificationAsync(string message)
    {
        try
        {
            var signalRUrl = _configuration["SignalR:Url"];

            var connection = new HubConnectionBuilder()
                .WithUrl(signalRUrl)
                .Build();

            await connection.StartAsync();
            await connection.InvokeAsync("SendMessage", message);

            _logger.LogInformation("Push notification sent: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification");
        }
    }
}
