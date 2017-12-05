using DawnOfIndustryCore.Items.Wires;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using TheOneLibrary.Energy.Energy;
using TheOneLibrary.Layer;
using TheOneLibrary.Layer.Items;
using TheOneLibrary.Layer.Layer;
using TheOneLibrary.Utility;
using static TheOneLibrary.Utility.Utility.Facing;

namespace DawnOfIndustryCore.Power
{
	public class WireLayer : ILayer
	{
		public CustomDictionary<Wire> elements = new CustomDictionary<Wire>();

		public TagCompound Save() => new TagCompound
		{
			["Keys"] = elements.Keys.ToList(),
			["Values"] = elements.Values.ToList()
		};

		public void Load(TagCompound tag)
		{
			elements.internalDict = tag.GetList<Point16>("Keys")
				.Zip(tag.GetList<Wire>("Values"), (x, y) => new KeyValuePair<Point16, Wire>(x, y))
				.ToDictionary(x => x.Key, x => x.Value);
		}

		public void Draw()
		{
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen) zero = Vector2.Zero;

			int num4 = (int)((Main.screenPosition.X - zero.X) / 16f - 1f);
			int num5 = (int)((Main.screenPosition.X + Main.screenWidth + zero.X) / 16f) + 2;
			int num6 = (int)((Main.screenPosition.Y - zero.Y) / 16f - 1f);
			int num7 = (int)((Main.screenPosition.Y + Main.screenHeight + zero.Y) / 16f) + 5;
			if (num4 < 4) num4 = 4;
			if (num5 > Main.maxTilesX - 4) num5 = Main.maxTilesX - 4;
			if (num6 < 4) num6 = 4;
			if (num7 > Main.maxTilesY - 4) num7 = Main.maxTilesY - 4;

			for (int i = num4 - 2; i < num5 + 2; i++)
			{
				for (int j = num6; j < num7 + 4; j++)
				{
					if (elements.ContainsKey(i, j))
					{
						Vector2 position = -Main.screenPosition + new Vector2(i, j) * 16;

						Wire wire = elements[i, j];
						if (GetInfo().Draw) wire.Draw(Main.spriteBatch, position);
					}
				}
			}
		}

		public void DrawPreview()
		{
			Point16 mouse = TheOneLibrary.Utility.Utility.MouseToWorldPoint();

			// Draws wire preview
			if (Main.LocalPlayer.inventory.Any(x => x.modItem is BaseWire) && !elements.ContainsKey(mouse))
				Main.spriteBatch.Draw(DawnOfIndustryCore.wireTexture, mouse.ToVector2() * 16 - Main.screenPosition, new Rectangle(0, 0, 16, 16), Color.White * 0.5f, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
		}

		public void Update()
		{
			foreach (Wire wire in elements.Values)
			{
				Point16 check = TheOneLibrary.Utility.Utility.TileEntityTopLeft(wire.position.X, wire.position.Y);

				if (TileEntity.ByPosition.ContainsKey(check))
				{
					WireGrid grid = wire.grid;
					TileEntity te = TileEntity.ByPosition[check];

					if (wire.IO == Connection.In || wire.IO == Connection.Both)
						(te as IEnergyProvider)?
							.GetEnergyStorage()
							.ModifyEnergyStored(
								-grid.energy.ReceiveEnergy(TheOneLibrary.Utility.Utility.Min(grid.energy.GetMaxReceive(), grid.energy.GetCapacity() - grid.energy.GetEnergy(), ((IEnergyProvider)te).GetEnergyStorage().GetEnergy())));

					if (wire.IO == Connection.Out || wire.IO == Connection.Both)
						(te as IEnergyReceiver)?
							.GetEnergyStorage()
							.ModifyEnergyStored(grid.energy.ExtractEnergy(TheOneLibrary.Utility.Utility.Min(grid.energy.GetMaxExtract(), grid.energy.GetEnergy(), ((IEnergyReceiver)te).GetCapacity() - ((IEnergyReceiver)te).GetEnergy())));
				}
			}
		}

		public void Place(Point16 mouse, Player player)
		{
			if (Main.LocalPlayer.inventory.Any(x => x.modItem is BaseWire))
			{
				// Consumes item
				int type = player.inventory.First(x => x.modItem is BaseWire).type;
				Item item = player.inventory.First(x => x.type == type);
				item.stack--;
				if (item.stack <= 0) item.TurnToAir();

				// Creates a wire and places it in grid
				Wire wire = new Wire();
				wire.SetDefaults(type);
				wire.position = mouse;
				elements.Add(mouse, wire);

				// Creates a grid and set base values
				WireGrid grid = new WireGrid();
				grid.energy.SetMaxTransfer(wire.maxIO);
				grid.energy.SetCapacity(wire.maxIO * 2);
				grid.tiles.Add(wire);
				wire.grid = grid;

				// Merges and frames the wire
				wire.Merge();
				wire.Frame();

				// Frames all surrounding wires
				foreach (Point16 add in TheOneLibrary.Utility.Utility.CheckNeighbours())
					if (elements.ContainsKey(mouse.X + add.X, mouse.Y + add.Y))
					{
						Wire merge = elements[mouse + add];
						if (merge.type == type) merge.Frame();
					}
			}
		}

		public void Remove(Point16 mouse, Player player)
		{
			Wire wire = elements[mouse];

			elements.Remove(mouse);
			wire.grid.tiles.Remove(wire);
			wire.grid.ReformGrid();

			foreach (Point16 check in TheOneLibrary.Utility.Utility.CheckNeighbours()) if (elements.ContainsKey(mouse + check) && elements[mouse + check].type == wire.type) elements[mouse + check].Frame();

			player.PutItemInInventory(wire.type);
		}

		public void Modify(Point16 mouse)
		{
			Wire wire = elements[mouse];

			int x = (int)Main.MouseWorld.X - mouse.X * 16;
			int y = (int)Main.MouseWorld.Y - mouse.Y * 16;

			Rectangle io = new Rectangle(4, 4, 8, 8);
			if (!io.Contains(x, y))
			{
				if (TheOneLibrary.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 0), new Point(8, 8), new Point(0, 16)))
				{
					wire.connections[Left] = !wire.connections[Left];
					wire.Frame();

					Main.NewText(wire.grid.tiles.Count.ToString());
					if (!wire.connections[Left]) wire.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X - 1, mouse.Y))
					{
						elements[mouse.X - 1, mouse.Y].connections[Right] = !elements[mouse.X - 1, mouse.Y].connections[Right];
						if (wire.connections[Left])
						{
							Main.NewText(wire.grid.tiles.Count + " " + elements[mouse.X - 1, mouse.Y].grid.tiles.Count);
							wire.grid.MergeGrids(elements[mouse.X - 1, mouse.Y].grid);
						}
						elements[mouse.X - 1, mouse.Y].Frame();
					}
				}
				else if (TheOneLibrary.Utility.Utility.PointInTriangle(new Point(x, y), new Point(16, 0), new Point(16, 16), new Point(8, 8)))
				{
					wire.connections[Right] = !wire.connections[Right];
					wire.Frame();

					if (!wire.connections[Right]) wire.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X + 1, mouse.Y))
					{
						elements[mouse.X + 1, mouse.Y].connections[Left] = !elements[mouse.X + 1, mouse.Y].connections[Left];
						if (wire.connections[Right]) wire.grid.MergeGrids(elements[mouse.X + 1, mouse.Y].grid);
						elements[mouse.X + 1, mouse.Y].Frame();
					}
				}
				else if (TheOneLibrary.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 0), new Point(16, 0), new Point(8, 8)))
				{
					wire.connections[Up] = !wire.connections[Up];
					wire.Frame();

					if (!wire.connections[Up]) wire.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X, mouse.Y - 1))
					{
						elements[mouse.X, mouse.Y - 1].connections[Down] = !elements[mouse.X, mouse.Y - 1].connections[Down];
						if (wire.connections[Up]) wire.grid.MergeGrids(elements[mouse.X, mouse.Y - 1].grid);
						elements[mouse.X, mouse.Y - 1].Frame();
					}
				}
				else if (TheOneLibrary.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 16), new Point(8, 8), new Point(16, 16)))
				{
					wire.connections[Down] = !wire.connections[Down];
					wire.Frame();

					if (!wire.connections[Down]) wire.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X, mouse.Y + 1))
					{
						elements[mouse.X, mouse.Y + 1].connections[Up] = !elements[mouse.X, mouse.Y + 1].connections[Up];
						if (wire.connections[Down]) wire.grid.MergeGrids(elements[mouse.X, mouse.Y + 1].grid);
						elements[mouse.X, mouse.Y + 1].Frame();
					}
				}
			}
			else wire.IO = wire.IO.NextEnum();
		}

		public void Info(Point16 mouse)
		{
			Wire wire = elements[mouse];

			Main.NewText("Tiles: " + wire.grid.tiles.Count);
			Main.NewText("Current capacity: " + wire.grid.energy.GetCapacity());
			Main.NewText("Current IO: " + wire.grid.energy.GetMaxReceive() + "/" + wire.grid.energy.GetMaxExtract());
			Main.NewText("Current energy: " + wire.grid.energy.GetEnergy());
		}

		public CustomDictionary<ILayerElement> GetElements() => elements.ToDictionary();

		public LayerInfo GetInfo() => new LayerInfo
		{
			Mod = DawnOfIndustryCore.Instance,
			Name = "Wires",
			Texture = DawnOfIndustryCore.PlaceholderTexturePath,
			Draw = LayerManager.ActiveLayer == this,
			DrawPreview = (TheOneLibrary.Utility.Utility.HeldItem.modItem as LayerTool)?.mode == LayerTool.Mode.Place
		};
	}
}