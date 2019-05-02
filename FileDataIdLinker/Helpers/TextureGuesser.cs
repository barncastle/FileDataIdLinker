using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileDataIdLinker.Models;

namespace FileDataIdLinker.Helpers
{
    public class TextureGuesser
    {
        public readonly TextureComponentGuesser TextureComponentGuesser;
        public readonly TextureBakedGuesser TextureBakedGuesser;
        public readonly Dictionary<uint, string> FileNames;
        private const int MinSuffixLen = 4;

        public TextureGuesser()
        {
            FileNames = new Dictionary<uint, string>();
            TextureComponentGuesser = new TextureComponentGuesser();
            TextureBakedGuesser = new TextureBakedGuesser();
        }


        public void Guess(IEnumerable<M2Model> models)
        {
            TextureComponentGuesser.Guess();
            TextureBakedGuesser.Guess();

            var textures = models.SelectMany(x => x.TextureFileIds).ToHashSet();
            textures.RemoveWhere(x => TextureComponentGuesser.FileNames.ContainsKey(x.FileDataId));
            textures.RemoveWhere(x => TextureBakedGuesser.FileNames.ContainsKey(x.FileDataId));

            foreach (var texture in textures)
            {
                var owningmodels = models.Where(x => x.TextureFileIds.Contains(texture));
                var names = owningmodels.Select(x => $"{x.FullName}.blp").ToArray();

                if (names.Length == 1)
                {
                    FileNames.Add(texture.FileDataId, names[0]);
                }                    
                else
                {
                    string suffix = LongestCommonSuffix(names);
                    if (string.IsNullOrWhiteSpace(suffix))
                        Console.WriteLine($"{texture} is used in multiple directories");
                    else
                        FileNames.Add(texture.FileDataId, $"{suffix}{texture.FileDataId}.blp");
                }

            }
        }

        private string LongestCommonSuffix(IEnumerable<string> filenames)
        {
            StringBuilder sb = new StringBuilder(0x40);

            string firstName = filenames.First();
            int minLength = filenames.Min(x => x.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (!filenames.All(x => x[i] == firstName[i]))
                    break;

                sb.Append(firstName[i]);
            }

            // grab the suffix and strip unwanted chars
            string result = Regex.Replace(sb.ToString(), "([_-]+[0-9a-z]?)$", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            int lastDirSep = result.LastIndexOf('/');

            // if filename suffix is too short return the directory
            if (result.Length - lastDirSep <= MinSuffixLen)
                return result.Substring(0, lastDirSep + 1);

            return result + "-";
        }
    }
}
