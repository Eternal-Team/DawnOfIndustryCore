using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using TheOneLibrary.UI.Elements;

namespace DawnOfIndustryCore.UI
{
	public class UIViewElement : BaseElement
	{
		public Vector2 BasePosition;
		
		public UIViewElement()
		{
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);

			Main.PlaySound(SoundID.MenuTick);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);

			Main.PlaySound(SoundID.MenuTick);
		}
	}
}