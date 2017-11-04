using DawnOfIndustryCore.Items.Armors;
using Terraria;
using Terraria.ModLoader;

namespace DawnOfIndustryCore
{
	public static class Utility
	{
		public static bool WearsHazmat(this Player player)
		{
			Mod mod = DawnOfIndustryCore.Instance;
			return player.armor[0].type == mod.ItemType<HazmatHelmet>() && player.armor[1].type == mod.ItemType<HazmatChestplate>() && player.armor[2].type == mod.ItemType<HazmatLeggings>();
		}
	}
}