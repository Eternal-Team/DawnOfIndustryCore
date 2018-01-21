using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;
using TheOneLibrary.Utility;

namespace DawnOfIndustryCore.Research.Logic
{
	public class ResearchCategory
	{
		public Mod mod;

		public string icon;

		public string key;

		public ResearchCategory(Mod mod, string key, string icon)
		{
			this.mod = mod;
			this.key = key;
			this.icon = icon;
		}

		public string GetName() => typeof(Mod).GetField<IDictionary<string, ModTranslation>>("translations", mod)[$"Mods.{mod.Name}.{key}"].GetTranslation(Language.ActiveCulture);

		public Dictionary<string, ModResearch> research = new Dictionary<string, ModResearch>();
	}
}