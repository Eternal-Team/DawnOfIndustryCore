using BaseLib.Items;
using Terraria;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Items.Armors
{
	[AutoloadEquip(EquipType.Head)]
	public class HazmatHelmet : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hazmat Suit Helmet");
			Tooltip.SetDefault("Protects you from radiation\nMust wear the full set");
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.value = Item.sellPrice(0, 0, 0, 50);
			item.rare = 3;
			item.defense = 1;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == mod.ItemType<HazmatChestplate>() && legs.type == mod.ItemType<HazmatLeggings>();

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "Protects you from radiation";
			player.statDefense += 1;
		}
	}

	[AutoloadEquip(EquipType.Body)]
	public class HazmatChestplate : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hazmat Suit Chestplate");
			Tooltip.SetDefault("Protects you from radiation\nMust wear the full set");
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.value = Item.sellPrice(0, 0, 0, 50);
			item.rare = 3;
			item.defense = 2;
		}
	}

	[AutoloadEquip(EquipType.Legs)]
	public class HazmatLeggings : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hazmat Suit Leggings");
			Tooltip.SetDefault("Protects you from radiation\nMust wear the full set");
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.value = Item.sellPrice(0, 0, 0, 50);
			item.rare = 3;
			item.defense = 1;
		}
	}
}