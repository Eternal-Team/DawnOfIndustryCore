using Terraria;
using TheOneLibrary.Base.Items;
using TheOneLibrary.Utility;

namespace DawnOfIndustryCore.Items.Tools
{
	public class ResearchNotes : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Research Notes");
			Tooltip.SetDefault("Intel inside:tm:");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.height = 40;
			item.useAnimation = 15;
			item.useTime = 15;
			item.value = 1;
			item.rare = 13;
			item.useStyle = UseStyleID.HoldingOut;
		}

		public override bool UseItem(Player player)
		{
			DawnOfIndustryCore.Instance.ResearchUI.visible = !DawnOfIndustryCore.Instance.ResearchUI.visible;
			return true;
		}
	}
}