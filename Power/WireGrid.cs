﻿using EnergyLib.Energy;
using System.Collections.Generic;

namespace DawnOfIndustryCore.Power
{
	public class WireGrid
	{
		public List<Wire> tiles = new List<Wire>();
		public EnergyStorage energy = new EnergyStorage(10000, 1000);

		public long GetCapacitySharePerNode() => energy.GetCapacity() / tiles.Count;

		public long GetEnergySharePerNode() => energy.GetEnergy() / tiles.Count;

		public void AddTile(Wire tile)
		{
			if (!tiles.Contains(tile))
			{
				energy.AddCapacity(tile.maxIO * 2);
				energy.ModifyEnergyStored(tile.GetGrid().GetEnergySharePerNode());
				tile.SetGrid(this);
				tiles.Add(tile);
			}
		}

		public void MergeGrids(WireGrid wireGrid)
		{
			for (int i = 0; i < wireGrid.tiles.Count; i++) AddTile(wireGrid.tiles[i]);
		}

		public void ReformGrid()
		{
			Terraria.Main.NewText(tiles.Count);

			for (int i = 0; i < tiles.Count; i++)
			{
				WireGrid newGrid = new WireGrid();
				newGrid.energy.SetMaxTransfer(tiles[i].maxIO);
				newGrid.energy.SetCapacity(tiles[i].maxIO * 2);
				newGrid.energy.ModifyEnergyStored(GetEnergySharePerNode());
				newGrid.tiles.Add(tiles[i]);
				tiles[i].SetGrid(newGrid);
			}

			for (int i = 0; i < tiles.Count; i++) tiles[i].Merge();
		}
	}
}