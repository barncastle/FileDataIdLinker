using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FileDataIdLinker.Databases
{
    /// <summary>
    /// (MaterialResourcesID, IsHD)
    /// </summary>
    public class CreatureDisplayInfoExtra : Dictionary<uint, (uint ID, bool IsHD)>
    {
        private const string Url = "https://wow.tools/api/export/?name=creaturedisplayinfoextra&build=";
        private readonly string Build;

        public CreatureDisplayInfoExtra(string build)
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
                    uint id = uint.Parse(record.ID);
                    uint normal = uint.Parse(record.BakeMaterialResourcesID);
                    uint hd = uint.Parse(record.HDBakeMaterialResourcesID);

                    if (normal > 0)
                        this[normal] = (id, false);
                    if (hd > 0)
                        this[hd] = (id, true);
                }
            }
        }
    }

    public class InfoExtraEntry
    {
        public uint ID;
        public bool IsHD;
    }
}
