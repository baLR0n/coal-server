using COAL.CORE.Core.Enums;
using COAL.CORE.Models;
using COAL.CORE.Models.Team;
using COAL.PES.Models;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COAL.PES.Data
{
    public class PESDataReader
    {
        private static string playerPath = "/Player.bin";
        private static int playerBlock = 188;

        private static string teamsPath = "/Team.bin";
        private static int teamsBlock = 1468;


        public SupportedGames Game { get; set; } = SupportedGames.PES_2019;

        /// <summary>
        /// Exports all players from the games database.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<List<Player>> ReadPlayersAsync(string path)
        {
            MemoryStream memory = await this.CreateMemoryStreamAsync(path + playerPath);

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

                // Fill all the data into the PESPlayer object.
                PESPlayer temp = new PESPlayer()
                {
                    Position = index,
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
  
        /// <summary>
        /// Updates a player in the PES Game database files.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task<List<Player>> UpdatePlayersAsync(List<Player> players, string path)
        {
            // ToDo: Later, use updateRequests and save the changes in a kind of BIO.
            // ToDo: Maybe also update team assignments here?

            List<PESPlayer> pesPlayers = await PlayerConverter.ConvertBackMany(players);

            MemoryStream memory = await this.CreateMemoryStreamAsync(path + playerPath);
            BinaryWriter writer = new BinaryWriter(memory);

            foreach (PESPlayer pesPlayer in pesPlayers)
            {
                writer.BaseStream.Position = pesPlayer.Position;
                writer.Write(pesPlayer.Id);

                // Update names
                writer.BaseStream.Position = pesPlayer.Position + 90;
                writer.Write(pesPlayer.ShirtName.ToCharArray());
                while (writer.BaseStream.Position < pesPlayer.Position + 136)
                {
                    writer.Write('\0');
                }

                writer.BaseStream.Position = pesPlayer.Position + 136;
                writer.Write(pesPlayer.Name.ToCharArray());
                while (writer.BaseStream.Position < pesPlayer.Position + 182)
                {
                    writer.Write('\0');
                }
            }

            // Create byte array from altered memory stream.
            byte[] ss13 = this.CreateWriteableBytes(memory.ToArray());
            await File.WriteAllBytesAsync(path + playerPath, ss13);

            return players;
        }

        /// <summary>
        /// Exports all teams from the PES database and creates clubs and teams.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<List<Club>> ReadClubsAsync(string path)
        {
            MemoryStream memory = await this.CreateMemoryStreamAsync(path + teamsPath);

            var reader = new BinaryReader(memory);

            int index;
            int length = Convert.ToInt32(reader.BaseStream.Length / teamsBlock);

            reader.BaseStream.Position = 0;
            string all = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(1468));

            List<PESTeam> tempList = new List<PESTeam>();

            for (int i = 0; i < length; i++)
            {
                index = (i * teamsBlock);
                reader.BaseStream.Position = index;

                // Read the 11 4-Byte uint values which contain all the player data.
                uint coach = reader.ReadUInt32();
                uint feeder = reader.ReadUInt32();
                uint idXparent = reader.ReadUInt32();
                uint temp = reader.ReadUInt32();
                uint stadium16 = reader.ReadUInt16();
                uint temp2 = reader.ReadUInt16();

                uint checks = reader.ReadUInt32();

                
                reader.BaseStream.Position = index + 24;
                string japName = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(45)).TrimEnd('\0');

                reader.BaseStream.Position = index + 94;
                string spanish  = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(45)).TrimEnd('\0');

                reader.BaseStream.Position = index + 234;
                string english = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(45)).TrimEnd('\0');

                PESTeam team = new PESTeam()
                {
                    ManagerId = coach,
                    Id = idXparent,
                    StadiumId = stadium16,
                    FeederTeamId = feeder,
                    Name = english
                };

                tempList.Add(team);
            }

            //return await TeamConverter.ConvertMany(tempList);
            return null;
        }

        /// <summary>
        /// Get a memory stream from a PES database file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<MemoryStream> CreateMemoryStreamAsync(string path)
        {
            byte[] file = await File.ReadAllBytesAsync(path);
            byte[] stream = this.GetDataFromStream(file);
            return new MemoryStream(stream);
        }

        /// <summary>
        /// Get a parsable byte array.
        /// </summary>
        /// <param name="fileToLoad"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create a writeable byte array to persist data back into the PES data system.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] CreateWriteableBytes(byte[] input)
        {
            uint fsize = (uint)input.Length;

            Deflater deflater = new Deflater(9);
            deflater.SetInput(input);
            deflater.Finish();
            using (var ms = new MemoryStream())
            {
                var outputBuffer = new byte[65536 * 32];
                while (deflater.IsNeedingInput == false)
                {
                    var read = deflater.Deflate(outputBuffer);
                    ms.Write(outputBuffer, 0, read);

                    if (deflater.IsFinished == true)
                        break;
                }

                deflater.Reset();

                uint zsize = (uint)ms.Length;
                byte[] header = { 0x04, 0x10, 0x01, 0x57, 0x45, 0x53, 0x59, 0x53 };
                byte[] b1, b2;
                b1 = BitConverter.GetBytes(zsize);
                b2 = BitConverter.GetBytes(fsize);

                byte[] zlib = ms.ToArray();

                return header.Concat(b1).Concat(b2).Concat(zlib).ToArray();
            }
        }
            
    }
}
