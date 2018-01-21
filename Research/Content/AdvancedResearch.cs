using DawnOfIndustryCore.Research.Logic;

namespace DawnOfIndustryCore.Research.Content
{
	public class AdvancedResearch : ModResearch
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The second research");
			Tooltip.SetDefault("more advanced");
		}

		public override void SetDefaults()
		{
			category = "Advanced";
		}
	}
}