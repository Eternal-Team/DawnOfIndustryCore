using BaseLib.Items;

namespace DawnOfIndustryCore.Items
{
	public class Power : BaseItem
	{
		public override string Texture => DawnOfIndustryCore.ItemTexturePath + "Power";

		public override void SetDefaults()
		{
			item.width = 6;
			item.height = 18;
		}
	}
}