using System.Collections.Generic;

namespace DawnOfIndustryCore.Heat
{
	public class HeatPipeGrid
	{
		public List<HeatPipe> tiles = new List<HeatPipe>();

		public HeatStorage.HeatStorage heatStorage = new HeatStorage.HeatStorage(10000, 1000);

		public void AddTile(HeatPipe tile)
		{
			if (!tiles.Contains(tile))
			{
				tile.grid = this;
				tiles.Add(tile);
			}
		}

		public void MergeGrids(HeatPipeGrid wireGrid)
		{
			for (int i = 0; i < wireGrid.tiles.Count; i++) AddTile(wireGrid.tiles[i]);
		}

		public void ReformGrid()
		{
			for (int i = 0; i < tiles.Count; i++)
			{
				HeatPipeGrid newGrid = new HeatPipeGrid();
				newGrid.tiles.Add(tiles[i]);
				tiles[i].grid = newGrid;
			}

			for (int i = 0; i < tiles.Count; i++) tiles[i].Merge();
		}
	}
}