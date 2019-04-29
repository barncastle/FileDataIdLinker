using System.Collections.Generic;

namespace FileDataIdLinker.Models
{
    public class Item
    {
        /// <summary>
        /// ModelFileData.Value
        /// </summary>
        public HashSet<uint> ModelResourcesID;
        /// <summary>
        /// TextureFileData.Value
        /// </summary>
        public HashSet<uint> ModelMaterialResourcesID;

        public Item()
        {
            ModelResourcesID = new HashSet<uint>();
            ModelMaterialResourcesID = new HashSet<uint>();
        }
    }
}
