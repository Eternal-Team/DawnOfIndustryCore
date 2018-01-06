using System.Collections.Generic;
using System.IO;
using DawnOfIndustryCore.Buffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Base.Items;

namespace DawnOfIndustryCore.Items.Cells
{
	public class UraniumFuelRod : BaseItem
	{
		public override bool CloneNewInstances => false;

		public override ModItem Clone(Item item)
		{
			UraniumFuelRod clone = (UraniumFuelRod)base.Clone(item);
			clone.amount = amount;
			return clone;
		}

		public int amount;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Uranium Fuel Rod");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 16;
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 10, 0);
		}

		public override void UpdateInventory(Player player)
		{
			if (amount > 0) player.AddBuff(mod.BuffType<Radiation>(), 72000);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string text = amount > 0 ? $"Uranium Fuel Rod ({amount}/10000)" : "Uranium Fuel Rod (Depleted)";
			tooltips.Find(x => x.mod == "Terraria" && x.Name == "ItemName").text = text;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Amount"] = amount
		};

		public override void Load(TagCompound tag)
		{
			amount = tag.GetInt("Amount");
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}