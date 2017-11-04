namespace DawnOfIndustryCore.Power
{
	public interface IMultiTile
	{
		WireGrid GetGrid();

		void SetGrid(WireGrid grid);

		long GetCapacity();
	}
}