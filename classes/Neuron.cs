using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNeuralNetwork
{
    [Serializable]
    class Neuron
    {
        private static readonly double a = 0.5;
        
        public Neuron() { }

        public double Value { get; set; }
        public double MeanError { get; set; }

        public void Activate(double input) 
        {
            Value = 1 / (1 + Math.Exp(-a * input));
        }
    }
}
