using DawnOfIndustryCore.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;
using TheOneLibrary.Base.Items;

namespace DawnOfIndustryCore.Items.Tools
{
	public class ShapedFlint : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shaped Flint");
			Tooltip.SetDefault("A worked piece of rock");
		}

		public override void SetDefaults()
		{
			item.damage = 5;
			item.melee = true;
			item.useTurn = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 30;
			item.useAnimation = 30;
			item.pick = 10;
			item.axe = 10;
			item.useStyle = 3;
			item.knockBack = 1;
			item.value = 100;
			item.rare = 1;
			item.UseSound = SoundID.Item1;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType<Rock>(), 2);
			recipe.SetResult(this);
			recipe.AddTile(TileID.Stone);
			recipe.AddRecipe();
		}
	}
}