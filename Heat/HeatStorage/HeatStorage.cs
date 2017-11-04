using BaseLib.Utility;
using System;
using Terraria.ModLoader.IO;

namespace DawnOfIndustryCore.Heat.HeatStorage
{
	public class HeatStorage : IHeatStorage
	{
		internal long heat;
		internal long capacity;
		internal long maxReceive;
		internal long maxExtract;

		public HeatStorage(long capacity)
		{
			this.capacity = capacity;
			maxReceive = capacity;
			maxExtract = capacity;
		}

		public HeatStorage(long capacity, long maxTransfer)
		{
			this.capacity = capacity;
			maxReceive = maxTransfer;
			maxExtract = maxTransfer;
		}

		public HeatStorage(long capacity, long maxReceive, long maxExtract)
		{
			this.capacity = capacity;
			this.maxReceive = maxReceive;
			this.maxExtract = maxExtract;
		}

		public HeatStorage SetCapacity(long capacity)
		{
			this.capacity = capacity;
			if (heat > capacity) heat = capacity;

			return this;
		}

		public HeatStorage AddCapacity(long capacity)
		{
			this.capacity += capacity;
			if (heat > capacity) heat = capacity;

			return this;
		}

		public HeatStorage SetMaxTransfer(long maxTransfer)
		{
			SetMaxReceive(maxTransfer);
			SetMaxExtract(maxTransfer);
			return this;
		}

		public HeatStorage SetMaxReceive(long maxReceive)
		{
			this.maxReceive = maxReceive;
			return this;
		}

		public HeatStorage SetMaxExtract(long maxExtract)
		{
			this.maxExtract = maxExtract;
			return this;
		}

		public long GetMaxReceive() => maxReceive;

		public long GetMaxExtract() => maxExtract;

		public void ModifyHeatStored(long heat)
		{
			this.heat += heat;

			if (this.heat > capacity) this.heat = capacity;
			else if (this.heat < 0) this.heat = 0;
		}

		public long ReceiveHeat(long maxReceive)
		{
			long heatReceived = Math.Min(capacity - heat, Math.Min(this.maxReceive, maxReceive));
			heat += heatReceived;

			return heatReceived;
		}

		public long ExtractHeat(long maxExtract)
		{
			long heatExtracted = Math.Min(heat, Math.Min(this.maxExtract, maxExtract));
			heat -= heatExtracted;

			return heatExtracted;
		}

		public long GetHeat() => heat;

		public long GetCapacity() => capacity;

		public HeatStorage GetHeatStorage() => this;

		public override string ToString() => heat.ToSI() + "/" + capacity.ToSI() + "HU";
	}

	public class HeatSerializer : TagSerializer<HeatStorage, TagCompound>
	{
		public override TagCompound Serialize(HeatStorage value)
		{
			TagCompound tag = new TagCompound();
			if (value.heat < 0) value.heat = 0;

			tag.Set("Heat", value.heat);
			tag.Set("Capacity", value.capacity);
			tag.Set("MaxIn", value.maxReceive);
			tag.Set("MaxOut", value.maxExtract);
			return tag;
		}

		public override HeatStorage Deserialize(TagCompound tag)
		{
			HeatStorage storage = new HeatStorage(tag.GetLong("Capacity"), tag.GetLong("MaxIn"), tag.GetLong("MaxOut"));
			storage.heat = tag.GetLong("Heat") > storage.capacity ? storage.capacity : tag.GetLong("Heat");

			return storage;
		}
	}
}