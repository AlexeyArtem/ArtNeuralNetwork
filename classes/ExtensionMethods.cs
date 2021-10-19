using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNeuralNetwork
{
    static class ExtensionMethods
    {
        public static TypesPaintings? GetTypesPaintings(this NeuralNetwork neuralNetwork, double[] inputs)
        {
            double result = neuralNetwork.GetResult(inputs);
            TypesPaintings? type;
            switch (result)
            {
                case double res when res <= 0.2:
                    type = TypesPaintings.Portrait;
                    break;

                case double res when res > 0.2 && res <= 0.7:
                    type = TypesPaintings.Landscape;
                    break;

                case double res when res > 0.7:
                    type = TypesPaintings.StillLife;
                    break;
                default:
                    type = null;
                    break;
            }

            return type;
        }
    }
}
