using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SaveMod20XX
{
    partial class Program
    {
        /// <summary>
        /// This will be created/stored/loaded from the program execution directory
        /// </summary>
        internal const string SettingsFilePath = "Items20XX.Xml";

        /// <summary>
        /// A unique numeric value will also be appended
        /// </summary>
        const string BackupAppend = ".backup";

        /// <summary>
        /// This is the location of the documents folder for Windows
        /// </summary>
        static readonly string SaveGamePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\20XX";
        
        /// <summary>
        /// This is the file extension for the save game file
        /// </summary>
        const string SaveGameExtension = ".sav";

        /// <summary>
        /// This is the file name for tha save game file
        /// </summary>
        const string SaveFileName = "20xxalphav64" + SaveGameExtension;
    }
}
