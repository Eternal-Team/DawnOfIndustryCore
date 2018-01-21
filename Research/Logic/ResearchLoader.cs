using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Research.Logic
{
	public class ResearchLoader
	{
		public static IList<ModResearch> research = new List<ModResearch>();
		public static IDictionary<string, ModResearch> researchKey = new Dictionary<string, ModResearch>();
		public static IDictionary<string, Texture2D> researchTexture = new Dictionary<string, Texture2D>();
		public static int NextModuleID;

		public static ModResearch GetModule(int type) => research[type] != null ? research[type] : null;

		public static ModResearch GetModule(string name) => researchKey.ContainsKey(name) ? researchKey[name] : null;

		public static int ModuleType(string name)
		{
			ModResearch module = GetModule(name);
			return module.type;
		}

		public static int ModuleType<T>() where T : ModResearch => ModuleType(typeof(T).Name);

		public static void Autoload()
		{
			foreach (Mod mod in ModLoader.LoadedMods)
			{
				if (mod != null && mod.Code != null)
				{
					foreach (Type type in mod.Code.GetTypes())
					{
						if (!type.IsAbstract && type.IsSubclassOf(typeof(ModResearch))) AutoloadModule(type, mod);
					}
				}
			}
		}

		public static void Unload()
		{
			NextModuleID = 0;
			researchKey.Clear();
			research.Clear();
			researchTexture.Clear();
		}

		internal static void SetDefaults(ModResearch module)
		{
			module = GetModule(module.Name).NewInstance();

			module.SetDefaults();
		}

		public static void SetupContent()
		{
			foreach (ModResearch module in researchKey.Values)
			{
				SetDefaults(module);
				module.SetStaticDefaults();
				module.AutoStaticDefaults();
				Research.AddResearch(module);
			}
		}

		public static int NextModule()
		{
			int num = NextModuleID;
			NextModuleID++;
			return num;
		}

		public static void AddModule(string name, ModResearch module)
		{
			module.type = NextModule();
			module.Name = name;
			researchKey[name] = module;
			research.Add(module);
		}

		private static void AutoloadModule(Type type, Mod mod)
		{
			ModResearch module = (ModResearch)Activator.CreateInstance(type);
			string name = type.Name;

			module.DisplayName = mod.CreateTranslation(mod.Name + "." + name);
			module.Tooltip = mod.CreateTranslation(name);
			module.Mod = mod;
			AddModule(name, module);
		}
	}
}