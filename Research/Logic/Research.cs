using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Research.Logic
{
	public class Research
	{
		public static IDictionary<string, ResearchCategory> researchCategories = new Dictionary<string, ResearchCategory>();

		public static ResearchCategory GetCategory(string key) => researchCategories[key];

		public static ModResearch GetResearch(string key) => researchCategories.Select(x => x.Value.research.Select(v => v.Value)).Aggregate((x, y) => x.Concat(y)).FirstOrDefault(x => x.Name == key);

		///	<param name="key">the key used for this category</param>
		/// <param name="icon">the icon to be used for the research category tab</param>
		public static void RegisterCategory(Mod mod, string key, string icon)
		{
			if (!researchCategories.ContainsKey(key))
			{
				ResearchCategory category = new ResearchCategory(mod, key, icon);
				researchCategories[key] = category;
			}
		}

		public static void AddResearch(ModResearch item)
		{
			ResearchCategory category = GetCategory(item.category);
			if (category != null && !category.research.ContainsKey(item.Name)) category.research[item.Name] = item;
		}
	}
}