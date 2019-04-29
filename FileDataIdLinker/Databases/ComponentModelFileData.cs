using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FileDataIdLinker.Constants;

namespace FileDataIdLinker.Databases
{
    /// <summary>
    /// (FileDataID, Suffix)
    /// </summary>
    public class ComponentModelFileData : Dictionary<uint, string>
    {
        private const string Url = "https://wow.tools/api/export/?name=componentmodelfiledata&build=";
        private readonly string Build;

        public ComponentModelFileData(string build)
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
                    ComponentGender gender = (ComponentGender)byte.Parse(record.GenderIndex);
                    ComponentRace race = (ComponentRace)byte.Parse(record.RaceID);

                    if (race == ComponentRace.None)
                        Add(id, "");
                    else
                        Add(id, $"_{race}_{gender}");
                }
            }
        }
    }
}
