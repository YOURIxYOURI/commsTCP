﻿using System.Net.Sockets;
using System.Net;
using System.Text;

namespace commsTCP;

public partial class MainPage : ContentPage
{
    bool Clicked = false;
    TcpListener server;
    TcpClient client;
    NetworkStream serverStream;

    public MainPage()
    {
        InitializeComponent();
        InitializeTCP();
    }

    void InitializeTCP()
    {
        // w celu sprawdzenia działąnia stworzyć drugą instancje programu z portami ustawionymi na odwrót
        server = new TcpListener(IPAddress.Any, 12345); // w drugiej instancji port 12345 zmienić na 54321
        server.Start();

        Task.Run(() => StartServer());

       
    }
    async Task StartServer()
    {
        while (true)
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            await Task.Run(() => HandleClient(client));
        }
    }

    [Obsolete]
    async Task StartClient()
    {
        while (true)
        {
            byte[] data = new byte[256];
            int bytes = await serverStream.ReadAsync(data, 0, data.Length);
            string message = Encoding.ASCII.GetString(data, 0, bytes);

            Device.BeginInvokeOnMainThread(() =>
            {
                ChatMessagesStack.Children.Add(new Label { Text = "Otrzymano: " + message });
            });
        }
    }
    async void Connect_Clicked(object sender, EventArgs e)
    {
        if (!Clicked)
        {
            client = new TcpClient();
            client.Connect("127.0.0.1", 54321);  // w drugiej instancji port 54321 zmienić na 12345
            serverStream = client.GetStream();

            Task.Run(() => StartClient());
        }
    }

    [Obsolete]
    async Task HandleClient(TcpClient client)
    {
        using (NetworkStream stream = client.GetStream())
        {
            while (true)
            {
                byte[] data = new byte[256];
                int bytes = await stream.ReadAsync(data, 0, data.Length);
                string message = Encoding.ASCII.GetString(data, 0, bytes);

                Device.BeginInvokeOnMainThread(() =>
                {
                    ChatMessagesStack.Children.Add(new Label { Text = "Otrzymano: " + message });
                });
            }
        }
    }

    [Obsolete]
    async void SendMessage_Clicked(object sender, EventArgs e)
    {
        string message = MessageEntry.Text;

        if (!string.IsNullOrWhiteSpace(message))
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            await serverStream.WriteAsync(data, 0, data.Length);

            Device.BeginInvokeOnMainThread(() =>
            {
                ChatMessagesStack.Children.Add(new Label { Text = "Wysłano: " + message });
            });
        }
        MessageEntry.Text = string.Empty;
    }
    
}



