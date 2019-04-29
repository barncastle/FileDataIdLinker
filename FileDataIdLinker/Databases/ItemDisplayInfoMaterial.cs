using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FileDataIdLinker.Constants;

namespace FileDataIdLinker.Databases
{
    /// <summary>
    /// ( MaterialResourcesID, ComponentSection )
    /// </summary>
    public class ItemDisplayInfoMaterial : Dictionary<uint, ComponentSection>
    {
        private const string Url = "https://wow.tools/api/export/?name=itemdisplayinfomaterialres&build=";
        private readonly string Build;

        public ItemDisplayInfoMaterial(string build)
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
                    var record = csv.GetRecord<dynamic>();

                    uint id = uint.Parse(record.MaterialResourcesID);
                    ComponentSection section = (ComponentSection)byte.Parse(record.ComponentSection);
                    this[id] = section;
                }
            }
        }
    }
}
