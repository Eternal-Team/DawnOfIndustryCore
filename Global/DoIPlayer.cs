using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Global
{
	public class DoIPlayer : ModPlayer
	{
		public bool radiation;
		public bool electrified;

		public override void SetupStartInventory(IList<Item> items)
		{
			items.ToList().RemoveAll(x => x.type == ItemID.CopperPickaxe || x.type == ItemID.CopperAxe || x.type == ItemID.CopperShortsword);
		}

		public override void UpdateBadLifeRegen()
		{
			if (electrified)
			{
				if (player.lifeRegen > 0) player.lifeRegen = 0;
				player.lifeRegenTime = 0;
				player.lifeRegen -= 8;
			}

			if (radiation)
			{
				if (player.lifeRegen > 0) player.lifeRegen = 0;
				player.lifeRegenTime = 0;
				player.lifeRegen -= 16;
			}
		}

		public override void UpdateDead()
		{
			electrified = false;
			radiation = false;
		}

		public override void ResetEffects()
		{
			electrified = false;
			radiation = false;
		}

		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (radiation)
			{
				if (Main.rand.Next(4) == 0 && drawInfo.shadow <= 0)
				{
					int dust = Dust.NewDust(player.Center, 4, 4, DustID.SparksMech, 0f, -1f, 175, Color.Green);
					Main.dust[dust].noGravity = true;
					if (Main.rand.Next(2) == 0) Main.dust[dust].velocity.X *= -1f;
					if (Main.rand.Next(2) == 0) Main.dust[dust].velocity.Y *= -1f;
					Main.playerDrawDust.Add(dust);
				}

				r *= 0.05f;
				g *= 0.8f;
				b *= 0.05f;
				fullBright = true;
			}
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (radiation) damageSource = PlayerDeathReason.ByCustomReason(" irradiated away");
			else if (electrified) damageSource = PlayerDeathReason.ByCustomReason(" forgot to not touch the wall outlet.");

			return true;
		}
	}
}