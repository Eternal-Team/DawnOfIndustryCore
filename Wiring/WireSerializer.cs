using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace DawnOfIndustryCore.Wiring
{
	public class WireSerializer : TagSerializer<Wire, TagCompound>
	{
		public override TagCompound Serialize(Wire value) => new TagCompound
		{
			["Type"] = value.Name,
			["Position"] = value.position,
			["frameX"] = value.frameX,
			["frameY"] = value.frameY,
			["IO"] = (int)value.IO,
			["Facing"] = value.connections.Keys.Select(x => (int)x).ToList(),
			["Connection"] = value.connections.Values.ToList(),
			["Energy"] = value.GetGrid().GetEnergySharePerNode()
		};

		public override Wire Deserialize(TagCompound tag)
		{
			Wire wire = new Wire();
			wire.Name = tag.GetString("Type");
			wire.SetDefaults(DawnOfIndustryCore.Instance.ItemType(wire.Name));
			wire.position = tag.Get<Point16>("Position");
			wire.frameX = tag.GetShort("frameX");
			wire.frameY = tag.GetShort("frameY");
			wire.IO = (Connection)tag.GetInt("IO");
			wire.connections = tag.GetList<int>("Facing").Select(x => (Facing)x).Zip(tag.GetList<bool>("Connection"), (x, y) => new KeyValuePair<Facing, bool>(x, y)).ToDictionary(x => x.Key, x => x.Value);
			wire.share = tag.GetLong("Energy");
			return wire;
		}
	}
}