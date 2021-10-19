using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for SavesWindow.xaml
    /// </summary>
    public partial class SaveListWindow : Window
    {
        public SaveListWindow()
        {
            InitializeComponent();
            listBoxSaves.ItemsSource = NeuralNetworkSaves.Saves;
        }

        public NeuralNetwork LoadedNeuralNetwork { get; private set; }

        private void btLoad_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSaves.SelectedItem == null)
            {
                MessageBox.Show("Выберите сохранение для загрузки", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                LoadedNeuralNetwork = NeuralNetworkSaves.Load((string)listBoxSaves.SelectedItem);
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
