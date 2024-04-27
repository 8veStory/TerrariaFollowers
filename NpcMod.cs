using Terraria.ModLoader;
using terraguardians;
using Terraria;
using Terraria.DataStructures;

namespace gaomonmod1dot4
{
	public class NpcMod : GlobalNPC
	{
        // Define a delegate for NPC spawn events
        public delegate void NpcSpawnEventHandler(NPC npc, IEntitySource source);

        // Define the event based on the delegate
        public static event NpcSpawnEventHandler NpcSpawned;

        public override bool PreAI(NPC npc)
        {
            if (npc.type == Terraria.ID.NPCID.BlueSlime && npc.ai[1] == 0)
            {
                int NearestPlayer = npc.FindClosestPlayer();
                // Makes so if Gaomon haven't been met, and neither there's its companion npc in the world, the Digivice can spawn in a slime.
                if (NearestPlayer > -1 && !terraguardians.WorldMod.HasMetCompanion(FollowersContainer.Glory, Mod.Name) && !terraguardians.WorldMod.HasCompanionNPCSpawned(FollowersContainer.Glory, Mod.Name) && Main.rand.Next(10) == 0)
                {
                    npc.ai[1] = ModContent.ItemType<Items.BlueDigivice>();
                    npc.netUpdate = true;
                }
            }
            return base.PreAI(npc);
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            // Invoke the event when an NPC spawns
            NpcSpawned?.Invoke(npc, source);
        }
    }
}