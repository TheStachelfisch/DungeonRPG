using System;
using System.Collections.Generic;

namespace DungeonRPG.Data
{
    public class Character
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public long Money { get; set; }
        public long Gems { get; set; }
        public string Class { get; set; } = "Townie";
        public List<long> Inventory { get; set; } = new List<long>();
    }
}