using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FileDataIdLinker.Databases
{
    public class LightSkybox : HashSet<uint>
    {
        private const string Url = "https://wow.tools/api/export/?name=lightskybox&build=";
        private readonly string Build;

        public LightSkybox(string build)
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
                    Add(uint.Parse(record.SkyboxFileDataID));
                    Add(uint.Parse(record.CelestialSkyboxFileDataID));
                }
            }
        }
    }
}
