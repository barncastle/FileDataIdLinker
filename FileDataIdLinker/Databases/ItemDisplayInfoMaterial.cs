﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FileDataIdLinker.Constants;

namespace FileDataIdLinker.Databases
{
    /// <summary>
    /// ( MaterialResourcesID, ComponentSection )
    /// </summary>
    public class ItemDisplayInfoMaterialRes : Dictionary<uint, MaterialRes>
    {
        private const string Url = "https://wow.tools/api/export/?name=itemdisplayinfomaterialres&build=";
        private readonly string Build;

        public ItemDisplayInfoMaterialRes(string build)
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

                    this[uint.Parse(record.MaterialResourcesID)] = new MaterialRes()
                    {
                        ItemDisplayInfoID = uint.Parse(record.ItemDisplayInfoID),
                        Section = (ComponentSection)byte.Parse(record.ComponentSection)
                    };
                }
            }
        }
    }

    public class MaterialRes
    {
        public uint ItemDisplayInfoID;
        public ComponentSection Section;
    }
}
