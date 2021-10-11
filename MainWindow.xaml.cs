﻿using Microsoft.Win32;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace ArtNeuralNetwork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NeuralNetwork neuralNet;

        public MainWindow()
        {
            InitializeComponent();
            neuralNet = new NeuralNetwork(5, 1);

        }

        private Dictionary<double[], int> ExportExcelDataSet()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "*.xls;*.xlsx";
            ofd.Title = "Выберите файл обучающей выборки";

            ofd.ShowDialog();

            //if (!(ofd.ShowDialog() == DialogResult.HasValue)) // если файл БД не выбран -> Выход
            //    return null;

            Excel.Application excel = null;
            Excel.Workbook workbook = null;
            Excel.Workbooks workbooks = null;
            Excel.Worksheet workSheet = null;
            Dictionary<double[], int> dataSet = new Dictionary<double[], int>();
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
                    int exceptedValue = 0;
                    for (int i = 2; i <= rows; i++)
                    {
                        if (i == rows)
                        {
                            int.TryParse(workSheet.Cells[i, j].Text.ToString(), out exceptedValue);
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
                //workSheet = null;
                //workbook = null;
                //workbooks = null;
                //excel = null;

                //GC.Collect();
            }
            
            return dataSet;
        }

        private double[] CollectData() 
        {
            List<double> dataParameters = new List<double>();
            foreach (UIElement uI in spParameters.Children) 
            {
                if (uI is CheckBox checkBox)
                {
                    double value = 0.1;
                    if ((bool)checkBox.IsChecked) value = 0.9;

                    dataParameters.Add(value);
                }
            }

            return dataParameters.ToArray();
        }

        private void btGetResult_Click(object sender, RoutedEventArgs e)
        {
            double[] data = CollectData();
            int result = neuralNet.GetResult(data);

            switch (result) 
            {
                case (int)TypesPaintings.Portrait:
                    tbResult.Text = "портрет.";
                    break;

                case (int)TypesPaintings.Landscape:
                    tbResult.Text = "пейзаж.";
                    break;

                case (int)TypesPaintings.StillLife:
                    tbResult.Text = "натюрморт.";
                    break;
                default:
                    tbResult.Text = "не определено.";
                    break;
            }
        }

        private void miLoadData_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<double[], int> dataSet = ExportExcelDataSet();
            neuralNet.Train(dataSet);
        }
    }
}
