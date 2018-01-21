using System.Collections.Generic;
using DawnOfIndustryCore.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using TheOneLibrary.Layer.Layer;

namespace DawnOfIndustryCore.Heat
{
	public class HeatPipe : ILayerElement
	{
		public HeatPipeGrid grid;

		public Point16 position;
		public string Name;
		public int type;
		public short frameX;
		public short frameY;

		public Dictionary<TheOneLibrary.Utility.Utility.Facing, bool> connections = new Dictionary<TheOneLibrary.Utility.Utility.Facing, bool>(4);

		public HeatPipe()
		{
			connections.Add(TheOneLibrary.Utility.Utility.Facing.Left, true);
			connections.Add(TheOneLibrary.Utility.Utility.Facing.Right, true);
			connections.Add(TheOneLibrary.Utility.Utility.Facing.Up, true);
			connections.Add(TheOneLibrary.Utility.Utility.Facing.Down, true);
		}

		public void SetDefaults(int type)
		{
			this.type = type;
			Item item = new Item();
			item.SetDefaults(type);
			Name = item.modItem.GetType().Name;
		}

		public void Frame()
		{
			CustomDictionary<HeatPipe> heatPipes = DawnOfIndustryCore.Instance.GetModWorld<DoIWorld>().heatPipes.elements;

			frameX = 0;
			frameY = 0;

			if (heatPipes.ContainsKey(position.X - 1, position.Y) && heatPipes[position.X - 1, position.Y].type == type) frameX += 18;
			if (heatPipes.ContainsKey(position.X + 1, position.Y) && heatPipes[position.X + 1, position.Y].type == type) frameX += 36;
			if (heatPipes.ContainsKey(position.X, position.Y - 1) && heatPipes[position.X, position.Y - 1].type == type) frameY += 18;
			if (heatPipes.ContainsKey(position.X, position.Y + 1) && heatPipes[position.X, position.Y + 1].type == type) frameY += 36;
		}

		public void Merge()
		{
			CustomDictionary<HeatPipe> heatPipes = DawnOfIndustryCore.Instance.GetModWorld<DoIWorld>().heatPipes.elements;

			foreach (Point16 check in TheOneLibrary.Utility.Utility.CheckNeighbours())
			{
				Point16 point = new Point16(position.X + check.X, position.Y + check.Y);

				HeatPipe wire = heatPipes.ContainsKey(point) ? heatPipes[point] : null;

				if (wire != null)
				{
					if (check.X == -1) grid.MergeGrids(wire.grid);
					else if (check.X == 1) grid.MergeGrids(wire.grid);
					if (check.Y == -1) grid.MergeGrids(wire.grid);
					else if (check.Y == 1) grid.MergeGrids(wire.grid);
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 position)
		{
			spriteBatch.Draw(DawnOfIndustryCore.heatPipeTexture, position, new Rectangle(frameX, frameY, 16, 16), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
		}

		public ElementInfo GetInfo() => new ElementInfo
		{
			Draw = true
		};
	}
}