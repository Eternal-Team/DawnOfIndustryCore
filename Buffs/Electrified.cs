using Terraria;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Buffs
{
	public class Electrified : ModBuff
	{
		public override bool Autoload(ref string name, ref string texture)
		{
			texture = DawnOfIndustryCore.BuffTexturePath + "Electrified";
			return mod.Properties.Autoload;
		}

		public override void SetDefaults()
		{
			DisplayName.SetDefault("Electrified");
			Description.SetDefault("Losing life");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<DoIPlayer>(mod).electrified = true;
			Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.3f, 0.8f, 1.1f);
		}
	}
}