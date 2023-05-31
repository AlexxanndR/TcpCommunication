using MVVM;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Receiver.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace Receiver.MVVM.ViewModel
{
    class GraphViewModel : ObservableObject
    {
        private const string _filePath = @"./files/messages.txt";

        private List<Message> _messages;

        private PlotModel _chartModel;

        public PlotModel ChartModel
        {
            get { return _chartModel; }
            set
            {
                _chartModel = value;
                OnPropertyChanged();
            }
        }

        private LineSeries _lineSeries;

        private string _timePeriod;

        public string TimePeriod
        {
            get { return _timePeriod; ; }
            set
            {
                _timePeriod = value;
                OnPropertyChanged();
            }
        }

        public GraphViewModel()
        {
            ReadMessagesFromFile();
            ConfigureChart();
        }

        private void ReadMessagesFromFile()
        {
            string json = File.ReadAllText(_filePath);
            _messages = JsonSerializer.Deserialize<List<Message>>(json);
        }

        private void ConfigureChart()
        {
            ChartModel = new PlotModel();

            _lineSeries = new LineSeries();

            foreach (var message in _messages)
            {
                DateTime time = DateTime.ParseExact(message.Time, "HH:mm", CultureInfo.InvariantCulture);
                _lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), message.Number));
            }

            ChartModel.Series.Add(_lineSeries);

            var _dateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "HH:mm",
                Title = "Time"
            };

            var _linearAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Number"
            };

            ChartModel.Axes.Add(_dateTimeAxis);
            ChartModel.Axes.Add(_linearAxis);
        }

        public RelayCommand SetTimePeriodCommand
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    if (!Regex.IsMatch(TimePeriod, @"^\d{2}\:\d{2} - \d{2}\:\d{2}$"))
                    {
                        MessageBox.Show("Incorrect time period format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    PlotModel newModel = new PlotModel();

                    ChartModel.Series.Remove(_lineSeries);
                    newModel.Series.Add(_lineSeries);

                    var matches = Regex.Matches(TimePeriod, @"\d+\:\d+", RegexOptions.Compiled);

                    var minTime = DateTime.ParseExact(matches[0].Value, "HH:mm", CultureInfo.InvariantCulture);
                    var doubleTimeMin = DateTimeAxis.ToDouble(minTime);

                    var maxTime = DateTime.ParseExact(matches[1].Value, "HH:mm", CultureInfo.InvariantCulture);
                    var doubleTimeMax = DateTimeAxis.ToDouble(maxTime);

                    var _dateTimeAxis = new DateTimeAxis
                    {
                        Position = AxisPosition.Bottom,
                        StringFormat = "HH:mm",
                        Minimum = doubleTimeMin,
                        Maximum = doubleTimeMax,
                        Title = "Time"
                    };

                    var _linearAxis = new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Title = "Number"
                    };

                    newModel.Axes.Add(_dateTimeAxis);
                    newModel.Axes.Add(_linearAxis);

                    ChartModel = newModel;
                });
            }
        }
    }
}
