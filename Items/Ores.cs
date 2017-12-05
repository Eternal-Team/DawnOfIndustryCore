using DawnOfIndustryCore.Buffs;
using Terraria;
using TheOneLibrary.Base.Items;
using TheOneLibrary.Utility;

namespace DawnOfIndustryCore.Items
{
	// Hematite (Iron), Cobaltite (Cobalt), Cassiterite (Tin), Bauxite (Aluminium), Chalcopyrite (Copper), Galena (Lead), Uraninite (Uranium), Wolframite (Tungsten),
	// Brine/Water (Lithium) 

	public class Cobaltite : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cobaltite");
			Tooltip.SetDefault("Chemical formula: CoAsS");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 7, 0);
		}
	}

	public class Yellowcake : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Yellowcake");
			Tooltip.SetDefault($"Chemical formula: U{3.Subscript()}O{8.Subscript()}");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 18;
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 3);
		}

		public override void UpdateInventory(Player player)
		{
			if (!player.WearsHazmat()) player.AddBuff(mod.BuffType<Radiation>(), 36000);
		}
	}

	public class Thorium : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thorium");
			Tooltip.SetDefault("Chemical formula: Th");
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 70);
		}
	}
}