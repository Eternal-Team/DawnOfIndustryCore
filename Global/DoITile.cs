using DawnOfIndustryCore.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Global
{
	public class DoITile : GlobalTile
	{
		public override void RightClick(int i, int j, int type)
		{
			if (type == TileID.Stone && Main.keyState.IsKeyDown(Keys.LeftShift) && Main.rand.Next(4) == 0)
				Item.NewItem(Main.LocalPlayer.position, Main.LocalPlayer.Size, mod.ItemType<Rock>(), Main.rand.Next(1, 4));
		}

		public override bool Drop(int i, int j, int type)
		{
			//if (type == TileID.Sand&&Main.rand.Next(4)==0) // drop sillica sand

			return true;
		}

		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (type == TileID.Plants) Item.NewItem(new Vector2(i, j) * 16, new Vector2(16), mod.ItemType<Thatch>(), Main.rand.Next(1, 3));
		}
	}
}