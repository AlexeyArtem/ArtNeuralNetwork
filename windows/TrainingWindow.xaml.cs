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
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace ArtNeuralNetwork
{
    /// <summary>
    /// Interaction logic for TrainingWindow.xaml
    /// </summary>
    public partial class TrainingWindow : Window
    {
        NeuralNetwork neuralNetwork;
        Dictionary<double[], double> dataset;
        public TrainingWindow(NeuralNetwork neuralNetwork)
        {
            InitializeComponent();
            this.neuralNetwork = neuralNetwork;
        }

        private Dictionary<double[], double> ExportExcelDataSet(out string fileName)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "*.xls;*.xlsx";
            ofd.Title = "Выберите файл обучающей выборки";

            if (ofd.ShowDialog() == false) 
            {
                fileName = ofd.FileName;
                return null;
            }

            fileName = ofd.FileName;
            Excel.Application excel = null;
            Excel.Workbook workbook = null;
            Excel.Workbooks workbooks = null;
            Excel.Worksheet workSheet = null;
            var dataSet = new Dictionary<double[], double>();
            try
            {
                excel = new Excel.Application();
                workbooks = excel.Workbooks;
                workbook = workbooks.Open(ofd.FileName);
                workSheet = (Excel.Worksheet)workbook.Sheets[1];
                var lastCell = workSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);

                int colums = lastCell.Column;
                int rows = 7;

                for (int j = 2; j <= colums; j++)
                {
                    List<double> inputParameters = new List<double>();
                    double exceptedValue = 0;
                    for (int i = 2; i <= rows; i++)
                    {
                        if (i == rows)
                        {
                            double.TryParse(workSheet.Cells[i, j].Text.ToString(), out exceptedValue);
                            dataSet.Add(inputParameters.ToArray(), exceptedValue);
                            break;
                        }

                        if (!double.TryParse(workSheet.Cells[i, j].Text.ToString(), out double value)) break;

                        inputParameters.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неверное заполнение файла данными. Подробнее об ошибке: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                int temp = dataSet.Count;
                workbook?.Close(false, Type.Missing, Type.Missing);
                workbooks?.Close();
                excel?.Quit();
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(workbooks);
                Marshal.ReleaseComObject(excel);
                workSheet = null;
                workbook = null;
                workbooks = null;
                excel = null;
            }

            return dataSet;
        }


        private void btTrainig_Click(object sender, RoutedEventArgs e)
        {
            if (dataset == null) 
            {
                MessageBox.Show("Выберите набор данных", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                neuralNetwork.Training(dataset, (int)udEnterEpochs.Value);
                MessageBox.Show("Нейросеть успешно обучена", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch
            {
                MessageBox.Show("В процессе обучения возникла ошибка. Попробуйте повторить попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btSelectFile_Click(object sender, RoutedEventArgs e)
        {
            string fileName;
            dataset = ExportExcelDataSet(out fileName);
            tbNameFile.Text = System.IO.Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
