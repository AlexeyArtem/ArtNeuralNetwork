using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNeuralNetwork
{
    class NeuralNetwork
    {
        public NeuralNetwork(int countInputNeurons, int countOutputNeurons, params int[] hiddenLayers)
        {
            InputLayer = new List<Neuron>();
            for (int i = 0; i < countInputNeurons; i++)
                InputLayer.Add(new Neuron());

            OutputLayer = new List<Neuron>();
            for (int i = 0; i < countOutputNeurons; i++)
                OutputLayer.Add(new Neuron());

            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                HiddenLayers.Add(new List<Neuron>());
                for (int j = 0; j < hiddenLayers[i]; j++)
                    HiddenLayers[i].Add(new Neuron());
            }

            Relations = new List<Relation>();
            CreateRelations();
        }

        public List<Neuron> InputLayer { get; }
        public List<List<Neuron>> HiddenLayers { get; }
        public List<Neuron> OutputLayer { get; }
        public List<Relation> Relations { get; }

        private void CreateRelations() 
        {
            List<List<Neuron>> totalList = new List<List<Neuron>>();
            totalList.Add(InputLayer);
            totalList = totalList.Concat(HiddenLayers).ToList();
            totalList.Add(OutputLayer);

            for (int i = 0; i < totalList.Count; i++)
            {
                for (int j = 0; j < totalList[i].Count; j++)
                {
                    if (j == totalList[i].Count - 1) break;

                    foreach (Neuron nInput in totalList[i]) 
                    {
                        foreach (Neuron nOutput in totalList[i + 1])
                            Relations.Add(new Relation(nInput, nOutput));
                    }
                }
            }
        }

        public double GetResult(double[] inputs) 
        {
            return double.NaN;
        }

        public void Train(double[,] dataSet) 
        {

        }
    }
}
