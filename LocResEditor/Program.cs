using System;
using System.IO;
using LocResEditor.LocresTools;
using Newtonsoft.Json;

namespace LocResEditor
{
    class Program
    {

        static void Main(string[] args)
        {
            string path;
            string outputPath = null;
            if (args.Length != 0)
            {
                path = args[0];
                if (args.Length != 1)
                {
                    outputPath = args[1];
                }
            }
            else
            {
                Console.WriteLine("Drag either .locres or .json into the Window and press enter.");
                path = Console.ReadLine();
            }

            if (path.EndsWith(".json"))
            {
                outputPath ??= $"./{Path.GetFileNameWithoutExtension(path)}.locres";
                var jsoncontents = File.ReadAllText(path);
                var filestream = File.Open(outputPath, FileMode.Create);
                var filewriter = new BinaryWriter(filestream);

                LocresHandling.LocResWriter(filewriter, jsoncontents);
                filewriter.Close();
            }
            else if (path.EndsWith(".locres"))
            {
                outputPath ??= $"./{Path.GetFileNameWithoutExtension(path)}.json";
                var locres = new LocresHandling();

                locres.LocResReader(new BinaryReader(File.Open(path, FileMode.Open)));
                var jsoncontents = JsonConvert.SerializeObject(locres.LocresContentDict, Formatting.Indented);

                File.AppendAllText(outputPath, jsoncontents);
            }

            Console.WriteLine("Conversion successfully completed!");
            Console.WriteLine("Press enter to exit. ");
            Console.ReadLine();
        }
    }
}
