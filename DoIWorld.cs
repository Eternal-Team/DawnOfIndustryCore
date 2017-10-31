using DawnOfIndustryCore.Wiring;
using EnergyLib.Energy;
using LayerLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static BaseLib.Utility.Utility;

namespace DawnOfIndustryCore
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
						Wire wire = elements[i, j];
						wire.Draw(Main.spriteBatch, i, j);
					}
				}
			}
		}

		public void Update()
		{

		}

		public Dictionary<Point16, ILayerElement> GetElements() => elements.ToDictionary();
	}

	public class DoIWorld : ModWorld
	{
		public WireLayer wires = new WireLayer();

		public override void Initialize()
		{
			LayerManager.RegisterLayer("Wires", wires);
		}

		public override TagCompound Save() => wires.Save();

		public override void Load(TagCompound tag)
		{
			try
			{
				wires.Load(tag);

				foreach (Wire wire in wires.elements.Values)
				{
					MultiTileGrid grid = new MultiTileGrid();
					grid.energy.SetCapacity(wire.maxIO * 2);
					grid.energy.SetMaxTransfer(wire.maxIO);
					grid.tiles.Add(wire);
					grid.energy.ModifyEnergyStored(wire.share);
					wire.share = 0;
					wire.SetGrid(grid);
				}

				foreach (Wire wire in wires.elements.Values) wire.Merge();
			}
			catch (Exception ex)
			{
				ErrorLogger.Log(ex);
			}
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetReceive(BinaryReader reader) => Load(TagIO.Read(reader));

		public override void PreUpdate()
		{
			foreach (Wire wire in wires.elements.Values)
			{
				Point16 check = TileEntityTopLeft(wire.position.X, wire.position.Y);

				if (TileEntity.ByPosition.ContainsKey(check))
				{
					MultiTileGrid grid = wire.GetGrid();
					TileEntity te = TileEntity.ByPosition[check];

					if (wire.IO == Connection.In || wire.IO == Connection.Both)
						(te as IEnergyProvider)?
							.GetEnergyStorage()
							.ModifyEnergyStored(
								-grid.energy.ReceiveEnergy(Min(grid.energy.GetMaxReceive(), grid.energy.GetCapacity() - grid.energy.GetEnergyStored(), ((IEnergyProvider)te).GetEnergyStorage().GetEnergyStored())));

					if (wire.IO == Connection.Out || wire.IO == Connection.Both)
						(te as IEnergyReceiver)?
							.GetEnergyStorage()
							.ModifyEnergyStored(grid.energy.ExtractEnergy(Min(grid.energy.GetMaxExtract(), grid.energy.GetEnergyStored(), ((IEnergyReceiver)te).GetCapacity() - ((IEnergyReceiver)te).GetEnergyStored())));
				}
			}
		}
	}
}