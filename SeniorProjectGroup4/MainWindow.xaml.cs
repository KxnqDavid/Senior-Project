﻿using System.Text;
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

            // Show open folder dialog box
            bool? result = dialog.ShowDialog();

            // Process open folder dialog box results
            if (result == true)
            {
                // Get the selected folder
                userDirectory = dialog.FolderName;
            }

            UserDir.Text = userDirectory;

        }

        private void VidQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UserLink_TextChanged(object sender, TextChangedEventArgs e)
        {
            mediaLink = UserLink.Text;
        }
      

        private void LightDarkMode_Click(object sender, RoutedEventArgs e)
        {
            if (backbox.Background == Brushes.Black)
                backbox.Background = Brushes.White;
            else
            backbox.Background = Brushes.Black;
        }

    }
}