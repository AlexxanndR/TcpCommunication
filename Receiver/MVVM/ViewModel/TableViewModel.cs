using MVVM;
using Receiver.MVVM.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Receiver.MVVM.ViewModel
{
    class TableViewModel : ObservableObject
    {
        private readonly string _txtFilePath = @"./files/messages.txt";
        private readonly string _csvFilePath = @"./files/messages.csv";

        private List<Message> _messages;

        public List<Message> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        public TableViewModel()
        {
            _txtFilePath = @"./files/messages.txt";
            _csvFilePath = @"./files/messages.csv";

            _messages = new List<Message>();

            if (!File.Exists(_txtFilePath))
            {
                MessageBox.Show("There are no data to analyze yet.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ReadMessagesFromFile();
        }

        private void ReadMessagesFromFile()
        {
            string json = File.ReadAllText(_txtFilePath);
            Messages = JsonSerializer.Deserialize<List<Message>>(json);
        }

        public RelayCommand ExportCommand
        {
            get
            {
                return new RelayCommand((obj) => 
                {
                    if (!File.Exists(_txtFilePath))
                        return;

                    using (StreamWriter writer = new StreamWriter(_csvFilePath))
                    {
                        string header = string.Join(",", typeof(Message).GetProperties().Select(p => p.Name));
                        writer.WriteLine(header);

                        foreach (var message in Messages)
                        {
                            string line = string.Join(",", message.Number, message.Time);
                            writer.WriteLine(line);
                        }
                    }
                });
            }
        }
    }
}
