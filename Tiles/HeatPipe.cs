using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DawnOfIndustryCore.Tiles
{
	public class HeatPipe : ModTile
	{
		public override bool Autoload(ref string name, ref string texture)
		{
			texture = DawnOfIndustryCore.TileTexturePath + "HeatPipe";
			return base.Autoload(ref name, ref texture);
		}

		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileNoAttach[Type] = false;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(Type);
			drop = mod.ItemType<Items.Wires.HeatPipe>();
			minPick = 50;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Heat Pipe");
			AddMapEntry(Color.Gray, name);
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			short frameX = 0;
			short frameY = 0;

			if (WorldGen.InWorld(i - 1, j) && Main.tile[i - 1, j].active())
			{
				ModTile tile = TileLoader.GetTile(Main.tile[i - 1, j].type);
				if (tile is HeatPipe) frameX += 18;
			}
			if (WorldGen.InWorld(i + 1, j) && Main.tile[i + 1, j].active())
			{
				ModTile tile = TileLoader.GetTile(Main.tile[i + 1, j].type);
				if (tile is HeatPipe) frameX += 36;
			}
			if (WorldGen.InWorld(i, j - 1) && Main.tile[i, j - 1].active())
			{
				ModTile tile = TileLoader.GetTile(Main.tile[i, j - 1].type);
				if (tile is HeatPipe) frameY += 18;
			}
			if (WorldGen.InWorld(i, j + 1) && Main.tile[i, j + 1].active())
			{
				ModTile tile = TileLoader.GetTile(Main.tile[i, j + 1].type);
				if (tile is HeatPipe) frameY += 36;
			}

			Main.tile[i, j].frameX = frameX;
			Main.tile[i, j].frameY = frameY;
			return false;
		}
	}
}