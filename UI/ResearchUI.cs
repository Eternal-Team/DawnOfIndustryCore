using DawnOfIndustryCore.Research.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utility;

namespace DawnOfIndustryCore.UI
{
	public class Line
	{
		public Vector2 A;
		public Vector2 B;
		public float Thickness;

		public Line() { }
		public Line(Vector2 a, Vector2 b, float thickness = 1)
		{
			A = a;
			B = b;
			Thickness = thickness;
		}

		public void Draw(SpriteBatch spriteBatch, Color color)
		{
			Vector2 tangent = B - A;
			float rotation = (float)Math.Atan2(tangent.Y, tangent.X);

			const float ImageThickness = 8;
			float thicknessScale = Thickness / ImageThickness;

			Vector2 capOrigin = new Vector2(DawnOfIndustryCore.lightningCap.Width, DawnOfIndustryCore.lightningCap.Height / 2f);
			Vector2 middleOrigin = new Vector2(0, DawnOfIndustryCore.lightningSegment.Height / 2f);
			Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

			spriteBatch.Draw(DawnOfIndustryCore.lightningSegment, A, null, color, rotation, middleOrigin, middleScale, SpriteEffects.None, 0f);
			spriteBatch.Draw(DawnOfIndustryCore.lightningCap, A, null, color, rotation, capOrigin, thicknessScale, SpriteEffects.None, 0f);
			spriteBatch.Draw(DawnOfIndustryCore.lightningCap, B, null, color, rotation + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
		}
	}

	public class ResearchUI : BaseUI
	{
		public UIElement baseElement = new UIElement();
		public UITexture background = new UITexture(DawnOfIndustryCore.textureBlueprint);
		public UIView view = new UIView();

		public override void OnInitialize()
		{
			baseElement.Width.Pixels = 1066;
			baseElement.Height.Pixels = 770;
			baseElement.Center();
			baseElement.SetPadding(0);
			Append(baseElement);

			background.Width.Pixels = 1026;
			background.Height.Pixels = 770;
			background.Left.Pixels = 40;
			background.SetPadding(0);
			baseElement.Append(background);

			view.Width.Set(-16, 1);
			view.Height.Set(-16, 1);
			view.Center();
			view.SetPadding(0);
			view.PreDrawChildren += DrawLines;
			background.Append(view);
		}

		public void PostInit()
		{
			AddCategories();
		}

		protected static List<Line> CreateBolt(Vector2 source, Vector2 dest, float thickness)
		{
			var results = new List<Line>();
			Vector2 tangent = dest - source;
			Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
			float length = tangent.Length();

			List<float> positions = new List<float>();
			positions.Add(0);

			for (int i = 0; i < length / 2; i++)
				positions.Add(Main.rand.NextFloat(0, 1));

			positions.Sort();

			const float Sway = 15;
			const float Jaggedness = 1 / Sway;

			Vector2 prevPoint = source;
			float prevDisplacement = 0;
			for (int i = 1; i < positions.Count; i++)
			{
				float pos = positions[i];

				// used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
				float scale = length * Jaggedness * (pos - positions[i - 1]);

				// defines an envelope. Points near the middle of the bolt can be further from the central line.
				float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

				float displacement = Main.rand.NextFloat(-Sway, Sway);
				displacement -= (displacement - prevDisplacement) * (1 - scale);
				displacement *= envelope;

				Vector2 point = source + pos * tangent + displacement * normal;
				results.Add(new Line(prevPoint, point, thickness));
				prevPoint = point;
				prevDisplacement = displacement;
			}

			results.Add(new Line(prevPoint, dest, thickness));

			return results;
		}

		public void AddCategories()
		{
			for (int i = 0; i < Research.Logic.Research.researchCategories.Count; i++)
			{
				ResearchCategory category = Research.Logic.Research.researchCategories.ElementAt(i).Value;
				UIButton button = new UIButton(ModLoader.GetTexture(category.icon));
				button.HoverText = category.GetName();
				button.Width.Pixels = 40;
				button.Height.Pixels = 40;
				button.Top.Pixels = 48 * i;
				button.OnClick += (a, b) =>
				{
					currentCategory = category;
					PopulateView();
				};
				baseElement.Append(button);
			}

			currentCategory = Research.Logic.Research.researchCategories.FirstOrDefault().Value;
			PopulateView();
		}

		private ResearchCategory currentCategory;
		public void PopulateView()
		{
			view.Clear();

			if (currentCategory != null)
			{
				foreach (KeyValuePair<string, ModResearch> item in currentCategory.research)
				{
					UIResearch research = new UIResearch(item.Value);
					research.Width.Pixels = 40;
					research.Height.Pixels = 40;
					research.BasePosition = item.Value.position;
					research.HoverText = item.Value.DisplayName.GetTranslation(Language.ActiveCulture) + "\n" + item.Value.Tooltip.GetTranslation(Language.ActiveCulture);
					view.Add(research);
				}
			}

			view.RecalculateChildren();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			DrawSelf(spriteBatch);

			DrawChildren(spriteBatch);
		}
		
		public void DrawLines(SpriteBatch spriteBatch)
		{
			RasterizerState state = new RasterizerState { ScissorTestEnable = true };
			spriteBatch.End();

			spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, state, null, Main.UIScaleMatrix);

			foreach (UIResearch element in view.items.Select(x => (UIResearch)x))
			{
				if (element.research.Parents.Any())
				{
					foreach (string parent in element.research.Parents)
					{
						UIResearch research = view.items.Select(x => (UIResearch)x).FirstOrDefault(x => x.research.Name == parent);

						if (research != null)
						{
							foreach (Line line in CreateBolt(element.GetDimensions().Center(), research.GetDimensions().Center(), 2f))
							{
								line.Draw(spriteBatch, Color.LightBlue);
							}
						}
					}
				}
			}

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, state, null, Main.UIScaleMatrix);
		}

		public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color colorStart, Color colorEnd, float width)
		{
			float num = Vector2.Distance(start, end);
			Vector2 vector = (end - start) / num;
			Vector2 value = start;
			float rotation = vector.ToRotation();
			float scale = width / 16f;
			for (float num2 = 0f; num2 <= num; num2 += width)
			{
				float amount = num2 / num;
				spriteBatch.Draw(Main.blackTileTexture, value, null, Color.Lerp(colorStart, colorEnd, amount), rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
				value = start + num2 * vector;
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (baseElement.ContainsPoint(Main.MouseScreen)) Main.LocalPlayer.mouseInterface = true;
		}
	}
}