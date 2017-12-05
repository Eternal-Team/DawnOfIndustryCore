using Terraria;
using TheOneLibrary.Base.Items;

namespace DawnOfIndustryCore.Items.Wires
{
	public abstract class BaseWire : BaseItem
	{
		public int maxIO;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Base Wire");
			Tooltip.SetDefault("You are not supposed to have this");
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