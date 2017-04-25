using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SaveMod20XX
{
    partial class Program
    {
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

            /// <summary>
            /// We can't tell if the GUI was error-free (without a lot more work)
            /// </summary>
            GuiExit = -1,
        }

        /// <summary>
        /// This program will open the settings file, creating one if it doesn't exist.
        /// Then it will locate the savegame, perform a backup and, finally, modify it.
        /// </summary>
        /// <param name="args">Arg[0], if it exists, should be the savegame file name and path.</param>
        /// <returns>0 for success, anything else for error</returns>
        [STAThread] // <-- Needed to run GUI elements within Windows
        static int Main(string[] args)
        {
            Settings programSettings = LoadDefaultSettingsFile();

            string SaveNameAndPathToUse = "";
            ErrorState saveFoundStatus = LocateSaveGameFile(args, out SaveNameAndPathToUse);
            if (saveFoundStatus != ErrorState.NoError)
            { return (int)saveFoundStatus; } // Early exit - error

            BackupOriginalSaveFile(SaveNameAndPathToUse);

            if (programSettings.UseGraphicalUserInterface)
            {
                RunGui(programSettings, SaveNameAndPathToUse);
                return (int)ErrorState.GuiExit;
            }
            else
            {
                ErrorState modificationStatus = PerformSaveModification(SaveNameAndPathToUse, programSettings);

                Console.WriteLine("Program completion code " + modificationStatus);
                return (int)modificationStatus;
            }
        }

        /// <summary>
        /// Loads the default settings file, creating one if it cannot be found
        /// </summary>
        /// <returns>The settings file</returns>
        private static Settings LoadDefaultSettingsFile()
        {
            Settings programSettings = null;
            do
            {
                if (File.Exists(SettingsFilePath) == false)
                {
                    Settings defaultSettings = new Settings();
                    defaultSettings.SaveToFile(SettingsFilePath);
                    defaultSettings = null;
                }
                programSettings = Settings.LoadFromFile(SettingsFilePath);

                Version oldVersion = new Version(programSettings.CreatedWithProgramVersion);
                Version currentVersion = Assembly.GetCallingAssembly().GetName().Version;
                if (oldVersion.Major != currentVersion.Major || oldVersion.Minor != currentVersion.Minor )
                {
                    programSettings = null;
                    File.Move(SettingsFilePath, SettingsFilePath + BackupAppend);
                    Console.WriteLine("Your settings file was out of date.\n  I have renamed it \"" + SettingsFilePath + BackupAppend + "' if you care to recover data from it.\n    It will be erased the next time you have an out of date settings file.");
                }
            } while (programSettings == null);
            return programSettings;
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
            } // If they didn't, check the MyDocuments default directory
            else if (Directory.Exists(SaveGamePath) && File.Exists(SaveGamePath + "\\" + SaveFileName))
            {
                SaveNameAndPathToUse = SaveGamePath + "\\" + SaveFileName;
                errorState = ErrorState.NoError;
            } // If it's none of those things, error. We cannot continue.
            else
            {
                Console.Error.WriteLine("Error: Unable to find default Save File location.");
                Console.Error.WriteLine("  Please enter a full file name and path for the 20XX save file as argument[0]");
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
        internal static ErrorState PerformSaveModification(string inputFileNameAndPath, Settings settings)
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
                                                  CongealItems(settings.BasicAugments, GetOriginalData(settings.DataLoreByteOffsets.BasicAugments, settings.DataSizes.BasicAugments, fileBytes))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.PrimaryWeapons,
                                                  CongealItems(settings.PrimaryWeapons, GetOriginalData(settings.DataLoreByteOffsets.PrimaryWeapons, settings.DataSizes.PrimaryWeapons, fileBytes))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.CoreAugs,
                                                  CongealItems(settings.CoreAugs, GetOriginalData(settings.DataLoreByteOffsets.CoreAugs, settings.DataSizes.CoreAugs, fileBytes))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.Protoypes,
                                                  CongealItems(settings.Prototypes, GetOriginalData(settings.DataLoreByteOffsets.Protoypes, settings.DataSizes.Protoypes, fileBytes))));

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
                                                CongealItems(settings.BasicAugments, GetOriginalData(settings.UnlockByteOffsets.BasicAugments, settings.DataSizes.BasicAugments, fileBytes))));
            Unlocks.Add(new Tuple<long, byte[]>(settings.UnlockByteOffsets.PrimaryWeapons,
                                                CongealItems(settings.PrimaryWeapons, GetOriginalData(settings.UnlockByteOffsets.PrimaryWeapons, settings.DataSizes.PrimaryWeapons, fileBytes))));

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
        /// Gets the data sub-array at the specified offset
        /// </summary>
        /// <param name="offset">Where in the file is this modification</param>
        /// <param name="size">How much to copy out</param>
        /// <param name="data">The contents of the save file</param>
        public static byte[] GetOriginalData(long offset, long size, byte[] data)
        {
            byte[] subData = new byte[size];
            Array.Copy(data, (long)offset, subData, (long)0, (long)subData.Length);
            return subData;
        }

        /// <summary>
        /// Congeals all item bit-flags for a specific section of the output data
        /// </summary>
        /// <param name="itemsToCongeal">Here's the bit-flags and toggle-state of the items for this section</param>
        /// <param name="originalData">The original data bytes</param>
        /// <returns>The combined bit-flags for this file section</returns>
        private static byte[] CongealItems(IList<Item> itemsToCongeal, byte[] originalData)
        {
            BigInteger allItems = Settings.GetBigIntFromRawBytes(originalData);
            foreach (Item item in itemsToCongeal)
            {
                if (item.Availability == LockState.Unlocked)
                {
                    allItems |= Settings.GetAsBigInt(item.HexValue);
                }
                else if (item.Availability == LockState.Locked)
                {
                    allItems &= ~Settings.GetAsBigInt(item.HexValue);
                }
                else
                {
                    ; // do nothing, leave it as-is
                }
            }
            return Settings.GetRawBytesFromBigInt(allItems);
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
