using TheOneLibrary.Base.Items;

namespace DawnOfIndustryCore.Items.Tools
{
	public class TheManual : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Manual");
			Tooltip.SetDefault("No one reads it");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.height = 40;
			item.value = 1;
			item.rare = 13;
		}
	}
}