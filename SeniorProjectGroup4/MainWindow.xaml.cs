using System.Text;
using System.Windows;
using Microsoft.Windows.Compatibility;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Diagnostics;

namespace SeniorProjectGroup4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Console.WriteLine("Hello World!");


        }
        private void ChangeDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                string selectedFolderPath = folderBrowserDialog.SelectedPath;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}