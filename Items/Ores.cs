using BaseLib.Items;
using DawnOfIndustryCore.Buffs;
using DawnOfIndustryCore.Items.Armors;
using Terraria;

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
			Tooltip.SetDefault("Chemical formula: U₃O₈");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 18;
			item.maxStack = 999;
			item.value = Item.sellPrice(0, 0, 3, 0);
		}

		public override void UpdateInventory(Player player)
		{
			if (player.armor[0].type != mod.ItemType<HazmatHelmet>() && player.armor[1].type != mod.ItemType<HazmatChestplate>() && player.armor[2].type != mod.ItemType<HazmatLeggings>()) player.AddBuff(mod.BuffType<Radiation>(), 36000);
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
			item.value = Item.sellPrice(0, 0, 70, 0);
		}
	}
}