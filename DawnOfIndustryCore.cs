using DawnOfIndustryCore.Wiring;
using EnergyLib.Energy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace DawnOfIndustryCore
{
	public class DawnOfIndustryCore : Mod
	{
		public static DawnOfIndustryCore Instance;

		public const string PlaceholderTexturePath = "DawnOfIndustryCore/Textures/Placeholder";
		public const string ItemTexturePath = "DawnOfIndustryCore/Textures/Items/";
		public const string TileTexturePath = "DawnOfIndustryCore/Textures/Tiles/";
		public const string BuffTexturePath = "DawnOfIndustryCore/Textures/Buffs/";

		public Texture2D wireTexture;
		public Texture2D inTexture;
		public Texture2D outTexture;
		public Texture2D bothTexture;
		public Texture2D blockedTexture;

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

		public override void Load()
		{
			Instance = this;
			wireTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/BasicWire");
			inTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionIn");
			outTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionOut");
			bothTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionBoth");
			blockedTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionBlocked");

			TagSerializer.AddSerializer(new WireSerializer());
		}

		public override void Unload()
		{
			Instance = null;
		}

		public void DrawWires()
		{
			for (int i = (int)Main.screenPosition.X / 16 - 2; i <= (int)Main.screenPosition.X / 16 + Main.screenWidth / 16 + 2; i++)
			{
				for (int j = (int)Main.screenPosition.Y / 16 - 2; j <= (int)Main.screenPosition.Y / 16 + Main.screenHeight / 16 + 2; j++)
				{
					if (DoIWorld.wires.ContainsKey(i, j) && DoIWorld.wires[i, j].type > 0)
					{
						Wire wire = DoIWorld.wires[i, j];
						Vector2 position = -Main.screenPosition + new Vector2(i, j) * 16;

						Main.spriteBatch.Draw(wireTexture, position, new Rectangle(wire.frameX, wire.frameY, 16, 16), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

						Point16 tePos = BaseLib.Utility.Utility.TileEntityTopLeft(i, j);
						TileEntity tileEntity = TileEntity.ByPosition.ContainsKey(tePos) ? TileEntity.ByPosition[tePos] : null;
						if (tileEntity != null && (tileEntity is IEnergyReceiver || tileEntity is IEnergyProvider))
						{
							switch (wire.IO)
							{
								case Connection.In:
									Main.spriteBatch.Draw(inTexture, position + new Vector2(4), Color.White);
									break;
								case Connection.Out:
									Main.spriteBatch.Draw(outTexture, position + new Vector2(4), Color.White);
									break;
								case Connection.Both:
									Main.spriteBatch.Draw(bothTexture, position + new Vector2(4), Color.White);
									break;
								case Connection.Blocked:
									Main.spriteBatch.Draw(blockedTexture, position + new Vector2(4), Color.White);
									break;
							}
						}
					}
				}
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int index = layers.FindIndex(x => x.Name == "Vanilla: Interface Logic 1");

			if (index != -1)
			{
				layers.Insert(index, new LegacyGameInterfaceLayer(
					"DawnOfIndustryCore: Wires",
					delegate
					{
						if (BaseLib.Utility.Utility.HasWrench) DrawWires();

						return true;
					}));
			}
		}
	}
}