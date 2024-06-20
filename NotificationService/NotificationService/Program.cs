using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var notificationService = host.Services.GetRequiredService<NotificationService>();

        // Testen Sie die E-Mail-Funktionalität
        await notificationService.SendEmailAsync("test@example.com", "Test Subject", "Test Body");

        // Testen Sie die SMS-Funktionalität
        notificationService.SendSms("+1234567890", "Test SMS Message");

        // Testen Sie die WhatsApp-Funktionalität
        notificationService.SendWhatsApp("+1234567890", "Test WhatsApp Message");

        // Testen Sie die Voice-Nachrichten-Funktionalität
        notificationService.SendVoiceMessage("+1234567890", "http://demo.twilio.com/docs/voice.xml");
        /*
        // Testen Sie die Slack-Funktionalität
        await notificationService.SendSlackMessageAsync("Test Slack Message");

        // Testen Sie die PagerDuty-Funktionalität
        await notificationService.SendPagerDutyEventAsync("Test PagerDuty Event");

        // Testen Sie die OpsGenie-Funktionalität
        await notificationService.SendOpsGenieAlertAsync("Test OpsGenie Alert");

        // Testen Sie die VictorOps-Funktionalität
        await notificationService.SendVictorOpsAlertAsync("Test VictorOps Alert");
        */
        // Testen Sie die Telegram-Funktionalität
        await notificationService.SendTelegramMessageAsync("Test Telegram Message");

        // Testen Sie die Push-Nachrichten-Funktionalität
        await notificationService.SendPushNotificationAsync("Test Push Notification");
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            /*.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext())*/
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<NotificationService>();
            });
}
