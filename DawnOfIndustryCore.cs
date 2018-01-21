using DawnOfIndustryCore.Heat;
using DawnOfIndustryCore.Power;
using DawnOfIndustryCore.Research.Logic;
using DawnOfIndustryCore.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base;
using TheOneLibrary.Utility;

namespace DawnOfIndustryCore
{
	public class DawnOfIndustryCore : Mod
	{
		[Null] public static DawnOfIndustryCore Instance;

		public const string TexturePath = "DawnOfIndustryCore/Textures/";
		public const string PlaceholderTexture = TexturePath + "Placeholder";
		public const string ItemTexturePath = TexturePath + "Items/";
		public const string TileTexturePath = TexturePath + "Tiles/";
		public const string BuffTexturePath = TexturePath + "Buffs/";
		public const string UIPath = TexturePath + "UI/";

		[Null] public static Texture2D wireTexture;
		[Null] public static Texture2D heatPipeTexture;

		[Null] public static Texture2D inTexture;
		[Null] public static Texture2D outTexture;
		[Null] public static Texture2D bothTexture;
		[Null] public static Texture2D blockedTexture;

		[Null] public static Texture2D textureBlueprint;
		[Null] public static Texture2D lightningCap;
		[Null] public static Texture2D lightningSegment;

		public ResearchUI ResearchUI;
		public UserInterface IResearchUI;

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
			try
			{
				Instance = this;
				
				TagSerializer.AddSerializer(new WireSerializer());
				TagSerializer.AddSerializer(new HeatPipeSerializer());

				//Main.OnPreDraw += PreDraw;
				//Main.OnPostDraw += PostDraw;

				if (!Main.dedServ)
				{
					wireTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/BasicWire");
					heatPipeTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/HeatPipe");

					inTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionIn");
					outTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionOut");
					bothTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionBoth");
					blockedTexture = ModLoader.GetTexture("DawnOfIndustryCore/Textures/Tiles/ConnectionBlocked");

					textureBlueprint = ModLoader.GetTexture(UIPath + "Blueprint");
					lightningCap = ModLoader.GetTexture(TexturePath + "LightningCap");
					lightningSegment = ModLoader.GetTexture(TexturePath + "LightningSegment");

					ResearchUI = new ResearchUI();
					ResearchUI.Activate();
					IResearchUI = new UserInterface();
					IResearchUI.SetState(ResearchUI);

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
			catch (Exception ex)
			{
				ErrorLogger.Log(ex);
			}
		}

		public override void PostSetupContent()
		{
			try
			{
				ModTranslation translation = CreateTranslation("Start");
				translation.SetDefault("The starting tab");
				AddTranslation(translation);

				translation = CreateTranslation("Advanced");
				translation.SetDefault("the second tab");
				AddTranslation(translation);
				
				Research.Logic.Research.RegisterCategory(this, "Start", PlaceholderTexture);
				Research.Logic.Research.RegisterCategory(this, "Advanced", ItemTexturePath + "Thatch");
				
				ResearchLoader.Autoload();
				ResearchLoader.SetupContent();

				ResearchUI.PostInit();
			}
			catch (Exception ex)
			{
				ErrorLogger.Log(ex);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int InventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

			if (InventoryIndex != -1)
			{
				if (!Main.ingameOptionsWindow && ResearchUI.visible)
				{
					layers.Insert(InventoryIndex + 1, new LegacyGameInterfaceLayer(
						"TDoICore: ResearchUI",
						delegate
						{
							IResearchUI.Update(Main._drawInterfaceGameTime);
							ResearchUI.Draw(Main.spriteBatch);

							return true;
						}, InterfaceScaleType.UI));
				}
			}
		}

		/*
		#region ints, dll imports
		public const int WM_PRINT = 0x0317;
		public const int WM_PRINTCLIENT = 0x0318;

		public const int PRF_CHECKVISIBLE = 0x00000001;
		public const int PRF_NONCLIENT = 0x00000002;
		public const int PRF_CLIENT = 0x00000004;
		public const int PRF_ERASEBKGND = 0x00000008;
		public const int PRF_CHILDREN = 0x00000010;
		public const int PRF_OWNED = 0x00000020;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern long SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
		#endregion

		// 55-60 fps, doesn't have to be visible, higher CPU usage	
		public Bitmap CaptureControlImage(Control ctrl, IntPtr handle)
		{
			Bitmap bmp = new Bitmap(ctrl.Width, ctrl.Height);

			Graphics gBmp = Graphics.FromImage(bmp);

			IntPtr dcBmp = gBmp.GetHdc();

			SendMessage(handle, WM_PRINT, dcBmp.ToInt32(), PRF_CLIENT | PRF_CHILDREN);

			gBmp.ReleaseHdc(dcBmp);

			return bmp;
		}

		private void PostDraw(GameTime obj)
		{
			Task.Run(delegate
			{
				using (MemoryStream stream = new MemoryStream())
				{
					Bitmap map = CaptureControlImage(browser, handle);
					map.Save(stream, ImageFormat.Png);

					texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, stream);
				}
			});

			if (texture != null)
			{
				Main.spriteBatch.Begin();

				Main.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(Main.screenWidth - 750, 20, 750, 422), Microsoft.Xna.Framework.Color.White);

				//Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, "MUEH", 120, 120, Color.DarkRed, Main.DiscoColor, Vector2.Zero);

				Main.spriteBatch.End();
			}
		}

		public void SetBrowserFeatureControlKey(string feature, string appName, uint value)
		{
			using (var key = Registry.CurrentUser.CreateSubKey(String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature), RegistryKeyPermissionCheck.ReadWriteSubTree))
			{
				key.SetValue(appName, value, RegistryValueKind.DWord);
			}
		}

		public void SetBrowserFeatureControl()
		{
			var fileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

			if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
				return;

			SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, GetBrowserEmulationMode());
			SetBrowserFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_MANAGE_SCRIPT_CIRCULAR_REFS", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_DOMSTORAGE ", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_GPU_RENDERING ", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_IVIEWOBJECTDRAW_DMLT9_WITH_GDI  ", fileName, 0);
			SetBrowserFeatureControlKey("FEATURE_DISABLE_LEGACY_COMPRESSION", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_LOCALMACHINE_LOCKDOWN", fileName, 0);
			SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_OBJECT", fileName, 0);
			SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_SCRIPT", fileName, 0);
			SetBrowserFeatureControlKey("FEATURE_DISABLE_NAVIGATION_SOUNDS", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_SCRIPTURL_MITIGATION", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_SPELLCHECKING", fileName, 0);
			SetBrowserFeatureControlKey("FEATURE_STATUS_BAR_THROTTLING", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_TABBED_BROWSING", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_VALIDATE_NAVIGATE_URL", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_WEBOC_DOCUMENT_ZOOM", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_WEBOC_POPUPMANAGEMENT", fileName, 0);
			SetBrowserFeatureControlKey("FEATURE_WEBOC_MOVESIZECHILD", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_ADDON_MANAGEMENT", fileName, 0);
			SetBrowserFeatureControlKey("FEATURE_WEBSOCKET", fileName, 1);
			SetBrowserFeatureControlKey("FEATURE_WINDOW_RESTRICTIONS ", fileName, 0);
			SetBrowserFeatureControlKey("FEATURE_XMLHTTP", fileName, 1);
		}

		private static uint GetBrowserEmulationMode()
		{
			int browserVersion = 7;
			using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
				RegistryKeyPermissionCheck.ReadSubTree,
				RegistryRights.QueryValues))
			{
				var version = ieKey.GetValue("svcVersion");
				if (null == version)
				{
					version = ieKey.GetValue("Version");
					if (null == version)
						throw new ApplicationException("Microsoft Internet Explorer is required!");
				}
				int.TryParse(version.ToString().Split('.')[0], out browserVersion);
			}

			UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode. Default value for Internet Explorer 11.
			switch (browserVersion)
			{
				case 7:
					mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.
					break;
				case 8:
					mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8
					break;
				case 9:
					mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
					break;
				case 10:
					mode = 10000; // Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 mode. Default value for Internet Explorer 10.
					break;
				default:
					// use IE11 mode by default
					break;
			}

			return mode;
		}

		private Form form;
		private WebBrowser browser;
		private bool locked;
		private IntPtr handle;
		public Texture2D texture;

		private void PreDraw(GameTime obj)
		{
			if (!locked)
			{
				SetBrowserFeatureControl();

				form = (Form)Control.FromHandle(Main.instance.Window.Handle);

				browser = new WebBrowser
				{
					Location = new System.Drawing.Point(0, 0),
					Name = "video",
					Size = new Size(750, 422),
					Visible = false
				};
				form.Controls.Add(browser);

				handle = browser.Handle;

				//&autoplay=1

				browser.Navigate("https://www.youtube.com/embed/DUaott0scI0?html5=1&controls=0&autoplay=1");

				locked = true;
			}
		}
		*/

		public override void Unload()
		{
			this.UnloadNullableTypes();

			GC.Collect();
		}
	}
}