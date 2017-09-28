using Terraria;
using Terraria.ModLoader;

namespace DawnOfIndustryCore
{
	public class DNPC : GlobalNPC
	{
		public bool electrified;

		public override bool InstancePerEntity => true;

		public override void ResetEffects(NPC npc)
		{
			electrified = false;
		}

		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			if (electrified)
			{
				if (npc.lifeRegen > 0)
				{
					npc.lifeRegen = 0;
				}
				if (npc.boss) npc.lifeRegen -= 8;
				if (npc.lifeMax > 500) npc.lifeRegen -= 16;
				else npc.lifeRegen -= 24;
			}
		}
	}
}