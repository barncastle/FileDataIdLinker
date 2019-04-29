using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileDataIdLinker.Constants;
using FileDataIdLinker.Databases;
using FileDataIdLinker.Models;

namespace FileDataIdLinker.Helpers
{
    public class TextureGuesser
    {
        public Dictionary<uint, string> FileNames;
        private const int MinSuffixLen = 4;

        public TextureGuesser() => FileNames = new Dictionary<uint, string>();


        public void Guess(IEnumerable<M2Model> models)
        {
            var textures = models.SelectMany(x => x.TextureFileIds).ToHashSet();

            foreach (var texture in textures)
            {
                var owningmodels = models.Where(x => x.TextureFileIds.Contains(texture));

                if (IsComponentTexture(texture.FileDataId, owningmodels))
                    continue;

                var names = owningmodels.Select(x => $"{x.FullName}-{texture.FileDataId}.blp").ToArray();

                if (names.Length == 1)
                    FileNames.Add(texture.FileDataId, names[0]);
                else
                    FileNames.Add(texture.FileDataId, $"{LongestCommonSuffix(names)}{texture.FileDataId}.blp");
            }
        }

        private bool IsComponentTexture(uint textureId, IEnumerable<M2Model> models)
        {

            if (Database.TextureFileData.TryGetValue(textureId, out var material) &&
                Database.ItemDisplayInfoMaterial.TryGetValue(material, out var section) &&
                Database.ComponentTextureFileData.TryGetValue(textureId, out var gender))
            {
                string name = models.First().InternalName.Replace("collections_", "", true, CultureInfo.InvariantCulture);
                string suffix = $"_{section}_{gender}-{textureId}.blp".ToLowerInvariant();
                string prefix;

                switch (section)
                {
                    case ComponentSection.AL:
                        prefix = "item/texturecomponents/armlowertexture/";
                        break;
                    case ComponentSection.AU:
                        prefix = "item/texturecomponents/armuppertexture/";
                        break;
                    case ComponentSection.FO:
                        prefix = "item/texturecomponents/foottexture/";
                        break;
                    case ComponentSection.HA:
                        prefix = "item/texturecomponents/handtexture/";
                        break;
                    case ComponentSection.LL:
                        prefix = "item/texturecomponents/leglowertexture/";
                        break;
                    case ComponentSection.LU:
                        prefix = "item/texturecomponents/leguppertexture/";
                        break;
                    case ComponentSection.PR:
                        prefix = "item/texturecomponents/accessorytexture/";
                        break;
                    case ComponentSection.TL:
                        prefix = "item/texturecomponents/torsolowertexture/";
                        break;
                    case ComponentSection.TU:
                        prefix = "item/texturecomponents/torsouppertexture/";
                        break;
                    default:
                        return false;
                }

                FileNames.Add(textureId, prefix + name + suffix);
                return true;
            }

            return false;
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
