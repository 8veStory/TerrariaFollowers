using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader;
using gaomonmod1dot4.Items;

namespace gaomonmod1dot4
{
	public class PlayerMod : ModPlayer
	{
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) {
			return new[] {
				new Item(ModContent.ItemType<GloryBag>(), 1),
			};
		}
	}
}
