using TheOneLibrary.Base.Items;

namespace DawnOfIndustryCore.Items.Materials
{
	public class Rock : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rock");
			Tooltip.SetDefault("Requires some work to be useful");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.height = 40;
			item.value = 1;
			item.rare = -1;
			item.maxStack = 999;
		}
	}
}