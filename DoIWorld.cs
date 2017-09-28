using DawnOfIndustryCore.Wiring;
using EnergyLib.Energy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DawnOfIndustryCore
{
	public class DoIWorld : ModWorld
	{
		public static CustomDictionary<Wire> wires = new CustomDictionary<Wire>();

		public override TagCompound Save() => new TagCompound
		{
			["Positions"] = wires.Keys.ToList(),
			["Wires"] = wires.Values.ToList()
		};

		public override void Load(TagCompound tag)
		{
			try
			{
				wires.internalDict = tag.GetList<Point16>("Positions").Zip(tag.GetList<Wire>("Wires"), (x, y) => new KeyValuePair<Point16, Wire>(x, y)).ToDictionary(x => x.Key, x => x.Value);

				for (int i = 0; i < wires.Count; i++)
				{
					Wire wire = wires.internalDict.Values.ToList()[i];
					MultiTileGrid grid = new MultiTileGrid();
					grid.energy.SetCapacity(wire.maxIO * 2);
					grid.energy.SetMaxTransfer(wire.maxIO);
					grid.tiles.Add(wire);
					grid.energy.ModifyEnergyStored(wire.share);
					wire.share = 0;
					wire.SetGrid(grid);
				}

				for (int i = 0; i < wires.Count; i++) wires.internalDict.Values.ToList()[i].Merge();
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
			for (int i = 0; i < wires.Count; i++)
			{
				Wire wire = wires.Values.ToList()[i];
				Point16 check = BaseLib.Utility.Utility.TileEntityTopLeft(wire.position.X, wire.position.Y);

				if (TileEntity.ByPosition.ContainsKey(check))
				{
					MultiTileGrid grid = wire.GetGrid();
					TileEntity te = TileEntity.ByPosition[check];

					if (wire.IO == Connection.In || wire.IO == Connection.Both) (te as IEnergyProvider)?.GetEnergyStorage().ModifyEnergyStored(-grid.energy.ReceiveEnergy(Math.Min(Math.Min(grid.energy.GetMaxReceive(), grid.energy.GetCapacity() - grid.energy.GetEnergyStored()), (te as IEnergyProvider).GetEnergyStorage().GetEnergyStored())));
					if (wire.IO == Connection.Out || wire.IO == Connection.Both) (te as IEnergyReceiver)?.GetEnergyStorage().ModifyEnergyStored(grid.energy.ExtractEnergy(Math.Min(Math.Min(grid.energy.GetMaxExtract(), grid.energy.GetEnergyStored()), (te as IEnergyReceiver).GetMaxEnergyStored() - (te as IEnergyReceiver).GetEnergyStored())));
				}
			}
		}
	}
}