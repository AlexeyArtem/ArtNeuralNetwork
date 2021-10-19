using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNeuralNetwork
{
    [Serializable]
    class Relation
    {
        private static Random random = new Random();

        public Relation(Neuron input, Neuron output)
        {
            Input = input;
            Output = output;
            Weight = random.NextDouble();
        }

        public Neuron Input { get; }
        public Neuron Output { get; }
        public double Weight { get; set; }
    }
}
