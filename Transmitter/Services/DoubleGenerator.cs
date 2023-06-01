using System;
using System.Threading;
using System.Threading.Tasks;

namespace Transmitter.Services
{
    class DoubleGenerator
    {
        private int _timePeriod;

        private int _numberAccuracy;

        public double RandomNumber { private set; get; }

        public bool isStarted { private set; get; }

        private Thread _t;

        private object _lockObject;

        public DoubleGenerator()
        {
            _timePeriod = 0;
            _numberAccuracy = 2;

            isStarted = false;

            _t = new Thread(new ThreadStart(Generate));
            _lockObject = new object();
        }

        async public Task SetPropsAsync(int timePeriod, int numberAccuracy)
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    _timePeriod = timePeriod;
                    _numberAccuracy = numberAccuracy;
                }
            });
        }

        private void Generate()
        {
            var rand = new Random();

            while (true)
            {
                var rndNumber = new decimal(rand.NextDouble());

                lock (_lockObject)
                {
                    if (_timePeriod != 0)
                    {
                        Thread.Sleep(_timePeriod);
                        RandomNumber = Math.Round((double)(rndNumber * rand.Next(0, 1000)), _numberAccuracy);
                    }
                }
            }
        }

        public void StartGeneration() => _t.Start();

        public void TerminateGeneration() => _t.Abort();

        ~DoubleGenerator() => TerminateGeneration();
    }
}
