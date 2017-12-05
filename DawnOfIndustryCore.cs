using DawnOfIndustryCore.Heat;
using DawnOfIndustryCore.Power;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Utility;

namespace DawnOfIndustryCore
{
	public class DawnOfIndustryCore : Mod
	{
		public static DawnOfIndustryCore Instance;

		public const string PlaceholderTexturePath = "DawnOfIndustryCore/Textures/Placeholder";
		public const string ItemTexturePath = "DawnOfIndustryCore/Textures/Items/";
		public const string TileTexturePath = "DawnOfIndustryCore/Textures/Tiles/";
		public const string BuffTexturePath = "DawnOfIndustryCore/Textures/Buffs/";

		public static Texture2D wireTexture;
		public static Texture2D heatPipeTexture;

		public static Texture2D inTexture;
		public static Texture2D outTexture;
		public static Texture2D bothTexture;
		public static Texture2D blockedTexture;

		public DawnOfIndustryCore()
		{
			Properties = new ModProperties
			{
				Autoload = true,
				AutoloadBackgrounds = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public static bool ChangedTitle;

		public override void Load()
		{
			Instance = this;

			wireTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/BasicWire");
			heatPipeTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/HeatPipe");

			inTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionIn");
			outTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionOut");
			bothTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionBoth");
			blockedTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionBlocked");

			TagSerializer.AddSerializer(new WireSerializer());
			TagSerializer.AddSerializer(new HeatPipeSerializer());

			if (!Main.dedServ)
			{
				try
				{
					Platform.Current.SetWindowUnicodeTitle(Main.instance.Window, "Terraria: Dawn of Industry");
				}
				catch (Exception ex)
				{
					this.Log($"Failed to change title:\n{ex}");
				}
			}
		}

		public override void Unload()
		{
			wireTexture = null;
			heatPipeTexture = null;

			inTexture = null;
			outTexture = null;
			bothTexture = null;
			blockedTexture = null;

			Instance = null;

			GC.Collect();
		}
	}
}