using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using TheOneLibrary.Heat.Heat;
using TheOneLibrary.Layer;
using TheOneLibrary.Layer.Items;
using TheOneLibrary.Layer.Layer;
using TheOneLibrary.Utility;

namespace DawnOfIndustryCore.Heat
{
	public class HeatPipeLayer : ILayer
	{
		public CustomDictionary<HeatPipe> elements = new CustomDictionary<HeatPipe>();

		public TagCompound Save() => new TagCompound
		{
			["Keys"] = elements.Keys.ToList(),
			["Values"] = elements.Values.ToList()
		};

		public void Load(TagCompound tag)
		{
			elements.internalDict = tag.GetList<Point16>("Keys")
				.Zip(tag.GetList<HeatPipe>("Values"), (x, y) => new KeyValuePair<Point16, HeatPipe>(x, y))
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

						HeatPipe wire = elements[i, j];
						if (GetInfo().Draw) wire.Draw(Main.spriteBatch, position);
					}
				}
			}
		}

		public void DrawPreview()
		{
			Point16 mouse = TheOneLibrary.Utility.Utility.MouseToWorldPoint();

			if (Main.LocalPlayer.inventory.Any(x => x.modItem is Items.Wires.HeatPipe) && !elements.ContainsKey(mouse))
				Main.spriteBatch.Draw(DawnOfIndustryCore.heatPipeTexture, mouse.ToVector2() * 16 - Main.screenPosition, new Rectangle(0, 0, 16, 16), Color.White * 0.5f, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
		}

		public void Update()
		{
			foreach (HeatPipe heatPipe in elements.Values)
			{
				Point16 check = TheOneLibrary.Utility.Utility.TileEntityTopLeft(heatPipe.position.X, heatPipe.position.Y);
				if (Main.tile[heatPipe.position.X, heatPipe.position.Y].liquidType() == Tile.Liquid_Lava) heatPipe.grid.heatStorage.ModifyHeatStored(Main.tile[heatPipe.position.X, heatPipe.position.Y].liquid);

				if (TileEntity.ByPosition.ContainsKey(check))
				{
					TileEntity te = TileEntity.ByPosition[check];
					HeatPipeGrid grid = heatPipe.grid;

					if (te is IHeatProvider)
					{
						IHeatProvider provider = (IHeatProvider)te;

						provider.GetHeatStorage().ModifyHeatStored(
							-grid.heatStorage.ReceiveHeat(TheOneLibrary.Utility.Utility.Min(grid.heatStorage.GetMaxReceive(), grid.heatStorage.GetCapacity() - grid.heatStorage.GetHeat(), provider.GetHeat())));
					}
					if (te is IHeatReceiver)
					{
						IHeatReceiver receiver = (IHeatReceiver)te;

						receiver.GetHeatStorage().ModifyHeatStored(
							grid.heatStorage.ExtractHeat(TheOneLibrary.Utility.Utility.Min(grid.heatStorage.GetMaxExtract(), grid.heatStorage.GetHeat(), receiver.GetCapacity() - receiver.GetHeat())));
					}
				}
			}
		}

		public void Place(Point16 mouse, Player player)
		{
			if (Main.LocalPlayer.inventory.Any(x => x.modItem is Items.Wires.HeatPipe))
			{
				int type = player.inventory.First(x => x.modItem is Items.Wires.HeatPipe).type;
				Item item = player.inventory.First(x => x.type == type);
				item.stack--;
				if (item.stack <= 0) item.TurnToAir();

				HeatPipe heatPipe = new HeatPipe();
				heatPipe.SetDefaults(type);
				heatPipe.position = mouse;
				elements.Add(mouse, heatPipe);

				HeatPipeGrid grid = new HeatPipeGrid();
				grid.tiles.Add(heatPipe);
				heatPipe.grid = grid;

				heatPipe.Merge();
				heatPipe.Frame();

				foreach (Point16 add in TheOneLibrary.Utility.Utility.CheckNeighbours())
					if (elements.ContainsKey(mouse.X + add.X, mouse.Y + add.Y))
					{
						HeatPipe merge = elements[mouse + add];
						if (merge.type == type) merge.Frame();
					}
			}
		}

		public void Remove(Point16 mouse, Player player)
		{
			HeatPipe heatPipe = elements[mouse];

			elements.Remove(mouse);
			heatPipe.grid.tiles.Remove(heatPipe);

			heatPipe.grid.ReformGrid();

			foreach (Point16 check in TheOneLibrary.Utility.Utility.CheckNeighbours()) if (elements.ContainsKey(mouse + check) && elements[mouse + check].type == heatPipe.type) elements[mouse + check].Frame();

			player.PutItemInInventory(heatPipe.type);
		}

		public void Modify(Point16 mouse)
		{
			HeatPipe heatPipe = elements[mouse];

			int x = (int)Main.MouseWorld.X - mouse.X * 16;
			int y = (int)Main.MouseWorld.Y - mouse.Y * 16;

			Rectangle io = new Rectangle(4, 4, 8, 8);
			if (!io.Contains(x, y))
			{
				if (TheOneLibrary.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 0), new Point(8, 8), new Point(0, 16)))
				{
					heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Left] = !heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Left];
					heatPipe.Frame();

					Main.NewText(heatPipe.grid.tiles.Count.ToString());
					if (!heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Left]) heatPipe.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X - 1, mouse.Y))
					{
						HeatPipe other = elements[mouse.X - 1, mouse.Y];
						other.connections[TheOneLibrary.Utility.Utility.Facing.Right] = !other.connections[TheOneLibrary.Utility.Utility.Facing.Right];
						if (heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Left]) heatPipe.grid.MergeGrids(other.grid);
						other.Frame();
					}
				}
				else if (TheOneLibrary.Utility.Utility.PointInTriangle(new Point(x, y), new Point(16, 0), new Point(16, 16), new Point(8, 8)))
				{
					heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Right] = !heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Right];
					heatPipe.Frame();

					if (!heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Right]) heatPipe.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X + 1, mouse.Y))
					{
						HeatPipe other = elements[mouse.X + 1, mouse.Y];
						other.connections[TheOneLibrary.Utility.Utility.Facing.Left] = !elements[mouse.X + 1, mouse.Y].connections[TheOneLibrary.Utility.Utility.Facing.Left];
						if (heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Right]) heatPipe.grid.MergeGrids(elements[mouse.X + 1, mouse.Y].grid);
						elements[mouse.X + 1, mouse.Y].Frame();
					}
				}
				else if (TheOneLibrary.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 0), new Point(16, 0), new Point(8, 8)))
				{
					heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Up] = !heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Up];
					heatPipe.Frame();

					if (!heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Up]) heatPipe.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X, mouse.Y - 1))
					{
						HeatPipe other = elements[mouse.X, mouse.Y - 1];
						other.connections[TheOneLibrary.Utility.Utility.Facing.Down] = !other.connections[TheOneLibrary.Utility.Utility.Facing.Down];
						if (heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Up]) heatPipe.grid.MergeGrids(other.grid);
						other.Frame();
					}
				}
				else if (TheOneLibrary.Utility.Utility.PointInTriangle(new Point(x, y), new Point(0, 16), new Point(8, 8), new Point(16, 16)))
				{
					heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Down] = !heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Down];
					heatPipe.Frame();

					if (!heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Down]) heatPipe.grid.ReformGrid();

					if (elements.ContainsKey(mouse.X, mouse.Y + 1))
					{
						HeatPipe other = elements[mouse.X, mouse.Y + 1];
						other.connections[TheOneLibrary.Utility.Utility.Facing.Up] = !other.connections[TheOneLibrary.Utility.Utility.Facing.Up];
						if (heatPipe.connections[TheOneLibrary.Utility.Utility.Facing.Down]) heatPipe.grid.MergeGrids(other.grid);
						other.Frame();
					}
				}
			}
		}

		public void Info(Point16 mouse)
		{
			HeatPipe heatPipe = elements[mouse];

			Main.NewText("Tiles: " + heatPipe.grid.tiles.Count);
			Main.NewText("Current heat: " + heatPipe.grid.heatStorage.GetHeat());
		}

		public CustomDictionary<ILayerElement> GetElements() => elements.ToDictionary();

		public LayerInfo GetInfo() => new LayerInfo
		{
			Mod = DawnOfIndustryCore.Instance,
			Name = "Heat Pipes",
			Texture = DawnOfIndustryCore.PlaceholderTexturePath,
			Draw = LayerManager.ActiveLayer == this,
			DrawPreview = (TheOneLibrary.Utility.Utility.HeldItem.modItem as LayerTool)?.mode == LayerTool.Mode.Place
		};
	}
}