using EnergyLib.Energy;
using System.Collections.Generic;

namespace DawnOfIndustryCore.Wiring
{
	public class MultiTileGrid
	{
		public IList<Wire> tiles = new List<Wire>();
		public EnergyStorage energy = new EnergyStorage(10000, 1000);

		public long GetCapacitySharePerNode() => energy.GetCapacity() / tiles.Count;

		public long GetEnergySharePerNode() => energy.GetEnergyStored() / tiles.Count;

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

		public void MergeGrids(MultiTileGrid multiTileGrid)
		{
			for (int i = 0; i < multiTileGrid.tiles.Count; i++) AddTile(multiTileGrid.tiles[i]);
		}

		public void ReformGrid()
		{
			for (int i = 0; i < tiles.Count; i++)
			{
				MultiTileGrid newGrid = new MultiTileGrid();
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