using System.Collections.Generic;
using System.Linq;
using FileDataIdLinker.Databases;

namespace FileDataIdLinker.Models
{
    public class M2Model
    {
        public uint FileDataId;
        public string Directory;
        public string Suffix;
        public string FullName => Directory + InternalName + Suffix;
        public Dictionary<uint, string> FileNames { get; private set; }

        public string InternalName;
        public int ViewCount;
        public int LodCount;
        public uint[] SkinFileIds;
        public uint[] LodSkinFileIds;
        public HashSet<Texture> TextureFileIds;
        public uint[] BoneFileIds;
        public AnimInfo[] AnimFiles;
        public uint PhysFileId;
        public uint SkelFileId;
        public uint ModelResourceId;

        public M2Model(uint fileDataId)
        {
            FileDataId = fileDataId;
            Database.ModelFileData.TryGetValue(fileDataId, out ModelResourceId);

            FileNames = new Dictionary<uint, string>();
            SkinFileIds = new uint[0];
            LodSkinFileIds = new uint[0];
            TextureFileIds = new HashSet<Texture>();
            BoneFileIds = new uint[0];
            AnimFiles = new AnimInfo[0];
        }

        public void LoadAssociatedTextures()
        {
            if (ModelResourceId == 0)
                return;

            var items = Database.ItemDisplayInfo.FindAll(x => x.ModelResourcesID.Contains(ModelResourceId));
            var materialResIds = items.SelectMany(x => x.ModelMaterialResourcesID);
            foreach (var resId in materialResIds)
                if (Database.TextureFileData.ReverseMap.TryGetValue(resId, out var textureIds))
                    TextureFileIds.UnionWith(textureIds.Select(x => new Texture(x)));
        }

        public void GenerateFileNames()
        {
            if (string.IsNullOrWhiteSpace(Directory))
                return;

            FileNames.Clear();

            FileNames.Add(FileDataId, FullName + ".m2");

            for (int i = 0; i < SkinFileIds.Length; i++)
                FileNames.Add(SkinFileIds[i], FullName + i.ToString("00") + ".skin");
            for (int i = 0; i < LodSkinFileIds.Length; i++)
                FileNames.Add(LodSkinFileIds[i], FullName + "_lod" + i.ToString("00") + ".skin");
            for (int i = 0; i < AnimFiles.Length; i++)
                FileNames.Add(AnimFiles[i].AnimFileId, FullName + AnimFiles[i]);
            for (int i = 0; i < BoneFileIds.Length; i++)
                FileNames.Add(BoneFileIds[i], FullName + "_" + BoneFileIds[i].ToString("00") + ".bone");

            if (PhysFileId > 0)
                FileNames.Add(PhysFileId, FullName + ".phys");
            if (SkelFileId > 0)
                FileNames.Add(SkelFileId, FullName + ".skel");
        }
    }

    public struct AnimInfo
    {
        public uint AnimId;
        public uint SubAnimId;
        public uint AnimFileId;

        public override string ToString() => AnimId.ToString("0000") + "-" + SubAnimId.ToString("00") + ".anim";
    }
}
