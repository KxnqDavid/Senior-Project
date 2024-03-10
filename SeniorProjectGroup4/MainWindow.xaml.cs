using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System;

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

        private Process process;

        private ProgressBar downloadBar;

        public MainWindow()
        {
            InitializeComponent();
            downloadBar = DownloadBar;
        }
        private void UserLink_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            mediaLink = UserLink.Text;
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
                    else if (child is TextBlock textBlock)
                    {
                        textBlock.Style = (Style)FindResource("DarkTextBlockStyle");
                    }
                    else if (child is Button button)
                    {
                        button.Style = (Style)FindResource("DarkButtonStyle");

                    }
                    else if (child is ComboBox comboBox)
                    {
                        comboBox.Style = (Style)FindResource("DarkComboBoxStyle");
                    }
                }
            }
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void VidQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)VidQuality.SelectedValue;
            quality = (string)item.Content;
        }

        private void VidFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)VidFormat.SelectedValue;
            format = (string)item.Content;
        }

        private void AudioFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)AudioFormat.SelectedValue;
            audioFormat = (string)item.Content;
        }

        private async void DLButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(mediaLink) || string.IsNullOrWhiteSpace(userDirectory))
            {
                MessageBox.Show("Please enter a valid YouTube URL and select a download directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(format) || string.IsNullOrWhiteSpace(quality))
            {
                MessageBox.Show("Please select a video format and quality.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                await downloadVideo(mediaLink, userDirectory, "best", DownloadBar);
                MessageBox.Show("Download initiated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task downloadVideo(string link, string directory, string format, ProgressBar progressBar)
        {
            try
            {
                string arguments = $"-f {format} -o \"{directory}\\%(title)s.%(ext)s\" {link}";
                await RunYTDLProcess(arguments, progressBar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task RunYTDLProcess(string arguments, ProgressBar progressBar)
        {
            try
            {
                string ytDlpExecutable = "yt-dlp.exe";

                using (process = new Process())
                {
                    process.StartInfo.FileName = ytDlpExecutable;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    TaskCompletionSource<bool> outputReadTaskCompletionSource = new TaskCompletionSource<bool>();

                    // Event handlers for capturing progress
                    process.OutputDataReceived += (s, args) => UpdateProgress(args.Data);
                    process.ErrorDataReceived += (s, args) => UpdateProgress(args.Data);

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.Exited += (s, args) => HandleProcessExit();

                    await outputReadTaskCompletionSource.Task;

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void UpdateProgress(string data)
        {
            // Log the output for debugging
            Console.WriteLine($"Output: {data}");

            // Example: Check for lines containing "download" and percentage
            if (!string.IsNullOrEmpty(data) && data.Contains("[download]") && data.Contains("%"))
            {
                // Extract percentage information and update ProgressBar
                // Example: [download]  81.2% of  204.11MiB at    3.46MiB/s ETA 00:11
                int indexOfPercentage = data.IndexOf('%');
                if (indexOfPercentage != -1)
                {
                    // Extract the percentage and convert it to a double
                    if (double.TryParse(data.Substring(indexOfPercentage - 4, 4).Trim(), out double progress))
                    {
                        // Update UI elements on the main thread
                        Dispatcher.Invoke(() =>
                        {
                            downloadBar.Value = progress / 100.0;
                        });
                    }
                }
            }
        }


        private void HandleProcessExit()
        {
            process.WaitForExit();
            process.Close();
        }



    }
}