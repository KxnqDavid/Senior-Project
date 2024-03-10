using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System;
using System.Configuration;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace SeniorProjectGroup4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string userDirectory { get; set; }
        string mediaLink = "";
        string _format;
        public string format { get; set; }
        public string audioFormat { get; set; }

        private Process process;

        private ProgressBar downloadBar;

        public string quality { get; set; }

        // Implement PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            downloadBar = DownloadBar;


            // ReadSettings needs to read the config file and we still need the program to 
            // automatically store the most recently selected options so that they will be read when opening program
            ReadSettings();
            DataContext = this;

        }
        private string GetConfigFilePath() 
        {
            // Get the directory of the executable or the current working directory
            string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // Combine the directory path with the config file name
            return Path.Combine(appDirectory, "..", "..", "..", "config.xml");
        }

        private void ReadSettings() // an attempt to save user settings such as directory so when app runs it will save the directory location (light/dark theme not added yet)
        {
            string configFile = GetConfigFilePath();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);
                XmlNode configNode = doc.SelectSingleNode("//Theme");
                if (configNode != null)
                {
                    LightDarkMode_Change(configNode.InnerText);
                }

                configNode = doc.SelectSingleNode("//Quality");
                if (configNode != null) 
                {
                    this.quality = configNode.InnerText;
                    VidQuality.SelectedIndex = VidQuality_Index(quality);
                }

                configNode = doc.SelectSingleNode("//Video");
                if (configNode != null)
                {
                    this.format = configNode.InnerText;
                    VidFormat.SelectedIndex = VidFormat_Index(format);
                }

                configNode = doc.SelectSingleNode("//Audio");
                if (configNode != null)
                {
                    this.audioFormat = configNode.InnerText;
                    AudioFormat.SelectedIndex = AudioFormat_Index(audioFormat);
                }

                configNode = doc.SelectSingleNode("//UserDirectory");
                if (configNode != null)
                {
                    this.userDirectory = configNode.InnerText;
                    UserDir.Text = userDirectory;
                }
                
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading user config");
            }

        }

        private void UserLink_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            mediaLink = UserLink.Text;
        }

        private void ChangeDirectory_Click_1(object sender, RoutedEventArgs e)
        {
            string configFile = GetConfigFilePath();
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
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);
                XmlNode configNode = doc.SelectSingleNode("//UserDirectory");
                if (configNode != null)
                {
                    configNode.InnerText = userDirectory;
                    doc.Save(configFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing config file: " + ex.Message);
            }
        }

        // applies style settings based on theme value
        private void LightDarkMode_Change(string pref)
        {
            if (pref == "Light")
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

        // now saves new theme value upon click
        private void LightDarkMode_Click(object sender, RoutedEventArgs e)
        {
            string theme;
            string configFile = GetConfigFilePath();
            if (backbox.Background != Brushes.White)
            {
                theme = "Light";
            }
            else
            {
                theme = "Dark";
            }

            LightDarkMode_Change(theme);

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);
                XmlNode configNode = doc.SelectSingleNode("//Theme");
                if (configNode != null)
                {
                    configNode.InnerText = theme;
                    doc.Save(configFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing config file: " + ex.Message);
            }
        }

        private int VidQuality_Index(string value)
        {
            if(value == "360")
                return 0;
            else if(value == "480")
                return 1;
            else if(value == "720")
                return 2;
            else
                return 3;
        }
        private void VidQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string configFile = GetConfigFilePath();
            var item = (ComboBoxItem)VidQuality.SelectedValue;
            quality = (string)item.Content;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);
                XmlNode configNode = doc.SelectSingleNode("//Quality");
                if (configNode != null)
                {
                    configNode.InnerText = quality;
                    doc.Save(configFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing config file: " + ex.Message);
            }
        }

        private int VidFormat_Index(string value)
        {
            if (value == "webm")
                return 0;
            else if(value == "mp4")
                return 1;
            else if(value == "mov")
                return 2;
            else
                return 3;
        }
        private void VidFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string configFile = GetConfigFilePath();
            var item = (ComboBoxItem)VidFormat.SelectedValue;
            format = (string)item.Content;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);
                XmlNode configNode = doc.SelectSingleNode("//Video");
                if (configNode != null)
                {
                    configNode.InnerText = format;
                    doc.Save(configFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing config file: " + ex.Message);
            }
        }

        private int AudioFormat_Index(string value)
        {
            if (value == "mp3")
                return 0;
            else if( value == "wav")
                return 1;
            else if(value == "flac")
                return 2;
            else
                return 3;
        }
        private void AudioFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string configFile = GetConfigFilePath();
            var item = (ComboBoxItem)AudioFormat.SelectedValue;
            audioFormat = (string)item.Content;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configFile);
                XmlNode configNode = doc.SelectSingleNode("//Audio");
                if (configNode != null)
                {
                    configNode.InnerText = audioFormat;
                    doc.Save(configFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing config file: " + ex.Message);
            }

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

                    MessageBox.Show("Download initiated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

            Console.WriteLine($"Output: {data}");

            if (!string.IsNullOrEmpty(data) && data.Contains("[download]") && data.Contains("%"))
            {
                int indexOfPercentage = data.IndexOf('%');
                if (indexOfPercentage != -1)
                {
                    if (double.TryParse(data.Substring(indexOfPercentage - 4, 4).Trim(), out double progress))
                    {
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

        private void togglebutton_Checked(object sender, RoutedEventArgs e)
        {

        }


    }
}