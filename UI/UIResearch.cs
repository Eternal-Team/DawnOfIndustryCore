using DawnOfIndustryCore.Research.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace DawnOfIndustryCore.UI
{
	public class UIResearch : UIViewElement
	{
		public ModResearch research;
		public string HoverText;

		public UIResearch(ModResearch research)
		{
			this.research = research;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			spriteBatch.Draw(ResearchLoader.researchTexture[research.Name], new Rectangle((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height), Color.White);

			if (IsMouseHovering && !string.IsNullOrWhiteSpace(HoverText))
			{
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
				TheOneLibrary.Utility.Utility.DrawMouseText(HoverText);
				Main.mouseText = true;
			}
		}
	}
}