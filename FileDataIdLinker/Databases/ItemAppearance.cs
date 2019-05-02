using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileDataIdLinker.Databases
{
    public class ItemAppearance : Dictionary<uint, uint>
    {
        private const string Url = "https://wow.tools/api/export/?name=itemappearance&build=";
        private readonly string Build;

        public ItemAppearance(string build)
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

                    uint id = uint.Parse(record.ItemDisplayInfoID);
                    uint icon = uint.Parse(record.DefaultIconFileDataID);
                    this[id] = icon;
                }
            }
        }
    }
}
