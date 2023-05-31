using System;
using System.Threading;

namespace Transmitter.Services
{
    class DoubleGenerator
    {
        private int _timePeriod;

        private int _numberAccuracy;

        public double RandomNumber { private set; get; }

        private Thread _t;

        public DoubleGenerator()
        {
            _timePeriod = 0;
            _numberAccuracy = 2;

            _t = new Thread(new ThreadStart(Generate));
        }

        public void SetProps(int timePeriod, int numberAccuracy)
        {
            _timePeriod = timePeriod;
            _numberAccuracy = numberAccuracy;
        }

        private void Generate()
        {
            var rand = new Random();
            var rndNumber = new decimal(rand.NextDouble());

            while (true)
            {
                if (_timePeriod != 0)
                {
                    Thread.Sleep(_timePeriod);
                    RandomNumber = Math.Round((double)(rndNumber * rand.Next(0, 1000)), _numberAccuracy);
                }
            }
        }

        public void StartGeneration() => _t.Start();

        public void TerminateGeneration() => _t.Abort();

        ~DoubleGenerator() => TerminateGeneration();
    }
}
