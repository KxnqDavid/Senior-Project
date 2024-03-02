using System.Text;
using System.Windows;
using Microsoft.Win32;
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
        string userDirectory = "";
        string mediaLink = "";
        string quality = "";
        string format = "";
        string audioFormat = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeDirectory_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFolderDialog dialog = new()
            {
                Multiselect = false,
                Title = "Select a directory to download to"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                userDirectory = dialog.FolderName;
            }

            UserDir.Text = userDirectory;

        }

        private void UserLink_TextChanged(object sender, TextChangedEventArgs e)
        {
            mediaLink = UserLink.Text;
        }
      

        private void LightDarkMode_Click(object sender, RoutedEventArgs e)
        {
            if (backbox.Background != Brushes.White)
            {
                backbox.Style = (Style)FindResource("DefaultGridStyle");
                foreach (var child in backbox.Children)
                {
                    if (child is TextBox textBox)
                    {
                        textBox.Style = (Style)FindResource("DefaultTextBoxStyle");
                    }
                    else if (child is TextBlock textBlock)
                    {
                        textBlock.Style = (Style)FindResource("DefaultTextBlockStyle");
                    }
                    else if (child is Button button)
                    {
                        button.Style = (Style)FindResource("DefaultButtonStyle");
                    }
                    else if (child is ComboBox comboBox)
                    {
                        comboBox.Style = (Style)FindResource("DefaultComboBoxStyle");
                    }
                }
            }
            else
            {
                backbox.Style = (Style)FindResource("DarkGridStyle");
                foreach (var child in backbox.Children)
                {
                    if (child is TextBox textBox)
                    {
                        textBox.Style = (Style)FindResource("DarkTextBoxStyle");
                    }
                    else if(child is TextBlock textBlock)
                    {
                        textBlock.Style = (Style)FindResource("DarkTextBlockStyle");
                    }
                    else if(child is Button button)
                    {
                        button.Style = (Style)FindResource("DarkButtonStyle");
                        
                    }
                    else if(child is ComboBox comboBox)
                    {
                        comboBox.Style = (Style)FindResource("DarkComboBoxStyle");
                    }
                }
            }
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void DLButton_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void VidQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)VidQuality.SelectedValue;
            quality = (string)item.Content;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)VidFormat.SelectedValue;
            format = (string)item.Content;
        }

        private void AudioFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)AudioFormat.SelectedValue;
            audioFormat = (string)item.Content;
        }
    }
}