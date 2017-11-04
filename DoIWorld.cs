using DawnOfIndustryCore.Heat;
using DawnOfIndustryCore.Power;
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
		public HeatPipeLayer heatPipes = new HeatPipeLayer();

		public override void Initialize()
		{
			LayerManager.RegisterLayer(wires);
			LayerManager.RegisterLayer(heatPipes);
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			tag["Wires"] = wires.Save();
			tag["HeatPipes"] = heatPipes.Save();
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			try
			{
				wires.Load((TagCompound)tag["Wires"]);
				heatPipes.Load((TagCompound)tag["HeatPipes"]);

				foreach (Wire wire in wires.elements.Values)
				{
					WireGrid grid = new WireGrid();
					grid.energy.SetCapacity(wire.maxIO * 2);
					grid.energy.SetMaxTransfer(wire.maxIO);
					grid.tiles.Add(wire);
					grid.energy.ModifyEnergyStored(wire.share);
					wire.share = 0;
					wire.grid = grid;
				}
				foreach (Wire wire in wires.elements.Values) wire.Merge();

				foreach (HeatPipe heatPipe in heatPipes.elements.Values)
				{
					HeatPipeGrid grid = new HeatPipeGrid();
					grid.tiles.Add(heatPipe);
					heatPipe.grid = grid;
				}
				foreach (HeatPipe heatPipe in heatPipes.elements.Values) heatPipe.Merge();
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