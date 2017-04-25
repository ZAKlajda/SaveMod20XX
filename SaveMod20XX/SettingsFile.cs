using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SaveMod20XX
{
    [XmlRoot("SaveMod20XXSettings")]
    public class Settings
    {
        /// <summary>
        /// The version of the app that created this settings file
        /// </summary>
        public string CreatedWithProgramVersion { get; set; } = Assembly.GetCallingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Whether or not to run in GUI mode
        /// </summary>
        public bool UseGraphicalUserInterface { get; set; } = true;

        /// <summary>
        /// The file offsets for these sections
        /// </summary>
        public OffsetsAndLengths UnlockByteOffsets { get; set; } = new OffsetsAndLengths { BasicAugments = 0xC8, PrimaryWeapons = 0xB1, CoreAugs = -1, Protoypes = -1 };

        /// <summary>
        /// The file offsets for these sections
        /// </summary>
        public OffsetsAndLengths DataLoreByteOffsets { get; set; } = new OffsetsAndLengths { BasicAugments = 0x114, PrimaryWeapons = 0xFD, CoreAugs = 0x12A, Protoypes = 0x124 };

        /// <summary>
        /// The size of each of these sections, in bytes, for this version
        /// </summary>
        public OffsetsAndLengths DataSizes { get; set; } = new OffsetsAndLengths { BasicAugments = 10, PrimaryWeapons = 2, CoreAugs = 3, Protoypes = 2 };


        /// <summary>
        /// Note that these hex values can be arbitrarily long. You're welcome.
        /// </summary>
        [XmlIgnore] // <-- This is serialized elsewhere
        public List<Item> BasicAugments = new List<Item>()
        {
            new Item { Name = "PowerEnhancer"      , HexValue = "0x10000000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x20000000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "HeartContainer"     , HexValue = "0x40000000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "BlueLander"         , HexValue = "0x80000000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "PlumberHat"         , HexValue = "0x01000000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x02000000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x04000000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x08000000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00100000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ForcemetalShell"    , HexValue = "0x00200000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "XCalibur"           , HexValue = "0x00400000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "GlassCannon"        , HexValue = "0x00800000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00010000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "BrainFoodLunch"     , HexValue = "0x00020000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Zephyr"             , HexValue = "0x00040000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00080000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ScrapmetalScavenger", HexValue = "0x00001000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "SevenLeafClover"    , HexValue = "0x00002000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "SpilloverMatrix"    , HexValue = "0x00004000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "HealthNut"          , HexValue = "0x00008000000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000100000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000200000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "VitalityScavenger"  , HexValue = "0x00000400000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "EnergyScavenger"    , HexValue = "0x00000800000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "NutReplicator"      , HexValue = "0x00000010000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "MinimechOGrinder"   , HexValue = "0x00000020000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Murderdrone"        , HexValue = "0x00000040000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "SkitterySmuggler"   , HexValue = "0x00000080000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ChargedNuts"        , HexValue = "0x00000001000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000002000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000004000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000008000000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Gapminder"          , HexValue = "0x00000000100000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "TheRebeginner"      , HexValue = "0x00000000200000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ShockwaveSidekick"  , HexValue = "0x00000000400000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Vendsmasher"        , HexValue = "0x00000000800000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ShinierToken"       , HexValue = "0x00000000010000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000000020000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ReFlapp"            , HexValue = "0x00000000040000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "TinyFlamespewer"    , HexValue = "0x00000000080000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ArmorativePlating"  , HexValue = "0x00000000001000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000000002000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ThriftActuator"     , HexValue = "0x00000000004000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "CrisisOverdrive"    , HexValue = "0x00000000008000000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "NutsavingStringwire", HexValue = "0x00000000000100000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ArmorNut"           , HexValue = "0x00000000000200000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "RegenerativePlating", HexValue = "0x00000000000400000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "EnerativePlating"   , HexValue = "0x00000000000800000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ArmorBloom"         , HexValue = "0x00000000000010000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Meganut"            , HexValue = "0x00000000000020000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "PureFlame"          , HexValue = "0x00000000000040000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ForgottenMemento"   , HexValue = "0x00000000000080000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "CrisisTimestopper"  , HexValue = "0x00000000000001000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "SystemRestore"      , HexValue = "0x00000000000002000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ArmorSpreader"      , HexValue = "0x00000000000004000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Choicebooster"      , HexValue = "0x00000000000008000000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Vibroreserve"       , HexValue = "0x00000000000000100000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000000000000200000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ArmorScavenger"     , HexValue = "0x00000000000000400000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "EntropyLock"        , HexValue = "0x00000000000000800000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "BracerofBattle"     , HexValue = "0x00000000000000010000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Hypersash"          , HexValue = "0x00000000000000020000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000000000000040000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Megaheart"          , HexValue = "0x00000000000000080000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "TheVolunteer"       , HexValue = "0x00000000000000001000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Thrillseeker"       , HexValue = "0x00000000000000002000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "QuantumSpook"       , HexValue = "0x00000000000000004000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "HoardersMight"      , HexValue = "0x00000000000000008000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "StrikingFeather"    , HexValue = "0x00000000000000000100", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ThunderousBoon"     , HexValue = "0x00000000000000000200", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "BandofMight"        , HexValue = "0x00000000000000000400", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "MistimedProtector"  , HexValue = "0x00000000000000000800", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "WorldSlug"          , HexValue = "0x00000000000000000010", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "LeafmetalPlating"   , HexValue = "0x00000000000000000020", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ZookeepersSigil"    , HexValue = "0x00000000000000000040", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = ""                   , HexValue = "0x00000000000000000080", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ContractorPlus"     , HexValue = "0x00000000000000000001", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ContractorOmega"    , HexValue = "0x00000000000000000002", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "MixmatchMastery"    , HexValue = "0x00000000000000000004", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "ChargingMagnet"     , HexValue = "0x00000000000000000008", Availability = LockState.NoChange, Lockable = true },
        };

        [XmlIgnore] // <-- This is serialized elsewhere
        public List<Item> PrimaryWeapons = new List<Item>()
        {
            new Item { Name = "WaveBeam"               , HexValue = "0x1000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "Scatterblast"           , HexValue = "0x2000", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "NBuster"                , HexValue = "0x0200", Availability = LockState.Unlocked, Lockable = false },
            new Item { Name = "StarBeam"               , HexValue = "0x0400", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "TheForkalator"          , HexValue = "0x0800", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "SpinningGlaive"         , HexValue = "0x0010", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "SharpSharpSpear"        , HexValue = "0x0020", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "ASaber"                 , HexValue = "0x0002", Availability = LockState.Unlocked, Lockable = false },
            new Item { Name = "RipplingAxe"            , HexValue = "0x0004", Availability = LockState.NoChange, Lockable = true },
            new Item { Name = "PlasmaBlender"          , HexValue = "0x0008", Availability = LockState.NoChange, Lockable = false },
        };

        [XmlIgnore] // <-- This is serialized elsewhere
        public List<Item> CoreAugs = new List<Item>()
        {
            new Item { Name = "ArmatortsShell"    , HexValue = "0x100000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "OxjacksGuile"      , HexValue = "0x300000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "DracopentsPride"   , HexValue = "0x400000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "OwlhawksReign"     , HexValue = "0x800000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "ArmatortsMomentum" , HexValue = "0x001000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "OxjacksBlitz"      , HexValue = "0x002000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "DracopentsBound"   , HexValue = "0x004000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "OwlhawksFeather"   , HexValue = "0x008000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "ArmatortsDome"     , HexValue = "0x000100", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "OxjacksKen"        , HexValue = "0x000200", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "DracopentsFang"    , HexValue = "0x000400", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "OwlhawksFocus"     , HexValue = "0x000800", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "ArmatortsPound"    , HexValue = "0x000001", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "OxjacksFury"       , HexValue = "0x000002", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "DracopentsClaw"    , HexValue = "0x000004", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "OwlhawksTalon"     , HexValue = "0x000008", Availability = LockState.NoChange, Lockable = false },
        };

        [XmlIgnore] // <-- This is serialized elsewhere
        public List<Item> Prototypes = new List<Item>()
        {

            new Item { Name = "BrutishAugmentation", HexValue = "0x1000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "FocusingSagelens"   , HexValue = "0x2000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "EarthmetalPlating"  , HexValue = "0x4000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "InterestingTimes"   , HexValue = "0x8000", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "SanityConverter"    , HexValue = "0x0200", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "ViolenceEnhancer"   , HexValue = "0x0400", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "DefiantDecree"      , HexValue = "0x0800", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "UnchargingForce"    , HexValue = "0x0001", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "FinalShell"         , HexValue = "0x0002", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "ZookeepersSigil"    , HexValue = "0x0004", Availability = LockState.NoChange, Lockable = false },
            new Item { Name = "ConsumingFury"      , HexValue = "0x0008", Availability = LockState.NoChange, Lockable = false },
        };


        #region Serialization Helpers

        /// <summary>
        /// This is for ease of serialization;
        /// Collections and arrays serialize differently, and I prefer the list behavior
        /// </summary>
        [XmlArray("BasicAugments")]
        [XmlArrayItem("Item")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Item[] BasicAugments_Serialization
        {
            get
            {
                return BasicAugments.ToArray();
            }
            set
            {
                BasicAugments = value.ToList();
            }
        }


        /// <summary>
        /// This is for ease of serialization;
        /// Collections and arrays serialize differently, and I prefer the list behavior
        /// </summary>
        [XmlArray("PrimaryWeapons")]
        [XmlArrayItem("Item")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Item[] PrimaryWeapons_Serialization
        {
            get
            {
                return PrimaryWeapons.ToArray();
            }
            set
            {
                PrimaryWeapons = value.ToList();
            }
        }


        /// <summary>
        /// This is for ease of serialization;
        /// Collections and arrays serialize differently, and I prefer the list behavior
        /// </summary>
        [XmlArray("CoreAugs")]
        [XmlArrayItem("Item")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Item[] CoreAugs_Serialization
        {
            get
            {
                return CoreAugs.ToArray();
            }
            set
            {
                CoreAugs = value.ToList();
            }
        }


        /// <summary>
        /// This is for ease of serialization;
        /// Collections and arrays serialize differently, and I prefer the list behavior
        /// </summary>
        [XmlArray("Prototypes")]
        [XmlArrayItem("Item")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Item[] Prototypes_Serialization
        {
            get
            {
                return Prototypes.ToArray();
            }
            set
            {
                Prototypes = value.ToList();
            }
        }

        #endregion Serialization Helpers


        /// <summary>
        /// This catches out of date or incorrect elements
        /// </summary>
        [XmlAnyElement]
        public List<XmlElement> CatchAll { get; set; } = new List<XmlElement>();


        /// <summary>
        /// Takes a hexadecimal string and turns it into a <see cref="BigInteger"/> to make performing bitwise operations easy
        /// </summary>
        /// <remarks>
        /// Not to be confused with "GetBigAssInt(...)"
        /// </remarks>
        public static BigInteger GetAsBigInt(string hexValue)
        {
            string valToUse = hexValue.ToLower().Trim();
            if (valToUse.StartsWith("0x"))
            { valToUse = valToUse.Substring(2); }

            return BigInteger.Parse(valToUse, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        /// <summary>
        /// Simply returns a <see cref="BigInteger"/> into a hexadecimal string
        /// </summary>
        public static string GetHexStringFromBigInt(BigInteger bigInt)
        {
            return "0x" + bigInt.ToString("X");
        }

        /// <summary>
        /// Returns a <see cref="BigInteger"/> into a (big-endian) byte[]
        /// </summary>
        public static byte[] GetRawBytesFromBigInt(BigInteger bigInt)
        {
            return bigInt.ToByteArray().Reverse().ToArray(); // They always come out in little-endian, which is worthless
        }
        
        /// <summary>
        /// Takes a (big-endian) byte[] and turns it into a <see cref="BigInteger"/> to make performing bitwise operations easy
        /// </summary>
        public static BigInteger GetBigIntFromRawBytes(byte[] rawBytes)
        {
            return new BigInteger(rawBytes.Reverse().ToArray()); // It requires them in little-endian, which is ridiculous
        }

        /// <summary>
        /// Saves this class and it's current settings out to a file
        /// </summary>
        public void SaveToFile(string FileNameAndPath)
        {
            using (MemoryStream ms = new MemoryStream())
            using (FileStream fs = new FileStream(FileNameAndPath, FileMode.Create, FileAccess.Write))
            {
                // We serialize it to a memory stream, first, so we can add comments
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                ser.Serialize(ms, this);
                ms.Position = 0;

                // This lets us add comments
                XDocument xd = XDocument.Load(ms);
                xd.Root.AddBeforeSelf(new XComment("NOTE: The Items' Availability setting can be 'NoChange', 'Unlocked', or 'Locked'. NoChange will retain the existing value in the save file, if able."));

                // When done, we then actually save it out
                xd.Save(fs);
            }
        }

        /// <summary>
        /// Loads this class with settings from a file
        /// </summary>
        public static Settings LoadFromFile(string FileNameAndPath)
        {
            using (FileStream fs = new FileStream(FileNameAndPath, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));

                Settings loadedSettings = (Settings)ser.Deserialize(fs);

                return loadedSettings;
            }
        }
    }


    public class OffsetsAndLengths
    {
        [XmlAttribute]
        public long BasicAugments { get; set; }
        [XmlAttribute]
        public long PrimaryWeapons { get; set; }
        [XmlAttribute]
        public long CoreAugs { get; set; }
        [XmlAttribute]
        public long Protoypes { get; set; }
    }

    public enum LockState
    {
        NoChange,
        Locked,
        Unlocked,
    }

    public class Item
    {
        /// <summary>
        /// Note that this hex value can be arbitrarily long. You're welcome.
        /// </summary>
        [XmlAttribute]
        public string HexValue { get; set; }

        /// <summary>
        /// Whether this thing should be locked, unlocked, or left alone
        /// </summary>
        [XmlAttribute]
        public LockState Availability { get; set; }

        /// <summary>
        /// Is this a thing that can be locked/unlocked?
        /// </summary>
        [XmlAttribute]
        public bool Lockable { get; set; }

        /// <summary>
        /// What do you call this thing?
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
    }
}