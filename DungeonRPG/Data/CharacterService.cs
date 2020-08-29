using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DungeonRPG.Data
{
    public class CharacterService
    {
        public static string GetPlayerJson() => File.ReadAllText("Players.json");

        public static async Task WritePlayerJson(string json)
        {
            try
            {
                await File.WriteAllTextAsync("Players.json", json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }
        
        public static async Task<bool> CreateCharacterAsync(string name, ulong id, Classes classCt)
        {
            var players = JsonConvert.DeserializeObject<List<Character>>(GetPlayerJson());
            
            var player = new Character() {Name = name, Id = id, Money = 100, Gems = 0, Class = classCt.ToString()};

            players.Add(player);
            
            var serialized = JsonConvert.SerializeObject(players, Formatting.Indented);
            
            await WritePlayerJson(serialized);
            return true;
        }

        public static Task<bool> GetifExists(ulong id)
        {
            var objects = JsonConvert.DeserializeObject<List<Character>>(GetPlayerJson());
            return Task.FromResult(objects.Any(y => y.Id.Equals(id)));
        }
    }
}