using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNeuralNetwork
{
    [Serializable]
    public class NeuralNetwork
    {
        private List<Neuron> inputLayer;
        private List<List<Neuron>> hiddenLayers;
        private List<Neuron> outputLayer;
        private Dictionary<Neuron, List<Relation>> relations;

        public NeuralNetwork(int countInputNeurons, int countOutputNeurons, params int[] hiddenLayers)
        {
            inputLayer = new List<Neuron>();
            for (int i = 0; i < countInputNeurons; i++)
                inputLayer.Add(new Neuron());

            outputLayer = new List<Neuron>();
            for (int i = 0; i < countOutputNeurons; i++)
                outputLayer.Add(new Neuron());

            this.hiddenLayers = new List<List<Neuron>>();
            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                this.hiddenLayers.Add(new List<Neuron>());
                for (int j = 0; j < hiddenLayers[i]; j++)
                    this.hiddenLayers[i].Add(new Neuron());
            }

            relations = new Dictionary<Neuron, List<Relation>>();
            CreateRelations();
        }

        private void CreateRelations() 
        {
            List<List<Neuron>> totalLayers = new List<List<Neuron>>();
            totalLayers.Add(inputLayer);
            totalLayers = totalLayers.Concat(hiddenLayers).ToList();
            totalLayers.Add(outputLayer);

            for (int i = 0; i < totalLayers.Count; i++)
            {
                if (i == totalLayers.Count - 1) break;

                foreach (Neuron nOutput in totalLayers[i + 1])
                {
                    List<Relation> relations = new List<Relation>();
                    foreach (Neuron nInput in totalLayers[i])
                    {
                        relations.Add(new Relation(nInput, nOutput));
                    }
                    this.relations.Add(nOutput, relations);
                }
            }
        }

        private void InitializeForwardPropagation(double[] inputs) 
        {
            if (inputs.Length != inputLayer.Count) throw new Exception("Количество входных значений не совпадает с количеством входных нейронов");

            for (int i = 0; i < inputs.Length; i++)
            {
                inputLayer[i].Value = inputs[i];
            }

            foreach (Neuron n in relations.Keys)
            {
                double sum = 0;
                foreach (Relation r in relations[n])
                {
                    sum += r.Input.Value * r.Weight;
                }

                n.Activate(sum);
            }
        }

        private void InitializeBackPropagation(double expectedResult)
        {
            foreach (Neuron n in outputLayer)
            {
                n.MeanError = expectedResult - n.Value;
            }

            for (int i = hiddenLayers.Count - 1; i >= 0; i--)
            {
                //Перебор количества нейронов в каждом слое
                foreach (Neuron nInput in hiddenLayers[i]) 
                {
                    //Получение связей, где nInput является входом
                    var outputRelations = relations.Values.SelectMany(relations => relations.Where(r => r.Input == nInput));

                    //Вычисление погрешности для nInput
                    double D = 0;
                    foreach (Relation r in outputRelations)
                    {
                        D += r.Weight * r.Output.MeanError;
                    }
                    nInput.MeanError = D;
                }
            }
        }

        private void AdjustWeights()
        {
            foreach (Neuron n in relations.Keys) 
            {
                foreach (Relation r in relations[n]) 
                {
                    r.Weight = r.Weight + n.MeanError * n.Value * (1 - n.Value) * r.Input.Value;
                }
            }
        }

        public double GetResult(double[] inputs) 
        {
            InitializeForwardPropagation(inputs);
            if (outputLayer.Count > 1)
            {
                return outputLayer.IndexOf(outputLayer.Max());
            }
            else 
            {
                return outputLayer[0].Value;
            }
        }

        public void Training(Dictionary<double[], double> dataSet, int epochs = 1) 
        {
            if (dataSet == null) return;

            for (int i = 0; i < epochs; i++)
            {
                foreach (var inputs in dataSet.Keys)
                {
                    double expectedResult = dataSet[inputs];

                    InitializeForwardPropagation(inputs);
                    InitializeBackPropagation(expectedResult);
                    AdjustWeights();
                }
            }
        }
    }
}
