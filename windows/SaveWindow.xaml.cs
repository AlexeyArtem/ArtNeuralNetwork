using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArtNeuralNetwork
{
    /// <summary>
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        NeuralNetwork neuralNetwork;
        public SaveWindow(NeuralNetwork neuralNetwork)
        {
            InitializeComponent();
            this.neuralNetwork = neuralNetwork;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbSaveName.Text == string.Empty) 
            {
                MessageBox.Show("Введите название сохранения", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                NeuralNetworkSaves.Save(neuralNetwork, tbSaveName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Close();
        }
    }
}
