using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileDataIdLinker.Databases;
using FileDataIdLinker.Helpers;
using FileDataIdLinker.Models;
using FileDataIdLinker.Readers.M2;

namespace FileDataIdLinker
{
    class Program
    {
        static List<M2Model> Models;
        static TextureGuesser TextureGuesser;
        static DirectoryGuesser DirectoryGuesser;

        static void Main(string[] args)
        {
            Database.Load("8.2.0.30262");

            TextureGuesser = new TextureGuesser();
            DirectoryGuesser = new DirectoryGuesser();

            Models = new List<M2Model>();
            EnumerateModels(@"D:\unknown");

            // fix duplicate names
            var matching = Models.GroupBy(x => x.FullName).Where(x => x.Count() > 1);
            foreach (var match in matching)
                foreach (var model in match)
                    model.Suffix += "-" + model.FileDataId;

            // generate filenames and guess textures
            Models.ForEach(x => x.GenerateFileNames());
            TextureGuesser.Guess(Models);

            DumpListfile();

            Console.ReadLine();
        }

        private static void EnumerateModels(string directory)
        {
            M2Reader m2Reader = new M2Reader();

            var files = Directory.EnumerateFiles(directory);
            foreach (var file in files)
            {
                if (m2Reader.Read(file, out var model))
                {
                    model.LoadAssociatedTextures();
                    DirectoryGuesser.Guess(model);
                    Models.Add(model);
                }
            }
        }

        private static void DumpListfile()
        {
            var filemap = Models.SelectMany(x => x.FileNames)
                                .Concat(TextureGuesser.FileNames)
                                .Concat(TextureGuesser.TextureComponentGuesser.FileNames)
                                .Concat(TextureGuesser.TextureBakedGuesser.FileNames)
                                .Where(x => !Database.ListFile.ContainsKey(x.Key))
                                .ToArray();

            var dupes = filemap.GroupBy(x => x.Value.ToLowerInvariant())
                               .Where(x => x.Count() > 1)
                               .SelectMany(x => x.Select(y => y.Key))
                               .ToHashSet();

            using (var fs = new StreamWriter("output.csv"))
            {
                int lastdotIdx;
                string name;

                foreach (var map in filemap)
                {
                    name = map.Value.ToLowerInvariant();

                    if (!dupes.Contains(map.Key))
                    {
                        fs.WriteLine(map.Key + ";" + name);
                    }
                    else
                    {
                        lastdotIdx = name.LastIndexOf('.');
                        fs.WriteLine(map.Key + ";" + name.Substring(0, lastdotIdx) + "_" + map.Key + Path.GetExtension(name));
                    }
                }
            }

        }
    }
}
