using TheOneLibrary.Base.Items;

namespace DawnOfIndustryCore.Items.Materials
{
	public class Thatch : BaseItem
	{
		public override string Texture => DawnOfIndustryCore.ItemTexturePath + "Thatch";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Thatch");
			Tooltip.SetDefault("Binds things together");
		}

		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 32;
			item.value = 1;
			item.rare = -1;
			item.maxStack = 999;
		}
	}
}