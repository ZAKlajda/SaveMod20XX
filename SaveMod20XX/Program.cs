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
    class Program
    {
        /// <summary>
        /// This will be created/stored/loaded from the program execution directory
        /// </summary>
        const string SettingsFilePath = "Items20XX.Xml";

        /// <summary>
        /// A unique numeric value will also be appended
        /// </summary>
        const string BackupAppend = ".backup";

        /// <summary>
        /// This is the location of the documents folder for Windows 7 onwards
        /// </summary>
        const string SaveGamePath = "%userprofile%\\Documents\\20XX\\";

        /// <summary>
        /// This is the legacy (windows XP) document folder location.
        /// In case someone upgraded to 7 from XP or whatever
        /// </summary>
        const string SaveGamePathLegacy = "%userprofile%\\My Documents\\20XX\\";

        /// <summary>
        /// This is the file extension for the save game file
        /// </summary>
        const string SaveGameExtension = ".sav";

        /// <summary>
        /// This is the file name for tha save game file
        /// </summary>
        const string SaveFileName = "20xxalphav64" + SaveGameExtension;

        /// <summary>
        /// For decoding program return codes
        /// </summary>
        public enum ErrorState : int
        {
            /// <summary>
            /// As per the DOS specification, a return of 0 is "OK" or "Success"
            /// </summary>
            NoError = 0,

            NoSaveFileFound = 1,
            BadProgramArgument = 2,
            WrongFileSpecified = 3,
        }

        /// <summary>
        /// This program will open the settings file, creating one if it doesn't exist.
        /// Then it will locate the savegame, perform a backup and, finally, modify it.
        /// </summary>
        /// <param name="args">Arg[0], if it exists, should be the savegame file name and path.</param>
        /// <returns>0 for success, anything else for error</returns>
        static int Main(string[] args)
        {
            if (File.Exists(SettingsFilePath) == false)
            {
                Settings defaultSettings = new Settings();
                defaultSettings.SaveToFile(SettingsFilePath);
                defaultSettings = null;
            }
            Settings programSettings = Settings.LoadFromFile(SettingsFilePath);

            string SaveNameAndPathToUse = "";
            ErrorState saveFoundStatus = LocateSaveGameFile(args, out SaveNameAndPathToUse);
            if (saveFoundStatus != ErrorState.NoError)
            { return (int)saveFoundStatus; } // Early exit - error

            BackupOriginalSaveFile(SaveNameAndPathToUse);

            ErrorState modificationStatus = PerformSaveModification(SaveNameAndPathToUse, programSettings);

            Console.WriteLine("Program completion code " + modificationStatus);
            return (int)modificationStatus;
        }

        /// <summary>
        /// We figure out which save file to use
        /// </summary>
        /// <returns>The error state. 0 is All-OK.</returns>
        private static ErrorState LocateSaveGameFile(string[] args, out string SaveNameAndPathToUse)
        {
            ErrorState errorState;
            SaveNameAndPathToUse = "";

            // If they specified a file, use that one
            if (args.Length >= 1)
            {
                if (File.Exists(args[0]) && args[0].ToLower().EndsWith(SaveGameExtension))
                {
                    SaveNameAndPathToUse = args[0];
                    errorState = ErrorState.NoError;
                }
                else if (args[0].ToLower().EndsWith(".xml"))
                {
                    Console.Error.WriteLine("Error: You seem to have accidentally provided an XML file as ");
                    Console.Error.WriteLine("    an argument to this program: stahpit. ");
                    Console.Error.WriteLine("  Try providing a '" + SaveGameExtension + "' file, instead.");
                    errorState = ErrorState.WrongFileSpecified;
                }
                else
                {
                    Console.Error.WriteLine("Error: Unexpected first argument. Unable to proceed.");
                    errorState = ErrorState.BadProgramArgument;
                }
            } // If the didn't, check the default Windows 7+ directory
            else if (Directory.Exists(SaveGamePath) && File.Exists(SaveGamePath + "\\" + SaveFileName))
            {
                SaveNameAndPathToUse = SaveGamePath + "\\" + SaveFileName;
                errorState = ErrorState.NoError;
            } // If it's not there, check the Windows XP directory (maybe the upgraded to 7?):
            else if (Directory.Exists(SaveGamePathLegacy) && File.Exists(SaveGamePathLegacy + "\\" + SaveFileName))
            {
                SaveNameAndPathToUse = SaveGamePathLegacy + "\\" + SaveFileName;
                errorState = ErrorState.NoError;
            } // If it's none of those things, error. We cannot continue.
            else
            {
                Console.Error.WriteLine("Error: Unable to find default Save File location.");
                Console.Error.WriteLine("  Please enter a full file name and path for the 20XX save file as argument 1.");
                Console.Error.WriteLine("  You may drag-and-drop the desired save file onto this .exe for simplicity.");
                errorState = ErrorState.NoSaveFileFound;
            }

            return errorState;
        }

        /// <summary>
        /// Exactly what it says on the tin.
        /// This new version appends an incrementing counter to always save your old file rather than overwrite it
        /// </summary>
        private static void BackupOriginalSaveFile(string inputFileNameAndPath)
        {
            Console.WriteLine("Backing up previous save file...");

            // Find a unique save backup name -- no more overwriting old save files!
            int appendCounter = 0;
            while (File.Exists(inputFileNameAndPath + BackupAppend + appendCounter))
            { ++appendCounter; }

            File.Copy(inputFileNameAndPath, inputFileNameAndPath + BackupAppend + appendCounter, false);
            Console.WriteLine("... backup complete.");
        }

        /// <summary>
        /// This does the actual modification of the save file according to your XML settings file
        /// </summary>
        /// <param name="inputFileNameAndPath">Where's our save file?</param>
        /// <param name="settings">The loaded XML settings file</param>
        /// <returns>Windows completion code--0 is success, anything else is an error of some kind</returns>
        private static ErrorState PerformSaveModification(string inputFileNameAndPath, Settings settings)
        {
            FileInfo fi = new FileInfo(inputFileNameAndPath);
            byte[] fileBytes = new byte[fi.Length];
            fileBytes = ReadInFile(inputFileNameAndPath, fi);

            PerformUnlocks(fileBytes, settings);
            PerformDataLore(fileBytes, settings);

            WriteOutFile(inputFileNameAndPath, fileBytes);

            // Everything OK!
            return ErrorState.NoError;
        }


        /// <summary>
        /// This does the "Datalore" section unlocks
        /// </summary>
        /// <param name="fileBytes">The contents of the save file</param>
        /// <param name="settings">The loaded XML settings file</param>
        private static void PerformDataLore(byte[] fileBytes, Settings settings)
        {
            List<Tuple<long, byte[]>> DataLores = new List<Tuple<long, byte[]>>();

            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.BasicAugments, 
                                                  Settings.GetRawBytesFromBigInt(CongealItems(settings.BasicAugments))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.PrimaryWeapons,
                                                  Settings.GetRawBytesFromBigInt(CongealItems(settings.PrimaryWeapons))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.CoreAugs,
                                                  Settings.GetRawBytesFromBigInt(CongealItems(settings.CoreAugs))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.Protoypes,
                                                  Settings.GetRawBytesFromBigInt(CongealItems(settings.Prototypes))));

            DoAllModifications(fileBytes, DataLores);
        }

        /// <summary>
        /// Performs the unlocks for the "Unlocks" section
        /// </summary>
        /// <param name="fileBytes">The contents of the save file</param>
        /// <param name="settings">The loaded XML settings file</param>
        private static void PerformUnlocks(byte[] fileBytes, Settings settings)
        {
            List<Tuple<long, byte[]>> Unlocks = new List<Tuple<long, byte[]>>();

            Unlocks.Add(new Tuple<long, byte[]>(settings.UnlockByteOffsets.BasicAugments,
                                                Settings.GetRawBytesFromBigInt(CongealItems(settings.BasicAugments))));
            Unlocks.Add(new Tuple<long, byte[]>(settings.UnlockByteOffsets.PrimaryWeapons,
                                                Settings.GetRawBytesFromBigInt(CongealItems(settings.PrimaryWeapons))));

            DoAllModifications(fileBytes, Unlocks);
        }

        /// <summary>
        /// Performs the actual modifications to the raw file data
        /// </summary>
        /// <param name="rawData">The contents of the save file</param>
        /// <param name="mods">The prepared set of modifications: the ulong is their starting file location, the byte[] is the modified data to place at that location</param>
        public static void DoAllModifications(byte[] rawData, List<Tuple<long, byte[]>> mods)
        {
            foreach (var mod in mods)
            {
                DoModification(mod.Item1, rawData, mod.Item2);
            }
        }

        /// <summary>
        /// Performs each specific modification to the raw file data
        /// </summary>
        /// <param name="offset">Where in the file is this modification</param>
        /// <param name="fileBytes">The contents of the save file</param>
        /// <param name="changed">What has changed at this file location?</param>
        public static void DoModification(long offset, byte[] data, byte[] changed)
        {
            Array.Copy(changed, (long)0, data, offset, (long)changed.Length);
        }

        /// <summary>
        /// Congeals all item bit-flags for a specific section of the output data
        /// </summary>
        /// <param name="itemsToCongeal">Here's the bit-flags and toggle-state of the items for this section</param>
        /// <returns>The combined bit-flags for this file section</returns>
        private static BigInteger CongealItems(List<Item> itemsToCongeal)
        {
            BigInteger allItems = 0;
            foreach (Item item in itemsToCongeal)
            {
                if (item.Unlocked)
                {
                    allItems |= Settings.GetAsBigInt(item.HexValue);
                }
                else
                {
                    allItems &= ~Settings.GetAsBigInt(item.HexValue);
                }
            }
            return allItems;
        }

        /// <summary>
        /// Simply writes the file. Nothing to see here.
        /// </summary>
        private static void WriteOutFile(string outputFileNameAndPath, byte[] fileBytes)
        {
            using (FileStream fs = new FileStream(outputFileNameAndPath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(fileBytes);
            }
        }

        /// <summary>
        /// Simply reads the file, nothing to see here.
        /// </summary>
        /// <returns>The file contents</returns>
        private static byte[] ReadInFile(string inputFileNameAndPath, FileInfo fi)
        {
            byte[] fileBytes;
            using (FileStream fs = new FileStream(inputFileNameAndPath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                fileBytes = br.ReadBytes((int)fi.Length);
            }

            return fileBytes;
        }
    }
}
