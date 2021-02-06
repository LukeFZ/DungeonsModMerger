using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using LocResEditor.LocresTools;
using Newtonsoft.Json;
using PakReader;

namespace LevelModMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Length != 0)
            {
                path = args[0];
            }
            else
            {
                Console.WriteLine("Drag the folder containing the .levelmod files you want to merge into the Window and press enter.");
                path = Console.ReadLine();
            }

            var locrespath = Path.Join(path, "Dungeons", "Content", "Localization", "Game", "en");
            var levelspath = Path.Join(path, "Dungeons", "Content", "data", "lovika", "levels");
            var labelspath = Path.Join(path, "Dungeons", "Content", "Decor", "Text");

            foreach (string modfilePath in Directory.GetFiles(path, "*.levelmod", SearchOption.TopDirectoryOnly))
            {
                var modname = Path.GetFileNameWithoutExtension(modfilePath);
                ZipFile.ExtractToDirectory(modfilePath,path,true);
                File.Move(Path.Join(path,"game.json"), Path.Join(path, $"{modname}.json"),true);

            }

            Dictionary<string,string> mergedModStrings = new Dictionary<string, string>();

            foreach (string modjsonPath in Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly))
            {
                var jsonstring = File.ReadAllText(modjsonPath);
                var modStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonstring);
                modStrings.ToList().ForEach(x => mergedModStrings[x.Key] = x.Value);
            }

            var locreshandle = new LocresHandling();
            var locresdict = JsonConvert.DeserializeObject<IndexedDictionary<string, IndexedDictionary<string, BinaryExtensions.FStringWithUCS>>>(File.ReadAllText("./Game.json"));

            mergedModStrings.ToList().ForEach(x => locresdict["creeperwoodsLabels"][x.Key] = new BinaryExtensions.FStringWithUCS()
            {
                FString = x.Value,
                LoadUCS2Char = false,
                RefNumber = 1
            });

            List<string> labelList = new List<string>();
            labelList.Add("Key,SourceString");
                
            foreach (KeyValuePair<string,BinaryExtensions.FStringWithUCS> text in locresdict["creeperwoodsLabels"])
            {
                labelList.Add($"{text.Key},{text.Value.FString}");
            }

            Directory.CreateDirectory(labelspath);

            var temp = File.Create(Path.Join(labelspath,"creeperwoodsLabels.csv"));
            temp.Close();
            File.AppendAllLines(Path.Join(labelspath,"creeperwoodsLabels.csv"),labelList);

            Directory.CreateDirectory(locrespath);
            LocresHandling.LocResWriter(
                new BinaryWriter(File.Open(Path.Join(locrespath, "Game.locres"), FileMode.Create)),
                JsonConvert.SerializeObject(locresdict));


            Dictionary<string,string> loaderDict = new Dictionary<string, string>();
            int beginningSeed = 100000;
            foreach (string levelfile in Directory.GetFiles(levelspath,"*.json",SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileNameWithoutExtension(levelfile);
                loaderDict.Add(beginningSeed.ToString(),name);
                beginningSeed++;
            }

            var temp2 = File.Create("./customlevels.json");
            temp2.Close();
            File.AppendAllText("./customlevels.json",JsonConvert.SerializeObject(loaderDict,Formatting.Indented));

            if (File.Exists("./Tools/u4pak.py"))
            {
                run_cmd("python", $"{Directory.GetCurrentDirectory() + "/Tools/u4pak.py"} pack {Directory.GetCurrentDirectory() + "/MergedMods.pak"} Dungeons", path);
                Console.WriteLine("Conversion successfully completed!");
            }
            else
            {
                Console.WriteLine("Conversion successful! As u4pak was not found, you need to create the .pak file yourself.");
                Console.Read();
            }
        }

        private static void run_cmd(string cmd, string args,string workingdirectory)
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = cmd,
                Arguments = args,
                WorkingDirectory = workingdirectory,
                UseShellExecute = true
            };

            Process.Start(start);

        }
    }
}
