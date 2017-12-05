using Terraria;
using TheOneLibrary.Base.Items;

namespace DawnOfIndustryCore.Items.Wires
{
	public class HeatPipe : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heat Pipe");
			Tooltip.SetDefault("Transfers heat");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.rare = 0;
			item.value = Item.sellPrice(0, 0, 0, 10);
		}
	}
}