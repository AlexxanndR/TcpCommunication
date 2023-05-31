using MVVM;
using Receiver.MVVM.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Receiver.MVVM.ViewModel
{
    class TableViewModel : ObservableObject
    {
        private const string txtFilePath = @"./files/messages.txt";
        private const string csvFilePath = @"./files/messages.csv";

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
            ReadMessagesFromFile();
        }

        private void ReadMessagesFromFile()
        {
            string json = File.ReadAllText(txtFilePath);
            Messages = JsonSerializer.Deserialize<List<Message>>(json);
        }

        public RelayCommand ExportCommand
        {
            get
            {
                return new RelayCommand((pageName) => 
                {
                    using (StreamWriter writer = new StreamWriter(csvFilePath))
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
