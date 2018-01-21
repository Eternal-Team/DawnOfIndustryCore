using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace DawnOfIndustryCore.Research.Logic
{
	public class ModResearch
	{
		public string Name { get; internal set; }

		public Mod Mod { get; internal set; }

		public ModTranslation DisplayName { get; internal set; }

		public ModTranslation Tooltip { get; internal set; }

		public virtual string Texture => DawnOfIndustryCore.PlaceholderTexture;

		public int type;
		public string category;
		public Vector2 position;

		public virtual ModResearch NewInstance()
		{
			var copy = (ModResearch)Activator.CreateInstance(GetType());
			copy = this;
			return copy;
		}

		public virtual void SetStaticDefaults()
		{
		}

		public void AutoStaticDefaults()
		{
			ResearchLoader.researchTexture[Name] = ModLoader.GetTexture(Texture);
		}

		public virtual void SetDefaults()
		{
		}

		public virtual List<string> Parents => new List<string>();
	}
}