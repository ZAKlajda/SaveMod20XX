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
        const string FilePath = "Items20XX.Xml";
        const string BackupAppend = ".backup";

        static int Main(string[] args)
        {
            if (File.Exists(FilePath) == false)
            {
                Settings defaultSettings = new Settings();
                defaultSettings.SaveToFile(FilePath);
                defaultSettings = null;
            }

            Settings programSettings = Settings.LoadFromFile(FilePath);
            
            if(args.Length != 1 || File.Exists(args[0]) == false)
            {
                Console.Error.WriteLine("Please enter a full file name and path for the 20XX save file as argument 1.");
                Console.Error.WriteLine("You may drag-and-drop the desired save file onto this program for simplicity.");
                return 1;
            }

            BackupOriginalSaveFile(args[0]);
            
            return PerformSaveModification(args[0], programSettings);
        }

        private static void BackupOriginalSaveFile(string inputFileNameAndPath)
        {
            File.Copy(inputFileNameAndPath, inputFileNameAndPath + BackupAppend, true);
        }

        private static int PerformSaveModification(string inputFileNameAndPath, Settings settings)
        {
            FileInfo fi = new FileInfo(inputFileNameAndPath);
            byte[] fileBytes = new byte[fi.Length];
            fileBytes = ReadInFile(inputFileNameAndPath, fi);

            PerformUnlocks(fileBytes, settings);
            PerformDataLore(fileBytes, settings);

            WriteOutFile(inputFileNameAndPath, fileBytes);

            // Everything OK!
            return 0;
        }


        private static void PerformDataLore(byte[] fileBytes, Settings settings)
        {
            List<Tuple<long, byte[]>> DataLores = new List<Tuple<long, byte[]>>();
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.BasicAugments, Settings.GetRawBytesFromBigInt(CongealItems(settings.BasicAugments))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.PrimaryWeapons, Settings.GetRawBytesFromBigInt(CongealItems(settings.PrimaryWeapons))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.CoreAugs, Settings.GetRawBytesFromBigInt(CongealItems(settings.CoreAugs))));
            DataLores.Add(new Tuple<long, byte[]>(settings.DataLoreByteOffsets.Protoypes, Settings.GetRawBytesFromBigInt(CongealItems(settings.Prototypes))));

            DoAllModifications(fileBytes, DataLores);
        }

        private static void PerformUnlocks(byte[] fileBytes, Settings settings)
        {
            List<Tuple<long, byte[]>> Unlocks = new List<Tuple<long, byte[]>>();
            Unlocks.Add(new Tuple<long, byte[]>(settings.UnlockByteOffsets.BasicAugments, Settings.GetRawBytesFromBigInt(CongealItems(settings.BasicAugments))));
            Unlocks.Add(new Tuple<long, byte[]>(settings.UnlockByteOffsets.PrimaryWeapons, Settings.GetRawBytesFromBigInt(CongealItems(settings.PrimaryWeapons))));

            DoAllModifications(fileBytes, Unlocks);
        }


        public static void DoAllModifications(byte[] rawData, List<Tuple<long, byte[]>> mods)
        {
            foreach (var mod in mods)
            {
                DoModification(mod.Item1, rawData, mod.Item2);
            }
        }
            

        public static void DoModification(long offset, byte[] data, byte[] changed)
        {
            Array.Copy(changed, (long)0, data, offset, (long)changed.Length);
        }

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



        private static void WriteOutFile(string inputFileNameAndPath, byte[] fileBytes)
        {
            using (FileStream fs = new FileStream(inputFileNameAndPath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(fileBytes);
            }
        }

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
