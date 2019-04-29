using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FileDataIdLinker.Databases
{
    /// <summary>
    /// (FileDataID, MaterialResourcesID)
    /// </summary>
    public class TextureFileData : Dictionary<uint, uint>
    {
        private const string Url = "https://wow.tools/api/export/?name=texturefiledata&build=";
        private readonly string Build;

        public Dictionary<uint, HashSet<uint>> ReverseMap;

        public TextureFileData(string build)
        {
            Build = build;
            ReverseMap = new Dictionary<uint, HashSet<uint>>();
        }

        public async Task Load()
        {
            HttpWebRequest req = WebRequest.CreateHttp(Url + Build);
            using (HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync())
            using (var respStream = resp.GetResponseStream())
            using (var sr = new StreamReader(respStream))
            using (var csv = new CsvHelper.CsvReader(sr))
            {
                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<dynamic>();

                    uint fileDataId = uint.Parse(record.FileDataID);
                    uint materialResourcesId = uint.Parse(record.MaterialResourcesID);

                    Add(fileDataId, materialResourcesId);

                    if (!ReverseMap.ContainsKey(materialResourcesId))
                        ReverseMap[materialResourcesId] = new HashSet<uint>();
                    ReverseMap[materialResourcesId].Add(fileDataId);
                }
            }
        }
    }
}
