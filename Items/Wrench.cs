using BaseLib.Items;
using DawnOfIndustryCore.Items.Wires;
using DawnOfIndustryCore.Wiring;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Items
{
	public class Wrench : BaseItem, IWrench
	{
		public override bool CloneNewInstances => false;

		public override ModItem Clone(Item item)
		{
			Wrench clone = (Wrench)base.Clone(item);
			clone.mode = mode;
			return clone;
		}

		public enum Mode
		{
			Place,
			Cut,
			Change,
			Info
		}

		public Mode mode;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wrench");
		}

		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 32;
			item.value = Item.sellPrice(0, 0, 50, 0);
			item.rare = 6;
			item.useStyle = 1;
			item.useTime = 5;
			item.useAnimation = 5;
			item.autoReuse = true;
			item.useTurn = true;
		}

		public void Change(Point16 mouse)
		{
			CustomDictionary<Wire> wires = mod.GetModWorld<DoIWorld>().wires.elements;

			if (wires.ContainsKey(mouse))
			{
				Wire wire = wires[mouse];

				int x = (int)Main.MouseWorld.X - mouse.X * 16;
				int y = (int)Main.MouseWorld.Y - mouse.Y * 16;

				Rectangle io = new Rectangle(4, 4, 8, 8);
				if (!io.Contains(x, y))
				{
					if (BaseLib.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 0), new Point(8, 8), new Point(0, 16)))
					{
						wire.connections[Facing.Left] = !wire.connections[Facing.Left];
						wire.Frame();

						Main.NewText(wire.grid.tiles.Count.ToString());
						if (!wire.connections[Facing.Left]) wire.grid.ReformGrid();

						if (wires.ContainsKey(mouse.X - 1, mouse.Y))
						{
							wires[mouse.X - 1, mouse.Y].connections[Facing.Right] = !wires[mouse.X - 1, mouse.Y].connections[Facing.Right];
							if (wire.connections[Facing.Left])
							{
								Main.NewText(wire.GetGrid().tiles.Count + " " + wires[mouse.X - 1, mouse.Y].GetGrid().tiles.Count);
								wire.GetGrid().MergeGrids(wires[mouse.X - 1, mouse.Y].GetGrid());
							}
							wires[mouse.X - 1, mouse.Y].Frame();
						}
					}
					else if (BaseLib.Utility.Utility.PointInTriangle(new Point(x, y), new Point(16, 0), new Point(16, 16), new Point(8, 8)))
					{
						wire.connections[Facing.Right] = !wire.connections[Facing.Right];
						wire.Frame();

						if (!wire.connections[Facing.Right]) wire.grid.ReformGrid();

						if (wires.ContainsKey(mouse.X + 1, mouse.Y))
						{
							wires[mouse.X + 1, mouse.Y].connections[Facing.Left] = !wires[mouse.X + 1, mouse.Y].connections[Facing.Left];
							if (wire.connections[Facing.Right]) wire.GetGrid().MergeGrids(wires[mouse.X + 1, mouse.Y].GetGrid());
							wires[mouse.X + 1, mouse.Y].Frame();
						}
					}
					else if (BaseLib.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 0), new Point(16, 0), new Point(8, 8)))
					{
						wire.connections[Facing.Up] = !wire.connections[Facing.Up];
						wire.Frame();

						if (!wire.connections[Facing.Up]) wire.grid.ReformGrid();

						if (wires.ContainsKey(mouse.X, mouse.Y - 1))
						{
							wires[mouse.X, mouse.Y - 1].connections[Facing.Down] = !wires[mouse.X, mouse.Y - 1].connections[Facing.Down];
							if (wire.connections[Facing.Up]) wire.GetGrid().MergeGrids(wires[mouse.X, mouse.Y - 1].GetGrid());
							wires[mouse.X, mouse.Y - 1].Frame();
						}
					}
					else if (BaseLib.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 16), new Point(8, 8), new Point(16, 16)))
					{
						wire.connections[Facing.Down] = !wire.connections[Facing.Down];
						wire.Frame();

						if (!wire.connections[Facing.Down]) wire.grid.ReformGrid();

						if (wires.ContainsKey(mouse.X, mouse.Y + 1))
						{
							wires[mouse.X, mouse.Y + 1].connections[Facing.Up] = !wires[mouse.X, mouse.Y + 1].connections[Facing.Up];
							if (wire.connections[Facing.Down]) wire.GetGrid().MergeGrids(wires[mouse.X, mouse.Y + 1].GetGrid());
							wires[mouse.X, mouse.Y + 1].Frame();
						}
					}
				}
				else wire.IO = wire.IO.NextEnum();
			}
		}

		public void Cut(Point16 mouse, Player player)
		{
			CustomDictionary<Wire> wires = mod.GetModWorld<DoIWorld>().wires.elements;

			if (wires.ContainsKey(mouse))
			{
				Wire wire = wires[mouse];
				wire.GetGrid().tiles.Remove(wire);

				wire.GetGrid().ReformGrid();

				wires.Remove(mouse);

				foreach (Point16 check in BaseLib.Utility.Utility.CheckNeighbours())
				{
					Point16 checkPoint = new Point16(mouse.X + check.X, mouse.Y + check.Y);
					if (wires.ContainsKey(checkPoint) && wires[checkPoint].type == wire.type) wires[checkPoint].Frame();
				}

				player.PutItemInInventory(wire.type);
			}
		}

		public void Place(Point16 mouse, Player player)
		{
			CustomDictionary<Wire> wires = mod.GetModWorld<DoIWorld>().wires.elements;

			if (!wires.ContainsKey(mouse) && player.inventory.Any(x => x.modItem is BaseWire))
			{
				Wire wire = new Wire();
				wire.SetDefaults(player.inventory.FirstOrDefault(x => x.modItem is BaseWire).type);
				wire.position = mouse;
				wires.Add(mouse, wire);

				MultiTileGrid grid = new MultiTileGrid();
				grid.energy.SetMaxTransfer(wire.maxIO);
				grid.energy.SetCapacity(wire.maxIO * 2);
				grid.tiles.Add(wire);
				wire.SetGrid(grid);

				player.ConsumeItem(mod.ItemType<BasicWire>());

				wire.Merge();
				wire.Frame();

				foreach (Point16 add in BaseLib.Utility.Utility.CheckNeighbours()) if (wires.ContainsKey(mouse.X + add.X, mouse.Y + add.Y) && wires[mouse.X + add.X, mouse.Y + add.Y].type == mod.ItemType<BasicWire>() && wires[mouse.X + add.X, mouse.Y + add.Y].connections[Facing.Right]) wires[mouse.X + add.X, mouse.Y + add.Y].Frame();
			}
		}

		public bool Info(Point16 mouse)
		{
			CustomDictionary<Wire> wires = mod.GetModWorld<DoIWorld>().wires.elements;

			if (wires.ContainsKey(mouse))
			{
				Wire wire = wires[mouse];

				Main.NewText("Tiles: " + wire.GetGrid().tiles.Count);
				Main.NewText("Current capacity: " + wire.GetGrid().energy.GetCapacity());
				Main.NewText("Current IO: " + wire.GetGrid().energy.GetMaxReceive() + "/" + wire.GetGrid().energy.GetMaxExtract());
				Main.NewText("Current energy: " + wire.GetGrid().energy.GetEnergyStored());

				return true;
			}

			return false;
		}

		public override bool UseItem(Player player)
		{
			Point16 mouse = BaseLib.Utility.Utility.MouseToWorldPoint();

			switch (mode)
			{
				case Mode.Change:
					Change(mouse);
					break;
				case Mode.Cut:
					Cut(mouse, player);
					break;
				case Mode.Place:
					Place(mouse, player);
					break;
				case Mode.Info:
					Info(mouse);
					break;
			}

			return true;
		}
	}
}