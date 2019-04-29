using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FileDataIdLinker.Constants;
using FileDataIdLinker.Databases;
using FileDataIdLinker.Models;

namespace FileDataIdLinker.Helpers
{
    public class DirectoryGuesser
    {
        private readonly SuffixMap SuffixMap;
        private readonly HashSet<uint> CreatureOverrides;
        private const StringComparison Comp = StringComparison.OrdinalIgnoreCase;

        public DirectoryGuesser()
        {
            SuffixMap = new SuffixMap();

            CreatureOverrides = new HashSet<uint> { 2459324, 2617696, 2741458, 2828022, 2918046, 2976143 };
        }

        public void Guess(M2Model model)
        {
            if (IsCharacter(model))
                return;
            if (IsSky(model))
                return;
            if (IsSpell(model))
                return;
            if (IsTradeSkillNode(model))
                return;
            if (HasSuffix(model))
                return;
            if (IsCreature(model))
                return;

            Console.WriteLine($"Directory not found :: {model.FileDataId} {model.InternalName}");
            model.Directory = "/";
        }


        private bool IsCharacter(M2Model model)
        {
            Regex characterRegex = new Regex("(.*)(male|female)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var match = characterRegex.Match(model.InternalName);

            if (!match.Success)
                return false;

            if (Enum.TryParse(typeof(Races), match.Groups[1].Value, true, out var race))
            {
                model.Directory = $"character/{race}/{match.Groups[2].Value}/".ToLowerInvariant();
                return true;
            }

            return false;
        }

        private bool IsSky(M2Model model)
        {
            if (Database.LightSkybox.Contains(model.FileDataId))
            {
                model.Directory = "environments/stars/";
                return true;
            }

            return false;
        }

        private bool IsSpell(M2Model model)
        {
            if (Database.SpellVisualKitAreaModel.Contains(model.FileDataId) || Database.SpellVisualEffectName.Contains(model.FileDataId))
            {
                model.Directory = "spells/";
                return true;
            }

            if (model.InternalName.StartsWith("SpellVisualPlaceholder", Comp))
            {
                model.Suffix = "_" + model.FileDataId;
                model.Directory = "spells/";
                return true;
            }

            return false;
        }

        private bool IsTradeSkillNode(M2Model model)
        {
            if (model.InternalName.IndexOf("HerbNode", Comp) > -1 ||
               model.InternalName.IndexOf("MiningNode", Comp) > -1)
            {
                model.Directory = "world/skillactivated/tradeskillnodes/";
                return true;
            }

            return false;
        }

        private bool IsCreature(M2Model model)
        {
            if (Database.CreatureModelData.ContainsValue(model.FileDataId) || CreatureOverrides.Contains(model.FileDataId))
            {
                model.Directory = $"creature/{model.InternalName}/".ToLowerInvariant();
                return true;
            }

            return false;
        }

        private bool HasSuffix(M2Model model)
        {
            string[] nameParts = model.InternalName.Split('_');

            for (int i = 2; i > 0; i--)
            {
                if (nameParts.Length < i)
                    continue;

                string suffix = string.Join("_", nameParts.Take(i));
                if (SuffixMap.TryGetValue(suffix, out string dir))
                {
                    model.Directory = dir;
                    return true;
                }
            }

            var probableDir = SuffixMap.FirstOrDefault(x => model.InternalName.StartsWith(x.Key, Comp));
            if (probableDir.Key != null && probableDir.Key.Length > 2) // skip small sufficies
            {
                model.Directory = probableDir.Value;
                return true;
            }

            return false;
        }
    }
}
