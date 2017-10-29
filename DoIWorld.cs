using DawnOfIndustryCore.Wiring;
using EnergyLib.Energy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static BaseLib.Utility.Utility;

namespace DawnOfIndustryCore
{
	public class DoIWorld : ModWorld
	{
		public Layer<Wire> wires = new Layer<Wire>();

		public override TagCompound Save() => wires.Save();

		public override void Load(TagCompound tag)
		{
			try
			{
				wires.Load(tag);

				foreach (Wire wire in wires.elements.Values)
				{
					MultiTileGrid grid = new MultiTileGrid();
					grid.energy.SetCapacity(wire.maxIO * 2);
					grid.energy.SetMaxTransfer(wire.maxIO);
					grid.tiles.Add(wire);
					grid.energy.ModifyEnergyStored(wire.share);
					wire.share = 0;
					wire.SetGrid(grid);
				}

				foreach (Wire wire in wires.elements.Values) wire.Merge();
			}
			catch (Exception ex)
			{
				ErrorLogger.Log(ex);
			}
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetReceive(BinaryReader reader) => Load(TagIO.Read(reader));

		public override void PreUpdate()
		{
			foreach (Wire wire in wires.elements.Values)
			{
				Point16 check = TileEntityTopLeft(wire.position.X, wire.position.Y);

				if (TileEntity.ByPosition.ContainsKey(check))
				{
					MultiTileGrid grid = wire.GetGrid();
					TileEntity te = TileEntity.ByPosition[check];

					if (wire.IO == Connection.In || wire.IO == Connection.Both)
						(te as IEnergyProvider)?
							.GetEnergyStorage()
							.ModifyEnergyStored(
								-grid.energy.ReceiveEnergy(Min(grid.energy.GetMaxReceive(), grid.energy.GetCapacity() - grid.energy.GetEnergyStored(), ((IEnergyProvider)te).GetEnergyStorage().GetEnergyStored())));

					if (wire.IO == Connection.Out || wire.IO == Connection.Both)
						(te as IEnergyReceiver)?
							.GetEnergyStorage()
							.ModifyEnergyStored(grid.energy.ExtractEnergy(Min(grid.energy.GetMaxExtract(), grid.energy.GetEnergyStored(), ((IEnergyReceiver)te).GetMaxEnergyStored() - ((IEnergyReceiver)te).GetEnergyStored())));
				}
			}
		}

		public void DrawWires()
		{
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen) zero = Vector2.Zero;

			int num4 = (int)((Main.screenPosition.X - zero.X) / 16f - 1f);
			int num5 = (int)((Main.screenPosition.X + Main.screenWidth + zero.X) / 16f) + 2;
			int num6 = (int)((Main.screenPosition.Y - zero.Y) / 16f - 1f);
			int num7 = (int)((Main.screenPosition.Y + Main.screenHeight + zero.Y) / 16f) + 5;
			if (num4 < 4) num4 = 4;
			if (num5 > Main.maxTilesX - 4) num5 = Main.maxTilesX - 4;
			if (num6 < 4) num6 = 4;
			if (num7 > Main.maxTilesY - 4) num7 = Main.maxTilesY - 4;

			for (int i = num4 - 2; i < num5 + 2; i++)
			{
				for (int j = num6; j < num7 + 4; j++)
				{
					if (wires.elements.ContainsKey(i, j))
					{
						Wire wire = wires.elements[i, j];
						Vector2 position = -Main.screenPosition + new Vector2(i, j) * 16;
						SpriteEffects effects = Main.LocalPlayer.gravDir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

						Main.spriteBatch.Draw(DawnOfIndustryCore.wireTexture, position, new Rectangle(wire.frameX, wire.frameY, 16, 16), Color.White, 0f, Vector2.Zero, Vector2.One, effects, 0f);

						Point16 tePos = TileEntityTopLeft(i, j);
						TileEntity tileEntity = TileEntity.ByPosition.ContainsKey(tePos) ? TileEntity.ByPosition[tePos] : null;
						if (tileEntity != null && (tileEntity is IEnergyReceiver || tileEntity is IEnergyProvider))
						{
							switch (wire.IO)
							{
								case Connection.In:
									Main.spriteBatch.Draw(DawnOfIndustryCore.inTexture, position + new Vector2(4), Color.White);
									break;
								case Connection.Out:
									Main.spriteBatch.Draw(DawnOfIndustryCore.outTexture, position + new Vector2(4), Color.White);
									break;
								case Connection.Both:
									Main.spriteBatch.Draw(DawnOfIndustryCore.bothTexture, position + new Vector2(4), Color.White);
									break;
								case Connection.Blocked:
									Main.spriteBatch.Draw(DawnOfIndustryCore.blockedTexture, position + new Vector2(4), Color.White);
									break;
							}
						}
					}
				}
			}
		}

		public override void PostDrawTiles()
		{
			RasterizerState rasterizer = Main.gameMenu || Main.LocalPlayer.gravDir == 1.0 ? RasterizerState.CullCounterClockwise : RasterizerState.CullClockwise;
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			DrawWires();
			Main.spriteBatch.End();
		}
	}
}