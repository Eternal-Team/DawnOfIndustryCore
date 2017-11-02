using BaseLib.Items;
using Terraria;

namespace DawnOfIndustryCore.Items
{
	public class Wrench : BaseItem, IWrench
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wrench");
			Tooltip.SetDefault("Does nothing, yet");
		}

		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 32;
			item.value = Item.sellPrice(0, 0, 50, 0);
			item.rare = 6;
			item.useStyle = 1;
			item.useTime = 5;
			item.useAnimation = 5;
			item.autoReuse = true;
			item.useTurn = true;
		}
	}
}