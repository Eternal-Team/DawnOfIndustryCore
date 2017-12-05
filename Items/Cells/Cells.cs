using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Base.Items;

namespace DawnOfIndustryCore.Items.Cells
{
	public class Cells : BaseItem
	{
		public enum FluidType
		{
			None,
			CrudeOil,
			Methane,
			Gasoline,
			Deuterium,
			Tritium
		}

		public override bool CloneNewInstances => false;

		public override ModItem Clone(Item item)
		{
			Cells clone = (Cells)base.Clone(item);
			clone.amount = amount;
			clone.fluidType = fluidType;
			return clone;
		}

		public int amount;

		public FluidType fluidType = FluidType.None;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cell");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 16;
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 10, 0);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string text = "Cell";
			switch (fluidType)
			{
				case FluidType.None: text = "Empty Cell"; break;
				case FluidType.CrudeOil: text = "Crude Oil Cell"; break;
				case FluidType.Methane: text = "Methane Cell"; break;
				case FluidType.Gasoline: text = "Gasoline Cell"; break;
				case FluidType.Deuterium: text = "Deuterium Cell"; break;
				case FluidType.Tritium: text = "Tritium Cell"; break;
			}

			tooltips.Find(x => x.mod == "Terraria" && x.Name == "ItemName").text = text;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Amount"] = amount,
			["Type"] = (int)fluidType
		};

		public override void Load(TagCompound tag)
		{
			amount = tag.GetInt("Amount");
			fluidType = (FluidType)tag.GetInt("Type");
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}