using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNeuralNetwork
{
    class Neuron
    {
        public Neuron(double value)
        {
            Value = value;
        }

        public Neuron() { }

        public double Value { get; set; }

        public void Activate(double input) 
        {
            if (input >= 0) Value = 1;
            else Value = 0;
        }
    }
}
