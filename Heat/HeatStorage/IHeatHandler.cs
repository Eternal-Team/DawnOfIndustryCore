namespace DawnOfIndustryCore.Heat.HeatStorage
{
	public interface IHeatHandler
	{
		long GetHeat();

		long GetCapacity();

		HeatStorage GetHeatStorage();
	}
}