using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using TwinFinder.FileMetadata;
using TwinFinder.ScannersInFileSystem;
using TwinFinder.service;
using System.Collections.Generic;
using TwinFinder.Dao;

namespace WPFFileTwinFinder
{
    public partial class MainWindow : Window
    {
        private readonly FileMetadataCsvDao _fileMetadataDao;
        private readonly TwinFinderService _twinFinder;
        private readonly List<CheckBox> _fileCheckBoxes = new List<CheckBox>(); // Track checkboxes for files

        public MainWindow()
        {
            // Check if the file specified in Settings.FileMetadataDaoPath exists
            if (!File.Exists(Settings.FileMetadataDaoPath))
            {
                // Create the file if it does not exist
                File.WriteAllText(Settings.FileMetadataDaoPath, "FullPath,CreationDate,FileHash\n");
            }

            _fileMetadataDao = new FileMetadataCsvDao(Settings.FileMetadataDaoPath);
            _twinFinder = new TwinFinderService(_fileMetadataDao);
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsStackPanel.Children.Clear();
            _fileCheckBoxes.Clear();
            var folderPicker = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "Select Folder"
            };

            if (folderPicker.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = folderPicker.FileName;
                DirectoryPathTextBox.Text = path;
                _twinFinder.DirectoryPath = path;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var directoryPath = _twinFinder.DirectoryPath;

            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                MessageBox.Show("Please select a valid folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _twinFinder.FindTwin();
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occurred while searching for duplicates.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            DisplayResults();
        }

        private void DisplayResults()
        {
            ResultsStackPanel.Children.Clear();
            _fileCheckBoxes.Clear(); // Clear previous checkboxes

            if (_twinFinder.Duplicates.Count == 0)
            {
                ResultsStackPanel.Children.Add(new TextBlock { Text = "No duplicates found." });
            }
            else
            {
                foreach (var duplicateGroup in _twinFinder.Duplicates)
                {
                    AddDuplicateGroupToResults(duplicateGroup);
                }
            }
        }

        private void AddDuplicateGroupToResults(IEnumerable<TwinFinder.FileMetadata.FileMetadataInfo> duplicateGroup)
        {
            var groupBox = CreateGroupBox();

            var stackPanel = new StackPanel();
            foreach (var fileMetadata in duplicateGroup)
            {
                AddFileToGroup(fileMetadata, stackPanel);
            }

            groupBox.Content = stackPanel;
            ResultsStackPanel.Children.Add(groupBox);
        }

        private GroupBox CreateGroupBox()
        {
            return new GroupBox
            {
                Header = "Duplicates found:",
                Margin = new Thickness(0, 10, 0, 10),
                Padding = new Thickness(10),
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                Background = System.Windows.Media.Brushes.LightGray
            };
        }

        private void AddFileToGroup(TwinFinder.FileMetadata.FileMetadataInfo fileMetadata, StackPanel stackPanel)
        {
            var stackPanelWithCheckbox = new StackPanel { Orientation = Orientation.Horizontal };

            var fileDisplay = new TextBlock
            {
                Text = fileMetadata.FullFilePath,
                Margin = new Thickness(5, 2, 5, 2),
                VerticalAlignment = VerticalAlignment.Center
            };
            
            var checkBox = new CheckBox
            {
                Tag = fileMetadata.FullFilePath, 
                Margin = new Thickness(5, 2, 5, 2),
                VerticalAlignment = VerticalAlignment.Center
            };
            
            checkBox.Checked += (s, e) => UpdateDeleteButtonState();
            checkBox.Unchecked += (s, e) => UpdateDeleteButtonState();

            _fileCheckBoxes.Add(checkBox);

            stackPanelWithCheckbox.Children.Add(checkBox);
            stackPanelWithCheckbox.Children.Add(fileDisplay);

            stackPanel.Children.Add(stackPanelWithCheckbox);
        }

        private void UpdateDeleteButtonState()
        {
            // Check if any checkbox is selected
            DeleteButton.IsEnabled = _fileCheckBoxes.Any(c => c.IsChecked == true);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var filesToDelete = _fileCheckBoxes.Where(c => c.IsChecked == true)
                .Select(c => c.Tag.ToString())
                .ToList();

            if (filesToDelete.Count == 0)
            {
                MessageBox.Show("Please select files to delete.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            foreach (var filePath in filesToDelete)
            {
                try
                {
                    _twinFinder.EraseFile(filePath);
                    MessageBox.Show($"File {filePath} has been successfully deleted.", "Success", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show($"Error: You do not have permission to delete the file {filePath}.",
                        "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(
                        $"Error: An I/O error occurred while trying to delete the file {filePath}: {ex.Message}",
                        "I/O Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error: An unexpected error occurred while deleting the file {filePath}: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            DeleteButton.IsEnabled = false;
            StartButton_Click(sender, e);
        }
    }
}