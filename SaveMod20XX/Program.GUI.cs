using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SaveMod20XX
{
    partial class Program
    {
        public static Application WinApp { get; private set; }
        public static SaveModGUI MainWindow { get; private set; }

        static void RunGui(Settings programSettings, string saveNameAndPathToUse)
        {
            WinApp = new Application();
            MainWindow = new SaveModGUI();
            foreach (Item item in programSettings.BasicAugments.Concat(programSettings.CoreAugs).Concat(programSettings.PrimaryWeapons).Concat(programSettings.Prototypes))
            {
                MainWindow.AllItems.Add(item);
            }
            MainWindow.SaveNameAndPathToUse = saveNameAndPathToUse;
            MainWindow.SettingsFile = programSettings;
            WinApp.Run(MainWindow); // note: blocking call
        }
    }
}
