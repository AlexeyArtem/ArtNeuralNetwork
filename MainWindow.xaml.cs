using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.Serialization.Formatters.Binary;

namespace ArtNeuralNetwork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NeuralNetwork neuralNetwork;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                neuralNetwork = NeuralNetworkSaves.Load(NeuralNetworkSaves.Saves[0]);
            }
            catch
            {
                neuralNetwork = new NeuralNetwork(5, 1, 6, 5);
            }

        }

        private double[] CollectData() 
        {
            List<double> dataParameters = new List<double>();
            foreach (UIElement uI in spParameters.Children) 
            {
                if (uI is CheckBox checkBox)
                {
                    double value = 0;
                    if ((bool)checkBox.IsChecked) value = 1;

                    dataParameters.Add(value);
                }
            }

            return dataParameters.ToArray();
        }

        private void btGetResult_Click(object sender, RoutedEventArgs e)
        {
            double[] data = CollectData();
            TypesPaintings? type = neuralNetwork.GetTypesPaintings(data);

            switch (type) 
            {
                case TypesPaintings.Portrait:
                    tbResult.Text = "портрет.";
                    break;

                case TypesPaintings.Landscape:
                    tbResult.Text = "пейзаж.";
                    break;

                case TypesPaintings.StillLife:
                    tbResult.Text = "натюрморт.";
                    break;
                default:
                    tbResult.Text = "не определено.";
                    break;
            }
        }

        private void miSaveNetwork_Click(object sender, RoutedEventArgs e)
        {
            new SaveWindow(neuralNetwork).ShowDialog();
        }

        private void miTraining_Click(object sender, RoutedEventArgs e)
        {
            new TrainingWindow(neuralNetwork).ShowDialog();
        }

        private void miOpenSaves_Click(object sender, RoutedEventArgs e)
        {
            SaveListWindow window = new SaveListWindow();
            window.Closed += delegate (object s, EventArgs args)
            {
                if (window.LoadedNeuralNetwork != null) neuralNetwork = window.LoadedNeuralNetwork;
            };
            window.ShowDialog();
        }

        private void miCreateNetwork_Click(object sender, RoutedEventArgs e)
        {
            neuralNetwork = new NeuralNetwork(5, 1, 6, 5);
            MessageBox.Show("Новая нейросеть успешно создана", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
