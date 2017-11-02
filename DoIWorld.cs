using DawnOfIndustryCore.Wiring;
using LayerLib;
using System;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DawnOfIndustryCore
{
	public class DoIWorld : ModWorld
	{
		public WireLayer wires = new WireLayer();

		public override void Initialize()
		{
			LayerManager.RegisterLayer(wires);
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
	}
}