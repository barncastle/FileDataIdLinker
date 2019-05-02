using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FileDataIdLinker.Databases
{
    public class ListFile : Dictionary<uint, string>
    {
        private const string Url = "https://wow.tools/casc/listfile/download/csv/unverified";

        public async Task Load()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(new Uri(Url), "listfile.csv");
            }

            using (var sr = new StreamReader("listfile.csv"))
            {
                while (!sr.EndOfStream)
                {
                    var parts = (await sr.ReadLineAsync()).Split(';', 2);
                    Add(uint.Parse(parts[0]), parts[1]);
                }
            }
        }
    }
}
