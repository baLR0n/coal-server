using COAL.CORE.Core.Enums;
using COAL.CORE.Models;
using COAL.PES.Models;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace COAL.PES.Data
{
    public class PESDataReader
    {
        private static string playerPath = "/Player.bin";
        private static int playerBlock = 188;


        public SupportedGames Game { get; set; } = SupportedGames.PES_2019;

        /// <summary>
        /// Exports all players from the games database.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<List<Player>> ReadPlayersAsync(string path)
        {
            MemoryStream memory = null;

            byte[] file = await File.ReadAllBytesAsync(path + playerPath);
            byte[] stream = this.GetDataFromStream(file);
            memory = new MemoryStream(stream);

            var reader = new BinaryReader(memory);

            int index;
            int length = Convert.ToInt32((reader.BaseStream.Length - 4) / playerBlock);

            List<PESPlayer> tempList = new List<PESPlayer>();

            for (int i = 0; i < length; i++)
            {
                index = (i * playerBlock) + 4;
                reader.BaseStream.Position = index;

                // Read the 11 4-Byte uint values which contain all the player data.
                uint value1 = reader.ReadUInt32();
                uint value2 = reader.ReadUInt32();
                uint value3 = reader.ReadUInt32();
                uint value4 = reader.ReadUInt32();
                uint value5 = reader.ReadUInt32();
                uint value6 = reader.ReadUInt32();
                uint value7 = reader.ReadUInt32();
                uint value8 = reader.ReadUInt32();
                uint value9 = reader.ReadUInt32();
                uint value10 = reader.ReadUInt32();
                uint value11 = reader.ReadUInt32();

                // Read names
                reader.BaseStream.Position = index + 44;
                string japName = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(46)).TrimEnd('\0');

                reader.BaseStream.Position = index + 90;
                string shirtName = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(46)).TrimEnd('\0');

                reader.BaseStream.Position = index + 136;
                string name = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(46)).TrimEnd('\0');

                PESPlayer temp = new PESPlayer()
                {
                    Id = value1,
                    Name = name,
                    ShirtName = shirtName,
                    SetPieces = (value2 >> 26) + 40,
                    Height = ((value2 << 6) >> 24) + 100,
                    Nationality = (value2 << 14) >> 23,
                    Nationality2 = (value2 << 23) >> 23,
                    Clearing = (value3 >> 26) + 40,
                    LowPass = ((value3 << 2) >> 26) + 40,
                    LeftBack = value4 >> 30,
                    Coverage = ((value4 << 2) >> 26) + 40,
                    Jump = ((value4 << 8) >> 26) + 40,
                    Header = ((value4 << 14) >> 26) + 40,
                    BallControl = ((value4 << 20) >> 26) + 40,
                    DefenseProwess = ((value4 << 26) >> 26) + 40,
                    GK = value5 >> 30,
                    Curve = ((value5 << 2) >> 26) + 40,
                    Goalkeeping = ((value5 << 8) >> 26) + 40,
                    Reflexes = ((value5 << 14) >> 26) + 40,
                    BallWinning = ((value5 << 20) >> 26) + 40,
                    Speed = ((value5 << 26) >> 26) + 40,
                    Penalty = (value6 >> 30) + 1,
                    Catching = ((value6 << 2) >> 26) + 40,
                    KickingPower = ((value6 << 8) >> 26) + 40,
                    Dribbling = ((value6 << 14) >> 26) + 40,
                    ExplosivePower = ((value6 << 20) >> 26) + 40,
                    Stamina = ((value6 << 26) >> 26) + 40,
                    Attitude = (value7 >> 30),
                    LoftedPass = ((value7 << 2) >> 26) + 40,
                    Finishing = ((value7 << 8) >> 26) + 40,
                    PhysicalContact = ((value7 << 14) >> 26) + 40,
                    BodyControl = ((value7 << 20) >> 26) + 40,
                    AttackProwess = ((value7 << 20) >> 26) + 40,
                };

                tempList.Add(temp);
            }

            return await PlayerConverter.ConvertMany(tempList);
        }

        private byte[] GetDataFromStream(byte[] fileToLoad)
        {
            byte[] outcome = { };
            try
            {
                Inflater inflater = new Inflater(false);
                int zzsize = Convert.ToInt32(BitConverter.ToUInt32(fileToLoad, 8));
                int eesize = Convert.ToInt32(BitConverter.ToUInt32(fileToLoad, 12));
                inflater.SetInput(fileToLoad, 16, zzsize);
                byte[] outdata = new byte[eesize];
                inflater.Inflate(outdata);
                outcome = outdata;
            }
            catch
            {

            }
            return outcome;
        }
    }
}
