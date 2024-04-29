using Terraria.ModLoader;
using Terraria;
using terraguardians;

namespace gaomonmod1dot4.Items
{
    public class HecateBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Seems to invoke something.");
        }

        private bool IsItemUsable(Player player)
        {
            return player.whoAmI == Main.myPlayer
                && !terraguardians.PlayerMod.PlayerHasCompanion(player, FollowersContainer.Hecate, this.Mod.Name)  // Player doesn't have Gaomon in their companion list
                && !terraguardians.WorldMod.HasCompanionNPCSpawned(FollowersContainer.Hecate, this.Mod.Name) // Glory is not a companion npc in the world.
                && !terraguardians.WorldMod.HasMetCompanion(FollowersContainer.Hecate, this.Mod.Name); // Glory has never been met in this world.
        }

        /// <summary>
        /// Spawn Glory at the position of the player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool CanUseItem(Player player)
        {
            if (IsItemUsable(player))
            {
                // Spawn Glory
                terraguardians.Companion tg = terraguardians.WorldMod.SpawnCompanionNPC(player.Bottom, FollowersContainer.Hecate, Mod.Name);

                // Create some nice dust particles
                for (int d = 0; d < 20; d++)
                {
                    Dust.NewDust(tg.Center, tg.width, tg.height, Terraria.ID.DustID.Electric);
                }

                Main.NewText("Glory appears!");
                Item.SetDefaults(0);
                return true;
            }
            Main.NewText("There is already a Hecate.");
            return false;
        }

        public override void SetDefaults()
        {
            Item.UseSound = Terraria.ID.SoundID.Item4;
            Item.useStyle = 4;
            Item.useTurn = false;
            Item.useAnimation = 5;
            Item.useTime = 5;
            Item.maxStack = 1;
            Item.consumable = true;
            Item.value = Terraria.Item.buyPrice(0, 1);
            Item.width = 22;
            Item.height = 22;
            Item.rare = 2;
            Item.value = 1;
        }
    }
}
