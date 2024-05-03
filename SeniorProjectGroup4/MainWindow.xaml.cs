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
using System.Windows.Input;
using System.Net.Mail;

namespace SeniorProjectGroup4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string userDirectory { get; set; }
        public string mediaLink { get; set; }
   
        public string format { get; set; }
        public string audioFormat { get; set; }

        private Process process;

        private ProgressBar downloadBar;

        public string quality { get; set; }

        public string fileName { get; set; }
        public string defaultFileMessage { get; set; }

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
            mediaLink = "";
            defaultFileMessage = "<-- Change Default File Name -->";
            UserLink.TextChanged += UserLink_TextChanged_1;
            FocusManager.SetFocusedElement(this, OutputFile);
            FocusManager.SetFocusedElement(this, this);
            ReadSettings();
            DataContext = this;

        }
        private string GetConfigFilePath()
        {
            // Get the directory of the executable or the current working directory
            string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // Combine the directory path with the config file name
            string configFilePath = Path.Combine(appDirectory, "..", "..", "..", "config.xml");

            // Check if the config file exists, if not, create it
            if (!File.Exists(configFilePath))
            {
                using (StreamWriter sw = new StreamWriter(configFilePath))
                {
                    // Write XML content to the file
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<Configuration>");
                    sw.WriteLine("  <Theme>Dark</Theme>");
                    sw.WriteLine("  <Quality>720</Quality>");
                    sw.WriteLine("  <Video>mp4</Video>");
                    sw.WriteLine("  <Audio>mp3</Audio>");
                    sw.WriteLine("  <UserDirectory></UserDirectory>");
                    sw.WriteLine("</Configuration>");
                }
            }

            return configFilePath;
        }

        private string GetExeFilePath()
        {
            string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            return Path.Combine(appDirectory, "..", "..", "..", "yt-dlp.exe");
        }

        private void ReadSettings() // an attempt to save user settings such as directory so when app runs it will save the directory location
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
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                this.mediaLink = textBox.Text;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == defaultFileMessage)
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.FontSize = 11;
                textBox.Text = defaultFileMessage;
            }
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

        private void ChangeFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string configFile = GetConfigFilePath();
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                this.fileName = textBox.Text;
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
                    else if (child is ProgressBar progressBar)
                    {
                        progressBar.Style = (Style)FindResource("DefaultMedia");
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
                    else if(child is ProgressBar progressBar)
                    {
                        progressBar.Style = (Style)FindResource("DarkMedia");
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
                DownloadBar.Value = 0;
                await downloadVideo(mediaLink, userDirectory, format, DownloadBar);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task downloadVideo(string link, string directory, string format, ProgressBar progressBar)
        {
            if(this.fileName == defaultFileMessage)
            {
                this.fileName = "%(title)s";
            }
            else
            {
                if (!verifyFileName(this.fileName))
                {
                    MessageBox.Show($"Invalid file name format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
            try
            {
                Trace.WriteLine($"Downloading video from {link} to {directory} in {format} format at {quality} quality.");

                // TODO: Add audio format options (bestaudio[ext={audioQuality}, include m4a as a audio format option (possibly as default), add command for backup ba+bv/b 
                string arguments = $"-f \"bestvideo[height<={quality}]+bestaudio/best[ext={format}]\" -o \"{directory}\\{fileName}.%(ext)s\" {link}";
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

                // reminder that we may need to update this path once we create a final release/executable for the application
                string ytDlpExecutable = GetExeFilePath();

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

                    // TODO: instead of MessageBox, we should add an area maybe at the bottom of the application window that displays alerts (less annoying this way)
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
                Trace.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void UpdateProgress(string data)
        {
            //TODO: add text for the progress/download %
            Trace.WriteLine($"Output: {data}");

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
    
        private bool verifyFileName(string nameCheck)
        {
            char[] illegalChars = { '#', '%', '&', '{', '}', '/', '<', '>', '*', '?', '$', '!', '"', ':', '@', '+', '`', '|', '=', '\\', '\''};
            if(illegalChars.Any(c => nameCheck.Contains(c)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}