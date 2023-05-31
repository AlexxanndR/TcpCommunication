using MVVM;
using Receiver.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using TCP;

namespace Receiver.MVVM.ViewModel
{
    class InfoViewModel : ObservableObject
    {
        private const string _filePath = @"./files/messages.txt";

        private IP _ip;

        private TCPClient _tcpClient;

        private string _randomNumber;

        public string RandomNumber
        {
            get { return _randomNumber; }
            set
            {
                _randomNumber = value;
                OnPropertyChanged();
            }
        }

        private string _receivedTime;

        public string ReceivedTime
        {
            get { return _receivedTime; }
            set
            {
                _receivedTime = value;
                OnPropertyChanged();
            }
        }

        public string Hostname
        {
            get { return _ip.Hostname; }
            set
            {
                if (value != _ip.Hostname)
                {
                    _ip.Hostname = value;
                    OnPropertyChanged();
                }
            }
        }

        public uint Port
        {
            get { return _ip.Port; }
            set
            {
                if (value != _ip.Port)
                {
                    _ip.Port = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<Message> _messages;

        public InfoViewModel()
        {
            _ip = new IP();
            _tcpClient = new TCPClient();
            _messages = new List<Message>();

            _tcpClient.MessageReceived += MessageReceivedHandler;
        }

        private void WriteMessagesToFile()
        {
            if (_messages.Count == 0)
                return;

            string json = JsonSerializer.Serialize(_messages);
            File.WriteAllText(_filePath, json);
        }

        private void MessageReceivedHandler(string message)
        {
            RandomNumber = Regex.Match(message, @"\d+\,\d+", RegexOptions.Compiled).Value;
            ReceivedTime = DateTime.Now.ToString("HH:mm");

            _messages.Add(new Message 
            { 
                Number = Double.Parse(RandomNumber), 
                Time = ReceivedTime
            });

            WriteMessagesToFile();
        }

        public RelayCommand ConnectCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    try { _tcpClient.ConnectAsync(Hostname, Port); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }   
                });
            }
        }

        ~InfoViewModel() => WriteMessagesToFile();
    }
}
