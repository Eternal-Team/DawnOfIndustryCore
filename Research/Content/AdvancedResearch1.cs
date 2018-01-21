using DawnOfIndustryCore.Research.Logic;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DawnOfIndustryCore.Research.Content
{
	public class AdvancedResearch1 : ModResearch
	{
		public override List<string> Parents => new List<string> { nameof(AdvancedResearch) };

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The link test");
			Tooltip.SetDefault("more advanced");
		}

		public override void SetDefaults()
		{
			category = "Advanced";
			position = new Vector2(300, -150);
		}
	}
}