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

            HiddenLayers = new List<List<Neuron>>();
            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                HiddenLayers.Add(new List<Neuron>());
                for (int j = 0; j < hiddenLayers[i]; j++)
                    HiddenLayers[i].Add(new Neuron());
            }

            Relations = new Dictionary<Neuron, List<Relation>>();
            CreateRelations();
        }

        public List<Neuron> InputLayer { get; }
        public List<List<Neuron>> HiddenLayers { get; }
        public List<Neuron> OutputLayer { get; }

        //Словарь связей, где ключ - нейрон, а значение - список входных связей в этот нейрон
        public Dictionary<Neuron, List<Relation>> Relations { get; }

        private void CreateRelations() 
        {
            List<List<Neuron>> totalLayers = new List<List<Neuron>>();
            totalLayers.Add(InputLayer);
            totalLayers = totalLayers.Concat(HiddenLayers).ToList();
            totalLayers.Add(OutputLayer);

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
                    Relations.Add(nOutput, relations);
                }
            }
        }

        private double[] GetNormalizeData(double[] data, double min, double max)
        {
            double[] result = new double[data.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 2 * (data[i] - min) / (max - min) - 1;
            }

            return result;
        }

        private double[] SoftMax(double[] data) 
        {
            double[] p = new double[data.Length];
            for (int i = 0; i < p.Length; i++)
            {
                double sum = 0;
                foreach (double value in data)
                    sum += Math.Exp(value);
                p[i] = Math.Exp(data[i]) / sum;
            }

            return p;
        }

        //Прямое распространение
        private List<double> InitializeForwardPropagation(double[] inputs) 
        {
            if (inputs.Length != InputLayer.Count) throw new Exception("Количество входных значений не совпадает с количеством входных нейронов");

            for (int i = 0; i < inputs.Length; i++)
            {
                InputLayer[i].Value = inputs[i];
            }

            foreach (Neuron n in Relations.Keys)
            {
                double sum = 0;
                foreach (Relation r in Relations[n])
                {
                    sum += r.Input.Value * r.Weight;
                }

                if (OutputLayer.Contains(n))
                {
                    n.Value = sum;
                    continue;
                }

                n.Activate(sum);
            }

            List<double> result = new List<double>();
            foreach (Neuron n in OutputLayer)
                result.Add(n.Value);

            //Преобразование выходных данных в вероятности
            //double[] p = SoftMax(result.ToArray()); //Вектор вероятностей

            //for (int i = 0; i < p.Length; i++)
            //{
            //    OutputLayer[i].Value = p[i];
            //}

            return result;
        }

        //Обратное распространение
        private void InitializeBackPropagation(int expectedResult)
        {
            //double[] expectedP = new double[OutputLayer.Count];
            //for (int i = 0; i < expectedP.Length; i++)
            //{
            //    double value = 0;
            //    if (i == expectedResult - 1) value = 1;
                
            //    expectedP[i] = value;
            //}

            //for (int i = 0; i < expectedP.Length; i++)
            //{
            //    OutputLayer[i].MeanError = expectedP[i] - OutputLayer[i].Value;
            //}

            foreach (Neuron n in OutputLayer)
            {
                n.MeanError = expectedResult - n.Value;
            }

            for (int i = HiddenLayers.Count - 1; i >= 0; i--)
            {
                //Перебор количества нейронов в каждом слое
                foreach (Neuron nInput in HiddenLayers[i]) 
                {
                    //Получение связей, где nInput является входом
                    var outputRelations = Relations.Values.SelectMany(relations => relations.Where(r => r.Input == nInput));

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

        //Корректировка весов
        private void AdjustWeights()
        {
            foreach (Neuron n in Relations.Keys) 
            {
                foreach (Relation r in Relations[n]) 
                {
                    r.Weight = r.Weight + n.MeanError * n.Value * (1 - n.Value) * r.Input.Value;
                }
            }
        }

        //Получение результата, т.е. номера класса классифицируемых объектов
        public int GetResult(double[] inputs) 
        {
            List<double> resultValues = InitializeForwardPropagation(inputs);
            
            if (resultValues.Count == 1)
            {
                int resultValue = (int)Math.Round(resultValues[0], MidpointRounding.AwayFromZero);
                return resultValue;
            }
            else 
            {
                int resultClass = resultValues.IndexOf(resultValues.Max());
                return resultClass + 1;
            }
        }

        //Метод обучения нейронной сети по заданному набору данных
        //На вход словарь, где ключ - массив входных параметров, а значение - ожидаемый результат для заданных входных параметров
        public void Train(Dictionary<double[], int> dataSet) 
        {
            foreach (var inputs in dataSet.Keys) 
            {
                int expectedResult = dataSet[inputs];
                
                InitializeForwardPropagation(inputs);
                InitializeBackPropagation(expectedResult);
                AdjustWeights();
            }
        }
    }
}
