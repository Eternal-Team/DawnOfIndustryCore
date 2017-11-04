using DawnOfIndustryCore.Items.Wires;
using DawnOfIndustryCore.Wiring;
using EnergyLib.Energy;
using LayerLib.Layer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace DawnOfIndustryCore.Power
{
	public enum Connection
	{
		In,
		Out,
		Both,
		Blocked
	}

	public class Wire : IMultiTile, ILayerElement
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

		public IDictionary<BaseLib.Utility.Utility.Facing, bool> connections = new Dictionary<BaseLib.Utility.Utility.Facing, bool>(4);

		public Wire()
		{
			connections.Add(BaseLib.Utility.Utility.Facing.Left, true);
			connections.Add(BaseLib.Utility.Utility.Facing.Right, true);
			connections.Add(BaseLib.Utility.Utility.Facing.Up, true);
			connections.Add(BaseLib.Utility.Utility.Facing.Down, true);
		}

		public void SetDefaults(int type)
		{
			this.type = type;
			Item item = new Item();
			item.SetDefaults(type);
			Name = item.modItem.GetType().Name;
			maxIO = ((BaseWire)item.modItem).maxIO;
		}

		public WireGrid GetGrid() => grid;

		public void SetGrid(WireGrid grid) => this.grid = grid;

		public long GetCapacity() => maxIO * 2;

		public void Frame()
		{
			CustomDictionary<Wire> wires = DawnOfIndustryCore.Instance.GetModWorld<DoIWorld>().wires.elements;

			frameX = 0;
			frameY = 0;

			if (wires.ContainsKey(position.X - 1, position.Y) && wires[position.X - 1, position.Y].type == type && connections[BaseLib.Utility.Utility.Facing.Left]) frameX += 18;
			if (wires.ContainsKey(position.X + 1, position.Y) && wires[position.X + 1, position.Y].type == type && connections[BaseLib.Utility.Utility.Facing.Right]) frameX += 36;
			if (wires.ContainsKey(position.X, position.Y - 1) && wires[position.X, position.Y - 1].type == type && connections[BaseLib.Utility.Utility.Facing.Up]) frameY += 18;
			if (wires.ContainsKey(position.X, position.Y + 1) && wires[position.X, position.Y + 1].type == type && connections[BaseLib.Utility.Utility.Facing.Down]) frameY += 36;
		}

		public void Merge()
		{
			CustomDictionary<Wire> wires = DawnOfIndustryCore.Instance.GetModWorld<DoIWorld>().wires.elements;

			foreach (Point16 check in BaseLib.Utility.Utility.CheckNeighbours())
			{
				Point16 point = position + check;

				Wire wire = wires.ContainsKey(point) ? wires[point] : null;

				if (wire != null)
				{
					if (check.X == -1 && connections[BaseLib.Utility.Utility.Facing.Left] && wire.connections[BaseLib.Utility.Utility.Facing.Right]) GetGrid().MergeGrids(wire.GetGrid());
					else if (check.X == 1 && connections[BaseLib.Utility.Utility.Facing.Right] && wire.connections[BaseLib.Utility.Utility.Facing.Left]) GetGrid().MergeGrids(wire.GetGrid());
					if (check.Y == -1 && connections[BaseLib.Utility.Utility.Facing.Up] && wire.connections[BaseLib.Utility.Utility.Facing.Down]) GetGrid().MergeGrids(wire.GetGrid());
					else if (check.Y == 1 && connections[BaseLib.Utility.Utility.Facing.Down] && wire.connections[BaseLib.Utility.Utility.Facing.Up]) GetGrid().MergeGrids(wire.GetGrid());
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 position)
		{
			spriteBatch.Draw(DawnOfIndustryCore.wireTexture, position, new Rectangle(frameX, frameY, 16, 16), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

			Vector2 tePosVec = (position + Main.screenPosition) / 16;
			Point16 tePos = BaseLib.Utility.Utility.TileEntityTopLeft((int)tePosVec.X, (int)tePosVec.Y);
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