namespace DawnOfIndustryCore.Heat.HeatStorage
{
	public interface IHeatStorage : IHeatHandler
	{
		long ReceiveHeat(long maxReceive);

		long ExtractHeat(long maxExtract);
	}
}