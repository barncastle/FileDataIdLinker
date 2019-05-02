using System;
using System.Threading.Tasks;

namespace FileDataIdLinker.Databases
{
    public static class Database
    {
        public static ListFile ListFile;

        public static ComponentModelFileData ComponentModelFileData;
        public static ComponentTextureFileData ComponentTextureFileData;
        public static CreatureDisplayInfoExtra CreatureDisplayInfoExtra;
        public static CreatureModelData CreatureModelData;
        public static ItemAppearance ItemAppearance;
        public static ItemDisplayInfo ItemDisplayInfo;
        public static ItemDisplayInfoMaterialRes ItemDisplayInfoMaterialRes;
        public static LightSkybox LightSkybox;
        public static ModelFileData ModelFileData;
        public static SpellVisualEffectName SpellVisualEffectName;
        public static SpellVisualKitAreaModel SpellVisualKitAreaModel;
        public static TextureFileData TextureFileData;

        public static void Load(string build)
        {
            ListFile = new ListFile();
            ComponentModelFileData = new ComponentModelFileData(build);
            ComponentTextureFileData = new ComponentTextureFileData(build);
            CreatureDisplayInfoExtra = new CreatureDisplayInfoExtra(build);
            CreatureModelData = new CreatureModelData(build);
            ItemAppearance = new ItemAppearance(build);
            ItemDisplayInfo = new ItemDisplayInfo(build);
            ItemDisplayInfoMaterialRes = new ItemDisplayInfoMaterialRes(build);
            LightSkybox = new LightSkybox(build);
            ModelFileData = new ModelFileData(build);
            SpellVisualEffectName = new SpellVisualEffectName(build);
            SpellVisualKitAreaModel = new SpellVisualKitAreaModel(build);
            TextureFileData = new TextureFileData(build);

            // high priority lookups
            Task.WaitAll(
                ListFile.Load(),
                ModelFileData.Load(),
                TextureFileData.Load()
            );

            Task.WaitAll(
                ComponentModelFileData.Load(),
                ComponentTextureFileData.Load(),
                CreatureDisplayInfoExtra.Load(),
                CreatureModelData.Load(),
                ItemAppearance.Load(),
                ItemDisplayInfo.Load(),
                ItemDisplayInfoMaterialRes.Load(),
                LightSkybox.Load(),
                SpellVisualEffectName.Load(),
                SpellVisualKitAreaModel.Load()
            );

            Console.WriteLine("Loaded DBs");
        }

    }
}
