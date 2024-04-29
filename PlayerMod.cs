using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader;
using gaomonmod1dot4.Items;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace gaomonmod1dot4
{
	public class PlayerMod : ModPlayer
	{
		public delegate void OnPlayerHurtHandler(Player player, Player.HurtInfo info);

		public static event OnPlayerHurtHandler PlayerHurt;

		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) {
			return new[] {
				new Item(ModContent.ItemType<GloryBag>(), 1),
			};
		}

        public override void OnHurt(Player.HurtInfo info)
        {
			PlayerHurt.Invoke(this.Player, info);
            base.OnHurt(info);
        }
    }
}
