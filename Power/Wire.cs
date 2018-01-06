using System.Collections.Generic;
using DawnOfIndustryCore.Items.Wires;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using TheOneLibrary.Energy.Energy;
using TheOneLibrary.Layer.Layer;
using static TheOneLibrary.Utility.Utility.Facing;

namespace DawnOfIndustryCore.Power
{
	public enum Connection
	{
		In,
		Out,
		Both,
		Blocked
	}

	public class Wire : ILayerElement
	{
		public WireGrid grid;

		public Point16 position;
		public string Name;
		public int type;
		public short frameX;
		public short frameY;
		public long share;
		public long maxIO;

		public Connection IO = Connection.Both;

		public IDictionary<TheOneLibrary.Utility.Utility.Facing, bool> connections = new Dictionary<TheOneLibrary.Utility.Utility.Facing, bool>(4);

		public Wire()
		{
			connections.Add(Left, true);
			connections.Add(Right, true);
			connections.Add(Up, true);
			connections.Add(Down, true);
		}

		public void SetDefaults(int type)
		{
			this.type = type;
			Item item = new Item();
			item.SetDefaults(type);
			Name = item.modItem.GetType().Name;
			maxIO = ((BaseWire)item.modItem).maxIO;
		}

		public void Frame()
		{
			CustomDictionary<Wire> wires = DawnOfIndustryCore.Instance.GetModWorld<DoIWorld>().wires.elements;

			frameX = 0;
			frameY = 0;

			if (wires.ContainsKey(position.X - 1, position.Y) && wires[position.X - 1, position.Y].type == type && connections[Left]) frameX += 18;
			if (wires.ContainsKey(position.X + 1, position.Y) && wires[position.X + 1, position.Y].type == type && connections[Right]) frameX += 36;
			if (wires.ContainsKey(position.X, position.Y - 1) && wires[position.X, position.Y - 1].type == type && connections[Up]) frameY += 18;
			if (wires.ContainsKey(position.X, position.Y + 1) && wires[position.X, position.Y + 1].type == type && connections[Down]) frameY += 36;
		}

		public void Merge()
		{
			CustomDictionary<Wire> wires = DawnOfIndustryCore.Instance.GetModWorld<DoIWorld>().wires.elements;

			foreach (Point16 check in TheOneLibrary.Utility.Utility.CheckNeighbours())
			{
				Point16 point = position + check;

				Wire wire = wires.ContainsKey(point) ? wires[point] : null;

				if (wire != null)
				{
					if (check.X == -1 && connections[Left] && wire.connections[Right]) grid.MergeGrids(wire.grid);
					else if (check.X == 1 && connections[Right] && wire.connections[Left]) grid.MergeGrids(wire.grid);
					if (check.Y == -1 && connections[Up] && wire.connections[Down]) grid.MergeGrids(wire.grid);
					else if (check.Y == 1 && connections[Down] && wire.connections[Up]) grid.MergeGrids(wire.grid);
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 position)
		{
			spriteBatch.Draw(DawnOfIndustryCore.wireTexture, position, new Rectangle(frameX, frameY, 16, 16), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

			Vector2 tePosVec = (position + Main.screenPosition) / 16;
			Point16 tePos = TheOneLibrary.Utility.Utility.TileEntityTopLeft((int)tePosVec.X, (int)tePosVec.Y);
			TileEntity tileEntity = TileEntity.ByPosition.ContainsKey(tePos) ? TileEntity.ByPosition[tePos] : null;
			if (tileEntity != null && (tileEntity is IEnergyReceiver || tileEntity is IEnergyProvider))
			{
				switch (IO)
				{
					case Connection.In:
						Main.spriteBatch.Draw(DawnOfIndustryCore.inTexture, position + new Vector2(4), Color.White);
						break;
					case Connection.Out:
						Main.spriteBatch.Draw(DawnOfIndustryCore.outTexture, position + new Vector2(4), Color.White);
						break;
					case Connection.Both:
						Main.spriteBatch.Draw(DawnOfIndustryCore.bothTexture, position + new Vector2(4), Color.White);
						break;
					case Connection.Blocked:
						Main.spriteBatch.Draw(DawnOfIndustryCore.blockedTexture, position + new Vector2(4), Color.White);
						break;
				}
			}
		}

		public ElementInfo GetInfo() => new ElementInfo
		{
			Draw = true
		};
	}
}