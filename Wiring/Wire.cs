using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace DawnOfIndustryCore.Wiring
{
	public enum Facing
	{
		Left,
		Right,
		Up,
		Down
	}

	public enum Connection
	{
		In,
		Out,
		Both,
		Blocked
	}

	public class Wire : IMultiTile
	{
		public MultiTileGrid grid;

		public Point16 position;
		public string Name;
		public int type;
		public short frameX;
		public short frameY;
		public long share;
		public long maxIO;

		public Connection IO = Connection.Both;

		public IDictionary<Facing, bool> connections = new Dictionary<Facing, bool>(4);

		public Wire()
		{
			connections.Add(Facing.Left, true);
			connections.Add(Facing.Right, true);
			connections.Add(Facing.Up, true);
			connections.Add(Facing.Down, true);
		}

		public void SetDefaults(int type)
		{
			this.type = type;
			Item item = new Item();
			item.SetDefaults(type);
			Name = item.modItem.GetType().Name;
			maxIO = 1000;
		}

		public MultiTileGrid GetGrid() => grid;

		public void SetGrid(MultiTileGrid grid) => this.grid = grid;

		public long GetCapacity() => maxIO * 2;

		public void Frame()
		{
			CustomDictionary<Wire> wires = DawnOfIndustryCore.Instance.GetModWorld<DoIWorld>().wires.elements;

			frameX = 0;
			frameY = 0;

			if (wires.ContainsKey(position.X - 1, position.Y) && wires[position.X - 1, position.Y].type == type && connections[Facing.Left]) frameX += 18;
			if (wires.ContainsKey(position.X + 1, position.Y) && wires[position.X + 1, position.Y].type == type && connections[Facing.Right]) frameX += 36;
			if (wires.ContainsKey(position.X, position.Y - 1) && wires[position.X, position.Y - 1].type == type && connections[Facing.Up]) frameY += 18;
			if (wires.ContainsKey(position.X, position.Y + 1) && wires[position.X, position.Y + 1].type == type && connections[Facing.Down]) frameY += 36;
		}

		public void Merge()
		{
			CustomDictionary<Wire> wires = DawnOfIndustryCore.Instance.GetModWorld<DoIWorld>().wires.elements;

			foreach (Point16 check in BaseLib.Utility.Utility.CheckNeighbours())
			{
				Point16 point = new Point16(position.X + check.X, position.Y + check.Y);

				Wire wire = wires.ContainsKey(point) ? wires[point] : null;

				if (wire != null)
				{
					if (check.X == -1 && connections[Facing.Left] && wire.connections[Facing.Right]) GetGrid().MergeGrids(wire.GetGrid());
					else if (check.X == 1 && connections[Facing.Right] && wire.connections[Facing.Left]) GetGrid().MergeGrids(wire.GetGrid());
					if (check.Y == -1 && connections[Facing.Up] && wire.connections[Facing.Down]) GetGrid().MergeGrids(wire.GetGrid());
					else if (check.Y == 1 && connections[Facing.Down] && wire.connections[Facing.Up]) GetGrid().MergeGrids(wire.GetGrid());
				}
			}
		}
	}
}