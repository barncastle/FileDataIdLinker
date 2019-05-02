using System.Collections.Generic;
using FileDataIdLinker.Constants;
using FileDataIdLinker.Databases;
using System.Linq;
using System.IO;

namespace FileDataIdLinker.Helpers
{
    public class TextureComponentGuesser
    {
        public Dictionary<uint, string> FileNames;

        public void Guess()
        {
            FileNames = new Dictionary<uint, string>();

            var t = from ctf in Database.ComponentTextureFileData
                    join tfd in Database.TextureFileData on ctf.Key equals tfd.Key
                    join idir in Database.ItemDisplayInfoMaterialRes on tfd.Value equals idir.Key
                    join ia in Database.ItemAppearance on idir.Value.ItemDisplayInfoID equals ia.Key
                    where !Database.ListFile.ContainsKey(ctf.Key) && Database.ListFile.ContainsKey(ia.Value)
                    select new 
                    {
                        TextureId = ctf.Key,
                        ComponentGender = ctf.Value,
                        ComponentSection = idir.Value.Section,
                        IconName = Database.ListFile[ia.Value],
                        Directory = DirLookup[idir.Value.Section],
                    };

            foreach(var texture in t)
            {
                var split = Path.GetFileNameWithoutExtension(texture.IconName).Split(new char[] { '_' }, 3);
                string Slot = split[1];
                string BaseName = split[2];

                string finalName = texture.Directory + BaseName + "_" + Slot + "_" + texture.ComponentSection + "_" + texture.ComponentGender + ".blp";
                FileNames[texture.TextureId] = finalName.Replace("\\", "/").ToLowerInvariant();
            }

        }


        private readonly Dictionary<ComponentSection, string> DirLookup = new Dictionary<ComponentSection, string>
        {
            { ComponentSection.AL , "item/texturecomponents/armlowertexture/"},
            { ComponentSection.AU , "item/texturecomponents/armuppertexture/"},
            { ComponentSection.FO , "item/texturecomponents/foottexture/"},
            { ComponentSection.HA , "item/texturecomponents/handtexture/"},
            { ComponentSection.LL , "item/texturecomponents/leglowertexture/"},
            { ComponentSection.LU , "item/texturecomponents/leguppertexture/"},
            { ComponentSection.PR , "item/texturecomponents/accessorytexture/"},
            { ComponentSection.TL , "item/texturecomponents/torsolowertexture/"},
            { ComponentSection.TU , "item/texturecomponents/torsouppertexture/"},
        };
    }
}
