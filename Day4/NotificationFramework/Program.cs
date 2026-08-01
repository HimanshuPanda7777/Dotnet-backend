using System;
using System.Collections.Generic;

namespace NotificationFramework
{
    // Interface for Notification Channels
    public interface INotificationChannel
    {
        void Send(string message);
        string ChannelName { get; }
        bool Status { get; set; }
    }

    // Concrete Channels
    public class Email : INotificationChannel
    {
        public string ChannelName => "Email";
        public bool Status { get; set; }

        public void Send(string message)
        {
            Console.WriteLine($"[Email] Sending: {message}");
            Status = true; // Mark as successful
        }
    }

    public class SMS : INotificationChannel
    {
        public string ChannelName => "SMS";
        public bool Status { get; set; }

        public void Send(string message)
        {
            Console.WriteLine($"[SMS] Sending: {message}");
            Status = true;
        }
    }

    public class WhatsApp : INotificationChannel
    {
        public string ChannelName => "WhatsApp";
        public bool Status { get; set; }

        public void Send(string message)
        {
            Console.WriteLine($"[WhatsApp] Sending: {message}");
            Status = true;
        }
    }
    
    public class PushNotification : INotificationChannel
    {
        public string ChannelName => "Push Notification";
        public bool Status { get; set; }

        public void Send(string message)
        {
            Console.WriteLine($"[Push Notification] Sending: {message}");
            Status = true;
        }
    }

    // Notification Manager using Dependency Injection Principles
    public class NotificationManager
    {
        public void Send(string message, params INotificationChannel[] channels)
        {
            Console.WriteLine($"--- Sending Notification: '{message}' ---");
            foreach (var channel in channels)
            {
                try
                {
                    channel.Send(message);
                    Console.WriteLine($"Status for {channel.ChannelName}: {(channel.Status ? "Success" : "Failed")}");
                }
                catch (Exception ex)
                {
                    channel.Status = false;
                    Console.WriteLine($"Status for {channel.ChannelName}: Failed - {ex.Message}");
                }
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var manager = new NotificationManager();

            // Sending to multiple channels simultaneously
            manager.Send(
                "System Update Scheduled for Midnight.",
                new Email(),
                new WhatsApp(),
                new SMS()
            );
            
            // Tomorrow we can easily add Slack without modifying existing manager
            // manager.Send("Alert", new Slack(), new Email());
        }
    }
}
