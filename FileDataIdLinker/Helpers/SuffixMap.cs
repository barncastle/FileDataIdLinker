using System;
using System.Collections.Generic;
using System.IO;

namespace FileDataIdLinker.Helpers
{
    public class SuffixMap : Dictionary<string, string>
    {
        public SuffixMap() : base(StringComparer.OrdinalIgnoreCase)
        {
            Load();
        }

        private void Load()
        {
            using (var sr = new StreamReader("SuffixMap.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string[] parts = sr.ReadLine().Split(';', 2);
                    Add(parts[0], parts[1]);
                }
            }
        }
    }
}
