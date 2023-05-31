using MVVM;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using TCP;
using Transmitter.MVVM.Model;
using Transmitter.Services;

namespace Transmitter.MVVM.ViewModel
{
    class TransmitterViewModel : ObservableObject
    {
        private IP _ip;

        private TCPServer _tcpServer;

        private DoubleGenerator _doubleGenerator;

        private double _randomNumber;
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

        private int _timePeriod;

        public int TimePeriod
        {
            get { return _timePeriod; }
            set
            {
                _timePeriod = value;
                OnPropertyChanged();
            }
        }

        private int _accuracy;

        public int Accuracy
        {
            get { return _accuracy; }
            set
            {
                _accuracy = value;
                OnPropertyChanged();
            }
        }

        public TransmitterViewModel()
        {
            _ip = new IP();
            _tcpServer = new TCPServer();
            _doubleGenerator = new DoubleGenerator();

            _doubleGenerator.StartGeneration();
        }

        public RelayCommand SendMessageCommand
        {
            get
            {
                return new RelayCommand(async (obj) =>
                {
                    _randomNumber = _doubleGenerator.RandomNumber;
                    string message = String.Format("$value={0}\r\n", _randomNumber);

                    try { await _tcpServer.SendMessageAsync(message); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                });
            }
        }

        public RelayCommand ConnectCommand
        {
            get
            {
                return new RelayCommand(async (obj) =>
                {
                    try { await _tcpServer.ListenAsync(Hostname, Port); }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    
                });
            }
        }

        public RelayCommand SetGeneratorPropsCommand
        {
            get
            {
                return new RelayCommand((obj) => _doubleGenerator.SetProps(TimePeriod, Accuracy));
            }
        }
    }
}
