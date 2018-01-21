using DawnOfIndustryCore.Global;
using Terraria;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Buffs
{
	public class Radiation : ModBuff
	{
		public override bool Autoload(ref string name, ref string texture)
		{
			texture = DawnOfIndustryCore.BuffTexturePath + "Radiation";
			return mod.Properties.Autoload;
		}

		public override void SetDefaults()
		{
			DisplayName.SetDefault("Radiated");
			Description.SetDefault("Your cells are falling apart");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<DoIPlayer>(mod).radiation = true;
		}
	}
}