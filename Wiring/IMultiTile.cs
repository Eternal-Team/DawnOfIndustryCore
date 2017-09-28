using Terraria.DataStructures;

namespace DawnOfIndustryCore.Wiring
{
	public interface IMultiTile
	{
		MultiTileGrid GetGrid();

		void SetGrid(MultiTileGrid grid);
		
		long GetCapacity();
	}
}