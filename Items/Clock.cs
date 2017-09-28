using BaseLib.Items;

namespace DawnOfIndustryCore.Items
{
	public class Clock : BaseItem
	{
		public override string Texture => DawnOfIndustryCore.ItemTexturePath + "Clock";

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 24;
		}
	}
}