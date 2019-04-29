using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileDataIdLinker.Constants;
using FileDataIdLinker.Databases;
using FileDataIdLinker.Models;

namespace FileDataIdLinker.Readers.M2
{
    public class M2Reader
    {
        public bool Read(string filename, out M2Model model)
        {
            if (!uint.TryParse(Path.GetFileNameWithoutExtension(filename).Split('_').Last(), out uint fileDataId))
            {
                model = null;
                return false;
            }

            model = new M2Model(fileDataId);

            using (var fs = File.OpenRead(filename))
            using (var br = new BinaryReader(fs))
            {
                if ((M2Chunk)br.ReadUInt32() != M2Chunk.MD21)
                    return false;

                br.BaseStream.Position = 0;

                while (fs.Position < fs.Length)
                {
                    M2Chunk chunkId = (M2Chunk)br.ReadUInt32();
                    int chunkSize = br.ReadInt32();

                    switch (chunkId)
                    {
                        case M2Chunk.MD21:
                            ReadMD20(br, model);
                            br.BaseStream.Position = 8 + chunkSize;
                            break;
                        case M2Chunk.LDV1:
                            ReadLDV1(br, model);
                            break;
                        case M2Chunk.SFID:
                            ReadSFID(br, model);
                            break;
                        case M2Chunk.TXID:
                            ReadTXID(br, model, chunkSize);
                            break;
                        case M2Chunk.AFID:
                            ReadAFID(br, model, chunkSize);
                            break;
                        case M2Chunk.PFID:
                            model.PhysFileId = br.ReadUInt32();
                            break;
                        case M2Chunk.SKID:
                            model.SkelFileId = br.ReadUInt32();
                            break;
                        case M2Chunk.BFID:
                            ReadBFID(br, model, chunkSize);
                            break;
                        default:
                            br.BaseStream.Position += chunkSize;
                            break;
                    }
                }
            }

            return true;
        }


        private void ReadMD20(BinaryReader br, M2Model model)
        {
            br.BaseStream.Position += 8;
            int nameSize = br.ReadInt32();
            int nameOffset = br.ReadInt32();

            br.BaseStream.Position += 52;
            model.ViewCount = br.ReadInt32();

            br.BaseStream.Position = nameOffset + 8;
            model.InternalName = Encoding.UTF8.GetString(br.ReadBytes(nameSize - 1));

            if (Database.ComponentModelFileData.TryGetValue(model.FileDataId, out var suffix))
                if (!model.InternalName.EndsWith(suffix, System.StringComparison.OrdinalIgnoreCase))
                    model.Suffix = suffix;
        }

        private void ReadLDV1(BinaryReader br, M2Model model)
        {
            br.ReadUInt16();
            model.LodCount = br.ReadUInt16() - 1;
            br.ReadSingle();
            br.ReadBytes(4);
            br.ReadUInt32();
        }

        private void ReadSFID(BinaryReader br, M2Model model)
        {
            var skinFileIds = Enumerable.Range(0, model.ViewCount).Select(x => br.ReadUInt32()).ToList();
            skinFileIds.RemoveAll(x => x == 0);
            model.SkinFileIds = skinFileIds.ToArray();

            var lodSkinFileIds = Enumerable.Range(0, model.LodCount).Select(x => br.ReadUInt32()).ToList();
            lodSkinFileIds.RemoveAll(x => x == 0);
            model.LodSkinFileIds = lodSkinFileIds.ToArray();
        }

        private void ReadTXID(BinaryReader br, M2Model model, int chunkSize)
        {
            var txids = Enumerable.Range(0, chunkSize / 4).Select(x => new Texture(br.ReadUInt32())).ToHashSet();
            txids.RemoveWhere(x => x.FileDataId == 0);
            model.TextureFileIds = txids;
        }

        private void ReadAFID(BinaryReader br, M2Model model, int chunkSize)
        {
            int count = chunkSize / 8;
            model.AnimFiles = new AnimInfo[count];

            for (int i = 0; i < count; i++)
            {
                model.AnimFiles[i] = new AnimInfo()
                {
                    AnimId = br.ReadUInt16(),
                    SubAnimId = br.ReadUInt16(),
                    AnimFileId = br.ReadUInt32(),
                };
            }
        }

        private void ReadBFID(BinaryReader br, M2Model model, int chunkSize)
        {
            List<uint> bfids = Enumerable.Range(0, chunkSize / 4).Select(x => br.ReadUInt32()).ToList();
            bfids.RemoveAll(x => x == 0);
            model.BoneFileIds = bfids.ToArray();
        }

    }
}
