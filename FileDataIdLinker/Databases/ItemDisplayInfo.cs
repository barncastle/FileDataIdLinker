using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FileDataIdLinker.Models;

namespace FileDataIdLinker.Databases
{
    public class ItemDisplayInfo : List<Item>
    {
        private const string Url = "https://wow.tools/api/export/?name=itemdisplayinfo&build=";
        private readonly string Build;

        public ItemDisplayInfo(string build)
        {
            Build = build;
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
                    var record = csv.GetRecord<dynamic>() as IDictionary<string, object>;

                    Add(new Item()
                    {
                        ModelResourcesID = new HashSet<uint> {
                             uint.Parse(record["ModelResourcesID[0]"].ToString()),
                             uint.Parse(record["ModelResourcesID[1]"].ToString())
                         },
                        ModelMaterialResourcesID = new HashSet<uint> {
                             uint.Parse(record["ModelMaterialResourcesID[0]"].ToString()),
                             uint.Parse(record["ModelMaterialResourcesID[1]"].ToString())
                         }
                    });
                }

                foreach (var record in this)
                {
                    record.ModelMaterialResourcesID.Remove(0);
                    record.ModelResourcesID.Remove(0);
                }
            }
        }
    }
}
