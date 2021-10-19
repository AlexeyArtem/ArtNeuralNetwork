using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArtNeuralNetwork
{
    static class NeuralNetworkSaves
    {
        public readonly static string NameDirectory = "saves";

        public static string[] Saves
        {
            get
            {
                return Directory.GetFiles(NameDirectory);
            }
        }

        public static void Save(NeuralNetwork neuralNetwork, string saveName) 
        {
            if (!saveName.Contains("txt")) saveName += ".txt";
            if (!saveName.Contains(NameDirectory + "/")) saveName = saveName.Insert(0, NameDirectory + "/");

            Stream saveFileStream = null;
            try
            {
                saveFileStream = File.Create(saveName);
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFileStream, neuralNetwork);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                saveFileStream?.Close();
            }
        }

        public static NeuralNetwork Load(string saveName)
        {
            if (!Saves.Contains(saveName)) throw new Exception("Не найдено сохранения с таким именем");
            
            NeuralNetwork neuralNetwork = null;
            Stream openFileStream = null;
            string[] saves = Saves;
            try
            {
                for (int i = 0; i < saves.Length; i++)
                {
                    if (saves[i].Contains(saveName)) 
                    {
                        openFileStream = File.OpenRead(saves[i]);
                        BinaryFormatter deserializer = new BinaryFormatter();
                        neuralNetwork = (NeuralNetwork)deserializer.Deserialize(openFileStream);
                        openFileStream.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                openFileStream?.Close();
            }

            return neuralNetwork;
        }
    }
}
