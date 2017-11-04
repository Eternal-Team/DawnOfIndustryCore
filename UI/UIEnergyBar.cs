using BaseLib.Elements;
using DawnOfIndustryCore.Heat.HeatStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace DawnOfIndustryCore.UI
{
	public class UIHeatBar : BaseElement
	{
		public HeatStorage heat;
		public long oldHeat;

		private Color bgColor = new Color(73, 94, 171) * 0.9f;
		private Color barColor = Color.Orange;

		private Texture2D corner = DawnOfIndustryCore.Instance.GetTexture("UI/BarCorner");
		private Texture2D side = DawnOfIndustryCore.Instance.GetTexture("UI/BarSide");

		protected void DrawBar(SpriteBatch spriteBatch, Color color)
		{
			CalculatedStyle dimensions = GetDimensions();

			spriteBatch.Draw(corner, dimensions.Position(), color);
			spriteBatch.Draw(corner, dimensions.Position() + new Vector2(dimensions.Width - 12, 12), null, color, MathHelper.Pi * 0.5f, new Vector2(12, 12), Vector2.One, SpriteEffects.None, 0f);
			spriteBatch.Draw(corner, dimensions.Position() + new Vector2(12, dimensions.Height - 12), null, color, MathHelper.Pi * 1.5f, new Vector2(12, 12), Vector2.One, SpriteEffects.None, 0f);
			spriteBatch.Draw(corner, dimensions.Position() + new Vector2(dimensions.Width - 12, dimensions.Height - 12), null, color, MathHelper.Pi * 1f, new Vector2(12, 12), Vector2.One, SpriteEffects.None, 0f);

			spriteBatch.Draw(side, new Rectangle((int)(dimensions.X + 12), (int)dimensions.Y, (int)(dimensions.Width - 24), 12), color);
			spriteBatch.Draw(side, new Rectangle((int)dimensions.X, (int)(dimensions.Y + dimensions.Height - 12), (int)(dimensions.Height - 24), 12), null, color, MathHelper.Pi * 1.5f, Vector2.Zero, SpriteEffects.None, 0f);
			spriteBatch.Draw(side, new Rectangle((int)(dimensions.X + dimensions.Width), (int)(dimensions.Y + 12), (int)(dimensions.Height - 24), 12), null, color, MathHelper.Pi * 0.5f, Vector2.Zero, SpriteEffects.None, 0f);
			spriteBatch.Draw(side, new Rectangle((int)(dimensions.X + dimensions.Width - 12), (int)(dimensions.Y + dimensions.Height), (int)(dimensions.Width - 24), 12), null, color, MathHelper.Pi, Vector2.Zero, SpriteEffects.None, 0f);

			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(dimensions.X + 12), (int)(dimensions.Y + 12), (int)(dimensions.Width - 24), (int)(dimensions.Height - 24)), color);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();

			long delta = heat.GetHeat() - oldHeat;
			oldHeat = heat.GetHeat();
			float progress = heat.GetHeat() / (float)heat.GetCapacity();

			DrawBar(spriteBatch, bgColor);

			spriteBatch.End();

			RasterizerState state = new RasterizerState { ScissorTestEnable = true };

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, state);

			Rectangle prevRect = spriteBatch.GraphicsDevice.ScissorRectangle;
			spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle((int)dimensions.X, (int)(dimensions.Y + dimensions.Height - dimensions.Height * progress), (int)dimensions.Width, (int)(dimensions.Height * progress));

			DrawBar(spriteBatch, barColor);

			spriteBatch.GraphicsDevice.ScissorRectangle = prevRect;
			spriteBatch.End();
			spriteBatch.Begin();

			if (IsMouseHovering)
			{
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
				Main.instance.MouseTextHackZoom($"{heat}\n{(delta > 0 ? "+" : "") + delta}HU/s");
				Main.mouseText = true;
			}
		}
	}
}