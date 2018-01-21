using DawnOfIndustryCore.Research.Logic;

namespace DawnOfIndustryCore.Research.Content
{
	public class StartResearch : ModResearch
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The first research");
			Tooltip.SetDefault("test tooltip");
		}

		public override void SetDefaults()
		{
			category = "Start";
		}
	}
}