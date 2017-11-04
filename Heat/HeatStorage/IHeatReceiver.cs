namespace DawnOfIndustryCore.Heat.HeatStorage
{
	public interface IHeatReceiver : IHeatHandler
	{
		long ReceiveHeat(long maxReceive);
	}
}