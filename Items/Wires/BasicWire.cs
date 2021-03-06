﻿namespace DawnOfIndustryCore.Items.Wires
{
	public class BasicWire : BaseWire
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Basic Wire");
			Tooltip.SetDefault("Transfers electricity\nMax IO: 1000 TF/s");
		}

		public override void SetDefaults()
		{
			maxIO = 1000;
			base.SetDefaults();
		}
	}
}