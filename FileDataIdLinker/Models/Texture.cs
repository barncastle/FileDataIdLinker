using FileDataIdLinker.Databases;

namespace FileDataIdLinker.Models
{
    public class Texture
    {
        public uint FileDataId;
        public uint MaterialResourceId;

        public Texture(uint fileDataId)
        {
            FileDataId = fileDataId;
            Database.TextureFileData.TryGetValue(fileDataId, out MaterialResourceId);
        }

        public override int GetHashCode()
        {
            return FileDataId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Texture texture)
                return texture.FileDataId == FileDataId;

            return false;
        }

        public override string ToString() => FileDataId.ToString();
    }
}
