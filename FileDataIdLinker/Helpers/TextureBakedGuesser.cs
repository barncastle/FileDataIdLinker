using System.Collections.Generic;
using System.Linq;
using FileDataIdLinker.Databases;

namespace FileDataIdLinker.Helpers
{
    public class TextureBakedGuesser
    {
        public Dictionary<uint, string> FileNames;

        public TextureBakedGuesser() => FileNames = new Dictionary<uint, string>();

        public void Guess()
        {
            foreach (var entry in Database.CreatureDisplayInfoExtra)
            {
                if (Database.TextureFileData.ReverseMap.TryGetValue(entry.Key, out var ids))
                {
                    FileNames.Add(ids.First(), $"textures/bakednpctextures/creaturedisplayextra-{entry.Value.ID.ToString("00000")}{(entry.Value.IsHD ? "_hd" : "")}.blp");
                }
            }                    
        }
    }
}
