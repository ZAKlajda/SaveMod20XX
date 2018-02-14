﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SaveMod20XX.Program;

namespace SaveMod20XX
{
    /// <summary>
    /// Interaction logic for SaveModGUI.xaml
    /// </summary>
    public partial class SaveModGUI : Window
    {
        public ObservableCollection<Item> AllItems { get; set; } = new ObservableCollection<Item>();
        internal Settings SettingsFile { get; set; }
        internal string SaveNameAndPathToUse { get; set; }
        internal string SettingsPathToUse { get; set; }
        
        public SaveModGUI()
        {
            InitializeComponent();
            
            Loaded += SaveModGUI_Loaded;

            SettingsPathToUse = Program.SettingsFilePath;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void SaveModGUI_Loaded(object sender, RoutedEventArgs e)
        {
            ItemGrid.Columns.Remove(ItemGrid.Columns.First((col) => col.Header.ToString() == "HexValue")); // Remove the HexValue column
            ItemGrid.Columns.Remove(ItemGrid.Columns.First((col) => col.Header.ToString() == "Lockable")); // Remove the HexValue column
            List<Item> removeItems = AllItems.Where((item) => item.Name == String.Empty || item.Lockable == false).ToList();
            foreach (Item item in removeItems)
            {
                AllItems.Remove(item); // <-- remove placeholders and non-lockable items
            }
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Opening Settings File...");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine("    " + openFileDialog.FileName);

                SettingsFile = Settings.LoadFromFile(openFileDialog.FileName);

                AllItems.Clear();

                foreach (Item item in SettingsFile.BasicAugments.Concat(SettingsFile.CoreAugs).Concat(SettingsFile.PrimaryWeapons).Concat(SettingsFile.Prototypes))
                {
                    AllItems.Add(item);
                }

                List<Item> removeItems = AllItems.Where((item) => item.Name == String.Empty || item.Lockable == false).ToList();
                foreach (Item item in removeItems)
                {
                    AllItems.Remove(item); // <-- remove placeholders and non-lockable items
                }

                SettingsPathToUse = openFileDialog.FileName;

                Console.WriteLine("    File Opened.");
            }
            
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Saving changes to settings file...");
            SettingsFile.SaveToFile(SettingsPathToUse);
            Console.WriteLine("    Changes saved.");
        }

        private void RunModification_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Performing GUI modification...");

            ErrorState modificationStatus = PerformSaveModification(SaveNameAndPathToUse, SettingsFile);

            Console.WriteLine("    Modification completion code " + modificationStatus);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
