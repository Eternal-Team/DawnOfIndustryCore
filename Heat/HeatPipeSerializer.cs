using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace DawnOfIndustryCore.Heat
{
	public class HeatPipeSerializer : TagSerializer<HeatPipe, TagCompound>
	{
		public override TagCompound Serialize(HeatPipe value) => new TagCompound
		{
			["Type"] = value.Name,
			["Position"] = value.position,
			["frameX"] = value.frameX,
			["frameY"] = value.frameY,
			["Facing"] = value.connections.Keys.Select(x => (int)x).ToList(),
		};

		public override HeatPipe Deserialize(TagCompound tag)
		{
			HeatPipe heatPipe = new HeatPipe();
			heatPipe.Name = tag.GetString("Type");
			heatPipe.SetDefaults(DawnOfIndustryCore.Instance.ItemType(heatPipe.Name));
			heatPipe.position = tag.Get<Point16>("Position");
			heatPipe.frameX = tag.GetShort("frameX");
			heatPipe.frameY = tag.GetShort("frameY");
			heatPipe.connections = tag.GetList<int>("Facing").Select(x => (BaseLib.Utility.Utility.Facing)x).Zip(tag.GetList<bool>("Connection"), (x, y) => new KeyValuePair<BaseLib.Utility.Utility.Facing, bool>(x, y)).ToDictionary(x => x.Key, x => x.Value);
			return heatPipe;
		}
	}
}