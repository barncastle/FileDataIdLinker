using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FileDataIdLinker.Databases
{
    public class SpellVisualEffectName : HashSet<uint>
    {
        private const string Url = "https://wow.tools/api/export/?name=spellvisualeffectname&build=";
        private readonly string Build;

        public SpellVisualEffectName(string build)
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
                    Add(uint.Parse(record.ModelFileDataID));
                }
            }
        }
    }
}
