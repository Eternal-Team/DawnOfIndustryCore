namespace DawnOfIndustryCore.Heat.HeatStorage
{
	public interface IHeatProvider : IHeatHandler
	{
		long ExtractHeat(long maxExtract);
	}
}