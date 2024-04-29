using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System.Collections.Generic;
using terraguardians;
using Terraria.Graphics;
using System.Linq;
using System;
using System.Configuration;
using Terraria.Utilities;
using Terraria.DataStructures;
using gaomonmod1dot4.Companions.Hecate;

namespace gaomonmod1dot4.Companion.Hecate
{
    public class HecateBase : TerrarianBase
    {
        public override CompanionGroup GetCompanionGroup => MainMod.followersGroup; // Assigning the Digimon group to the companion. You can take TerraGuardian groups from the MainMod script of TerraGuardian.
        private static Random rng = new Random();

        public HecateBase()
        {
            NpcMod.NpcSpawned += OnBossStart;
        }

        // DESCRIPTION - Name, and info that shows up in the bestiary
        #region Description
        public override string Name => "Hecate";
        public override string Description => """
A young, promising mage who keeps to herself.
""";
        public override int Age => 19;
        public override Genders Gender => Genders.Female;
        public override BirthdayCalculator SetBirthday => new BirthdayCalculator(Seasons.Summer, 15);
        #endregion

        // STATS - HP, Mana, Speed, scaling, etc.
        #region Stats
        public override int InitialMaxHealth => 120; // Glory is more tanky.
        public override int InitialMaxMana => 200;
        public override int ManaPerManaCrystal => 30; // Glory is incompetent at magic
        public override float AccuracyPercent => 0.95f; // Glory is decent with ranged weaponry
        public override float AgilityPercent => base.AgilityPercent; // Glory has average Terrarian speed
        public override CombatTactics DefaultCombatTactic => CombatTactics.LongRange;

        #endregion

        // FRIENDSHIP - When to move in, be a buddy, visit, etc.
        #region Friendship
        public override bool CanBeAppointedAsBuddy => false;
        protected override FriendshipLevelUnlocks SetFriendshipUnlocks => new FriendshipLevelUnlocks()
        {
            FollowerUnlock = 0,
            InviteUnlock = 0,
            VisitUnlock = 1,
            MoveInUnlock = 3,
            MountUnlock = 5,
            ControlUnlock = 10,
            RequestUnlock = 0,
            BuddyUnlock = 30
        };
        #endregion

        // APPEARANCE - How they look
        #region Appearance
        protected override TerrarianCompanionInfo SetTerrarianCompanionInfo
        {
            get
            {
                return new TerrarianCompanionInfo()
                {
                    HairStyle = 5,
                    SkinVariant = 7,
                    HairColor = new Color(102, 47, 223),
                    EyeColor = new Color(102, 47, 223),
                    SkinColor = new Color(225, 190, 173),
                    ShirtColor = new Color(70, 64, 49),
                    UndershirtColor = new Color(255, 255, 255),
                    PantsColor = new Color(255, 255, 255),
                    ShoesColor = new Color(70, 64, 49)
                };
            }
        }
        #endregion

        // INVENTORY - Initial inventory
        #region Inventory
        public override void InitialInventory(out InitialItemDefinition[] InitialInventoryItems, ref InitialItemDefinition[] InitialEquipments)
        {
            InitialInventoryItems = new InitialItemDefinition[]
            {
                new InitialItemDefinition(ItemID.CopperShortsword),
                new InitialItemDefinition(ItemID.AmethystStaff),
                new InitialItemDefinition(ItemID.SlimeStaff),
                new InitialItemDefinition(ItemID.HealingPotion, 10),
                new InitialItemDefinition(ItemID.ManaPotion, 10),
                new InitialItemDefinition(ItemID.FamiliarShirt, 1),
                new InitialItemDefinition(ItemID.FamiliarPants, 1),
                new InitialItemDefinition(ItemID.FamiliarWig, 1),
            };
        }
        #endregion

        // DIALOGUE - All dialogue, including situational, periodic, on hit, etc.
        #region Dialogue
        protected override CompanionDialogueContainer GetDialogueContainer => new HecateDialogues();
        private static double OnAttackDialogueChance = 0.10;
        private static double OnOtherCompanionDeathDialogueChance = 0.50;
        private static double OnDeathDialogueChance = 0.30;
        private static double OnGenericNPCDefeatDialogueChance = 0.08;

        private static int GenericNPCDefeatWaitTime = 360; // 60 = 1sec because we do 60FPS updates
        private int GenericNpcDeathDialogueTimer = 0;

        private static int RandomDialogueWaitTimeMin = 60 * 180;
        private static int RandomDialogueWaitTimeMax = 60 * 360;
        private int RandomDialogueTimer = 0;

        // Hurt Dialogue
        public override void OnAttackedByNpc(terraguardians.Companion companion, NPC attacker, int Damage, bool Critical)
        {
            // Should we generate dialogue?
            if (rng.NextDouble() <= 1.0 - OnAttackDialogueChance) { return; }

            // Get dialogue randomly
            float currHealthPercentage = (float)companion.Health / (float)companion.MaxHealth;
            List<string> texts = new List<string>();
            if (currHealthPercentage > 0.7)
            {
                texts.Add("Ow...");
                texts.Add("That hurts...");
            }
            else if (currHealthPercentage > 0.4)
            {
                texts.Add("Ow...");
                texts.Add("Stop...");
                texts.Add("No...");
            }
            else
            {
                texts.Add("It's getting dire...");
                texts.Add("I need assistance...");
                texts.Add("Kindly refrain...");
                texts.Add("This is getting bad...");
                texts.Add("Ah...");
            }
            string text = texts[rng.Next(texts.Count)];

            // Display dialogue in chat and as a popoup
            companion.SaySomething(text, true);
        }

        public override void OnCompanionDeath(terraguardians.Companion companion, terraguardians.Companion target)
        {
            if (rng.NextDouble() <= 1.0 - OnOtherCompanionDeathDialogueChance) { return; }
            List<string> texts = new List<string>
            {
                $"We... we lost {target.name}...",
                $"{target.name} is down..."
            };
            string text = texts[rng.Next(texts.Count)];

            companion.SaySomething(text, true);
        }

        public override void OnPlayerDeath(terraguardians.Companion companion, Player player)
        {
            List<string> texts = new List<string>
            {
                "No, [nickname]!",
                "Stay with us, [nickname]!",
                $"[nickname], I won't let this be the end!"
            };
            string text = texts[rng.Next(texts.Count)];
            companion.SaySomething(text, true);
            base.OnPlayerDeath(companion, player);
        }

        public override void OnDeath(terraguardians.Companion companion)
        {
            if (rng.NextDouble() <= 1.0 - OnDeathDialogueChance) { return; }
            List<string> texts = new List<string>
            {
                "I guess this is it...",
                "I still want to learn more...",
                "Forgive me, everyone...",
                "Forgive me, everyone...",
            };
            string text = texts[rng.Next(texts.Count)];
            companion.SaySomething(text, true);
            base.OnDeath(companion);
        }

        public override void OnNpcDeath(terraguardians.Companion companion, NPC npc)
        {
            if (!companion.IsFollower) { return; }
            // CRITTERS
            switch (npc.type)
            {
                case NPCID.Goldfish:
                case NPCID.Bunny:
                case NPCID.Bird:
                case NPCID.Duck:
                case NPCID.Duck2:
                case NPCID.DuckWhite:
                case NPCID.DuckWhite2:
                    return;
            }

            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    companion.SaySomething("We did well. That was a test of our resolve.", true); return;
                case NPCID.KingSlime:
                    companion.SaySomething("Such a messy ordeal...", true); return;
                case NPCID.EaterofWorldsHead:
                    companion.SaySomething("We've restored a bit more balance today.", true); return;
                case NPCID.BrainofCthulhu:
                    companion.SaySomething("Well done everyone.", true); return;
                case NPCID.QueenBee:
                    companion.SaySomething("Certainly a tough opponent, but manageable.", true); return;
                case NPCID.SkeletronHead:
                    companion.SaySomething("Womp womp.", true); return;
                case NPCID.Deerclops:
                    companion.SaySomething("Such an odd creature... I hope it finds peace.", true); return;
                case NPCID.WallofFlesh:
                    companion.SaySomething("The world has changed... Can you feel it?", true); return;
                case NPCID.WyvernHead:
                    companion.SaySomething("Its grace in the sky was almost enchanting...", true); return;
                case NPCID.Retinazer:
                    companion.SaySomething("We have ended their watch. Well done, [nickname].", true); return;
                case NPCID.QueenSlimeBoss:
                    companion.SaySomething("That was... unexpectedly sticky.", true); return;
                case NPCID.HallowBoss:
                    companion.SaySomething("It’s sad, yet necessary to maintain balance.", true); return;
                case NPCID.TheDestroyer:
                    companion.SaySomething("Together, we've overcome great adversity, [nickname].", true); return;
                case NPCID.SkeletronPrime:
                    companion.SaySomething("A commendable effort, [nickname].", true); return;
                case NPCID.Plantera:
                    companion.SaySomething("That was a challenging trial...", true); return;
                case NPCID.Golem:
                    companion.SaySomething("Victory, at last! We did it together.", true); return;
                case NPCID.DukeFishron:
                    companion.SaySomething("Such a peculiar battle... I need a moment.", true); return;
                case NPCID.CultistBoss:
                    companion.SaySomething("Finally, it's over. Let's move forward.", true); return;
                case NPCID.MoonLordHead:
                    companion.SaySomething("We've achieved the impossible!", true); return;
            }
            if (GenericNpcDeathDialogueTimer > 0) { return; } // Don't speak if we recently spoke
            if (npc.boss)
            {
                companion.SaySomething("Great job [nickname]! That was a powerful foe.", true);
                GenericNpcDeathDialogueTimer = GenericNPCDefeatWaitTime;
                return;
            }

            // SPECIAL NPC DEATH
            if (companion.KnockoutStates > KnockoutStates.Awake || companion.Health < 1) { return; } // Don't speak if unconcious
            bool playerKilled = MainMod.GetLocalPlayer == Main.player[Main.myPlayer];

            switch (npc.type)
            {
                case NPCID.Pinky:
                    {
                        List<string> msgs = new List<string>
                        {
                            "Wow, that slime dropped a lot of coins!",
                            "For something tiny it was kind of tough..."
                        };
                        string msg = msgs[rng.Next(msgs.Count)];
                        companion.SaySomething(msg, true);
                        GenericNpcDeathDialogueTimer = GenericNPCDefeatWaitTime;
                        return;
                    }
                case NPCID.Tim:
                    companion.SaySomething("What a funny looking skeleton...", true);
                    GenericNpcDeathDialogueTimer = GenericNPCDefeatWaitTime;
                    return;
                case NPCID.TheBride:
                    companion.SaySomething("She had a really pretty dress...", true);
                    GenericNpcDeathDialogueTimer = GenericNPCDefeatWaitTime;
                    return;
                case NPCID.TheGroom:
                    companion.SaySomething("That one had a fancy suit on.", true);
                    GenericNpcDeathDialogueTimer = GenericNPCDefeatWaitTime;
                    return;
                case NPCID.Nymph:
                    {
                        List<string> msgs = new List<string>
                        {
                            "What a dangerous creature...",
                            "That creature tried to trick us!"
                        };
                        string msg = msgs[rng.Next(msgs.Count)];
                        companion.SaySomething(msg, true);
                        GenericNpcDeathDialogueTimer = GenericNPCDefeatWaitTime;
                        return;
                    }
            }

            // GENERIC NPC DEATH
            if (rng.NextDouble() <= 1.0 - OnGenericNPCDefeatDialogueChance) { return; } // Randomly speak
            GenericNpcDeathDialogueTimer = GenericNPCDefeatWaitTime;
            List<string> texts = new List<string>
            {
                "One less to worry about...",
                "We must keep going...",
                "That was too close..."
            };
            float currHealthPercentage = (float)companion.Health / (float)companion.MaxHealth;
            if (currHealthPercentage < 0.4)
            {
                texts.Add("That was scary...");
                texts.Add("that was much too close...");
            }
            switch (npc.type)
            {
                case NPCID.Zombie:
                    texts.Add("Rest now, no more wandering...");
                    break;
                case NPCID.DemonEye:
                    texts.Add("Such intense gazes, now put to rest.");
                    break;

                // Blood moon
                case NPCID.BloodSquid:
                case NPCID.Drippler:
                case NPCID.BloodZombie:
                    texts.Add("This blood moon brings such turmoil...");
                    break;

                // Corruption
                case NPCID.EaterofSouls:
                    texts.Add("It's dead.");
                    break;
                case NPCID.Corruptor:
                    texts.Add("It's been dispelled.");
                    break;
                case NPCID.Wraith:
                    texts.Add("Wandering spirit, may you find your way...");
                    break;

                // Underworld
                case NPCID.FireImp:
                    texts.Add("I got it.");
                    break;

                // Crimson
                case NPCID.FaceMonster:
                case NPCID.Crimera:
                case NPCID.IchorSticker:
                    texts.Add("So terrifying...");
                    break;

                // Hallow
                case NPCID.EnchantedSword:
                    texts.Add("How do  they hold so much energy?");
                    break;
                case NPCID.Gastropod:
                    texts.Add("I wish I could study these ones...");
                    break;
                case NPCID.Unicorn:
                    texts.Add("Magnificent creatures, unicorns...");
                    break;

                // Sky
                case NPCID.Harpy:
                    texts.Add("These ones frighten me...");
                    break;

                // Others
                case NPCID.Ghost:
                    texts.Add("Rest in peace, wayward spirit...");
                    break;
                case NPCID.BlueSlime:
                case NPCID.GreenSlime:
                case NPCID.DungeonSlime:
                case NPCID.MotherSlime:
                case NPCID.BlackSlime:
                case NPCID.IceSlime:
                    texts.Add("I think have gel in my hair.");
                    break;
                case NPCID.WallCreeper:
                case NPCID.BloodCrawler:
                    texts.Add("I hate these things...");
                    break;

                // Goblin Army
                case NPCID.GoblinArcher:
                case NPCID.GoblinPeon:
                case NPCID.GoblinScout:
                case NPCID.GoblinSorcerer:
                case NPCID.GoblinThief:
                    texts.Add("There are a lot of them.");
                    break;
            }

            string text = texts[rng.Next(texts.Count)];
            companion.SaySomething(text, true);
            base.OnNpcDeath(companion, npc);
        }

        public override void UpdateCompanion(terraguardians.Companion companion)
        {
            if (!companion.IsFollower) { return; }
            if (GenericNpcDeathDialogueTimer > 0) { GenericNpcDeathDialogueTimer--; }
            if (RandomDialogueTimer > 0) { RandomDialogueTimer--; }
            OnBiomePrompt(companion);
            OnRandomDialogue(companion);
        }

        private Player Player => MainMod.GetLocalPlayer;
        //Biomes
        public bool seenDesertBiome;
        public bool seenSandstorm;
        public bool seenSnowBiome;
        public bool seenSnow;
        public bool seenCorruptionBiome;
        public bool seenCrimsonBiome;
        public bool seenJungleBiome;
        public bool seenDungeonBiome;
        public bool seenGlowingMushroomBiome;
        public bool seenBeachBiome;
        public bool seenMeteoriteBiome;
        public bool seenHallowBiome;
        public bool seenSpaceBiome;
        public bool seenUnderworldBiome;
        public void OnBiomePrompt(terraguardians.Companion companion)
        {
            if (Player.ZoneBeach && !seenBeachBiome)
            {
                companion.SaySomething("Wow, the ocean... It's so beautiful!", true);
                seenBeachBiome = true;
            }
            if (Player.ZoneSnow && !seenSnowBiome)
            {
                companion.SaySomething("It's cold... if anything can survive this extreme weather, it must be tougher than normal.", true);
                seenSnowBiome = true;
            }
            if (Player.ZoneSnow && Player.ZoneRain && !seenSnow)
            {
                companion.SaySomething("A blizzard. Try and keep warm, [nickname].", true);
                seenSnow = true;
            }
            if (Player.ZoneCorrupt && !seenCorruptionBiome)
            {
                companion.SaySomething("I can feel the evil aura here... stay close to me, [nickname], we're being watched.", true);
                seenCorruptionBiome = true;
            }
            if (Player.ZoneCrimson && !seenCrimsonBiome)
            {
                companion.SaySomething("Gods that smell... is this... flesh...? Don't get separated from me, [nickname].", true);
                seenCrimsonBiome = true;
            }
            if (Player.ZoneDesert && !seenDesertBiome)
            {
                companion.SaySomething("Wow it's hot... I should have brought sunglasses.", true);
                seenDesertBiome = true;
            }
            if (Player.ZoneSandstorm && !seenSandstorm)
            {
                companion.SaySomething("A sandstorm! Try not to get any in your eyes.", true);
                seenSandstorm = true;
            }
            if (Player.ZoneJungle && !seenJungleBiome)
            {
                companion.SaySomething("It's the Jungle. Watch out, [nickname], I've heard this place is really dangerous.", true);
                seenJungleBiome = true;
            }
            if (Player.ZoneGlowshroom && !seenGlowingMushroomBiome)
            {
                companion.SaySomething("Are those... glowing mushrooms? It's beautiful...", true);
                seenGlowingMushroomBiome = true;
            }
            if (Player.ZoneMeteor && !seenMeteoriteBiome)
            {
                companion.SaySomething("Looks like you could make something useful out of this hunk of space rock.", true);
                seenMeteoriteBiome = true;
            }
            if (Player.ZoneUnderworldHeight && !seenUnderworldBiome)
            {
                companion.SaySomething("So this is where the souls of the dead go to rest... It's horrible...", true);
                seenUnderworldBiome = true;
            }
            if (Player.ZoneHallow && !seenHallowBiome)
            {
                companion.SaySomething("This place looks so magical! The inhabitants don't look too happy to see us though.", true);
                seenHallowBiome = true;
            }
            if (Player.ZoneSkyHeight && !seenSpaceBiome)
            {
                List<string> texts = new List<string>
                {
                    "The stars are really beautiful, aren't they?",
                    "It's like I can almost touch the clouds uyp here!",
                    "I hope I don't slip and fall..."
                };
                string text = texts[rng.Next(texts.Count)];
                companion.SaySomething(text, true);
                seenSpaceBiome = true;
            }
            if (Player.ZoneDungeon && !seenDungeonBiome)
            {
                companion.SaySomething("I've heard the Dungeon holds powerful grimoires and swords... as well as challenging traps and foes. Stay vigilant!", true);
                seenDungeonBiome = true;
            }
        }

        /// <summary>
        /// Random dialogue every now and then!
        /// </summary>
        /// <param name="companion"></param>
        public void OnRandomDialogue(terraguardians.Companion companion)
        {
            if (RandomDialogueTimer > 0) { return; } // need to wait for timer to finish
            if (companion.TargettingSomething) { RandomDialogueTimer += 60 * 10; return; } // not when fighting
            RandomDialogueTimer = rng.Next(RandomDialogueWaitTimeMin, RandomDialogueWaitTimeMax);

            List<string> msgs = new List<string>();
            msgs.Add("Wish I had a book to read right now...");
            msgs.Add("I wonder what spell I should learn next?");
            msgs.Add("*Sneezes*");
            msgs.Add("*Yawns*");
            msgs.Add("*Hums*");
            msgs.Add("Mm... cake would be nice...");
            msgs.Add("Do you have any sweets?");

            // Friendship Messages
            if (companion.FriendshipLevel > 5)
            {
                msgs.Add("You're pretty strong, [nickname].");
                msgs.Add("I think I've gotten stronger since we first started hanging out, [nickname].");
            }
            if (companion.FriendshipLevel > 10)
            {
                msgs.Add("Fighting monsters ain't so bad when you're around, [nickname].");
            }

            // Environmental Messages
            if (Main.bloodMoon)
            {
                msgs.Add("There are a lot of enemies around.");
                msgs.Add("Keep your guard up.");
                msgs.Add("I hate nights like these.");
            }
            else
            {
                int CompanionsCount = WorldMod.GetCompanionsCount;
                bool HasCompanions = CompanionsCount > 0;
                if (Main.dayTime)
                {
                    if (Main.eclipse)
                    {
                        msgs.Add("The eclipse is oddly beautiful.");
                        msgs.Add("It's like the world has turned upside down.");
                        msgs.Add("Stay vigilant.");
                    }
                    else
                    {
                        msgs.Add("It's nice being out in the Sun.");
                    }
                }
                else
                {
                    msgs.Add("*Yawns*");
                    msgs.Add("*Stretches*");
                }
                if (Main.raining)
                {
                    msgs.Add("I've always liked the rain!");
                    msgs.Add("Rain is always relaxing.");
                }
            }

            // Situational Messages
            Player player = MainMod.GetLocalPlayer;
            if (player.statLife < player.statLifeMax2 * 0.4) // Player is at 25% health or less
            {
                msgs.Add("You're really beat up, maybe we should hang back a bit.");
                msgs.Add("Hey, take a potion before you faint!");
            }

            // Number of enemies

            // Biome Messages
            if (Player.ZoneBeach)
            {
                msgs.Add("Uh, so much sand...");
                msgs.Add("I think I have sand in my shoes.");
            }
            if (Player.ZoneSnow)
            {
                msgs.Add("*Sneezes*");
                msgs.Add("So cold!");
            }
            if (Player.ZoneSnow && Player.ZoneRain)
            {
                msgs.Add("I can't read in a place like this...");
                msgs.Add("I can't see!");
            }
            if (Player.ZoneCorrupt)
            {
                msgs.Add("I don't like this place.");
                msgs.Add("There's strong magic here.");
            }
            if (Player.ZoneCrimson)
            {
                msgs.Add("That smell...");
                msgs.Add("I hate this place.");
            }
            if (Player.ZoneDesert)
            {
                msgs.Add("It's hot.");
                msgs.Add("I think there's sand in my shoes.");
            }
            if (Player.ZoneSandstorm)
            {
                msgs.Add("I can barely see in here.");
            }
            if (Player.ZoneJungle)
            {
                msgs.Add("Ow, I think I got bit.");
            }
            if (Player.ZoneGlowshroom)
            {
                msgs.Add("I wonder what potions I could make with glowshrooms...");
            }
            if (Player.ZoneMeteor)
            {
                msgs.Add("This strange rock has some magic in it...");
            }
            if (Player.ZoneUnderworldHeight)
            {
                msgs.Add("The magic here is very strong.");
                msgs.Add("Demonic magic is powerful, I wish to study it.");
            }
            if (Player.ZoneHallow)
            {
                msgs.Add("There's magic in almost everything here...");
                msgs.Add("How pretty.");
            }
            if (Player.ZoneDungeon)
            {
                msgs.Add("I wonder what grimoires are here?");
                msgs.Add("There's bound to be a spell book somewhere.");
                msgs.Add("I wouldn't mind staying here for a while.");
                seenDungeonBiome = true;
            }
            if (Player.ZoneSkyHeight)
            {
                msgs.Add("I can see the stars.");
            }
            string msg = msgs[rng.Next(msgs.Count)];
            companion.SaySomething(msg, true);

            rng.Next(RandomDialogueWaitTimeMin, RandomDialogueWaitTimeMax + 1); // Choose random time for next dialogue
        }

        //Bosses
        public bool seenEyeOfCthulhu;
        public bool seenKingSlime;
        public bool seenEaterOfWorlds;
        public bool seenBrainOfCthulhu;
        public bool seenQueenBee;
        public bool seenSkeletron;
        public bool seenDeerclops;
        public bool seenWallOfFlesh;
        //Post hardmode
        public bool seenQueenSlime;
        public bool seenWyvern;
        public bool seenTwins;
        public bool seenDestroyer;
        public bool seenSkeletronPrime;
        public bool seenPlantera;
        public bool seenGolem;
        public bool seenDukeFishron;
        public bool seenCultist;
        public bool seenEmpress;
        public bool seenMoonLord;

        public void OnBossStart(NPC spawnedNpc, IEntitySource source)
        {
            if (!spawnedNpc.boss) { return; }
            terraguardians.Companion companion = terraguardians.MainMod.GetActiveCompanions.FirstOrDefault(c => c.Base.Equals(this));
            if (companion == null) { return; }
            if (!companion.IsFollower) { return; }
            if (companion.KnockoutStates > KnockoutStates.Awake) { return; } // Don't speak if unconcious

            switch (spawnedNpc.type)
            {
                case NPCID.EyeofCthulhu:
                    if (seenEyeOfCthulhu) { return; }
                    companion.SaySomething("It can't be... Be careful! I have your back!", true);
                    seenEyeOfCthulhu = true;
                    break;
                case NPCID.KingSlime:
                    if (seenKingSlime) { return; }
                    companion.SaySomething("A giant slime!\n Surely it can't be that hard to kill... right?", true);
                    seenKingSlime = true;
                    break;
                case NPCID.EaterofWorldsHead:
                    if (seenEaterOfWorlds) { return; }
                    companion.SaySomething("This world is not for yours to take, worm!", true);
                    seenEaterOfWorlds = true;
                    break;
                case NPCID.BrainofCthulhu:
                    if (seenBrainOfCthulhu) { return; }
                    companion.SaySomething("Augh, it's getting into my head...! Let's end this thing!", true);
                    seenBrainOfCthulhu = true;
                    break;
                case NPCID.QueenBee:
                    if (seenQueenBee) { return; }
                    companion.SaySomething("It's fast, keep your eyes peeled for its dashes!", true);
                    seenQueenBee = true;
                    break;
                case NPCID.SkeletronHead:
                    if (seenSkeletron) { return; }
                    companion.SaySomething("He looks powerful, stay away from his limbs!", true);
                    seenSkeletron = true;
                    break;
                case NPCID.Deerclops:
                    if (seenDeerclops) { return; }
                    companion.SaySomething("What the heck is that?! Be on your guard!", true);
                    seenDeerclops = true;
                    break;
                case NPCID.WallofFlesh:
                    if (seenWallOfFlesh) { return; }
                    companion.SaySomething("So this is it... Keep running, and let's give it a fight to remember!", true);
                    seenWallOfFlesh = true;
                    break;
                case NPCID.WyvernHead:
                    if (seenWyvern) { return; }
                    companion.SaySomething("Uh... do you see that thing in the sky?", true);
                    seenWyvern = true;
                    break;
                case NPCID.Retinazer:
                    if (seenTwins) { return; }
                    companion.SaySomething("How is this possible...\nthey're just like the Eye... except with more lasers!", true);
                    seenTwins = true;
                    break;
                case NPCID.QueenSlimeBoss:
                    if (seenQueenSlime) { return; }
                    companion.SaySomething("This one kind of looks... tasty?", true);
                    seenQueenSlime = true;
                    break;
                case NPCID.HallowBoss:
                    if (seenEmpress) { return; }
                    companion.SaySomething("Maybe we shouldn't have touched that butterfly...", true);
                    seenEmpress = true;
                    break;
                case NPCID.TheDestroyer:
                    if (seenDestroyer) { return; }
                    companion.SaySomething("The Eater of Worlds... resurrected? We must put this abomination down!", true);
                    seenDestroyer = true;
                    break;
                case NPCID.SkeletronPrime:
                    if (seenSkeletronPrime) { return; }
                    companion.SaySomething("He's much more powerful than before... I've got your back!", true);
                    seenSkeletronPrime = true;
                    break;
                case NPCID.Plantera:
                    if (seenPlantera) { return; }
                    companion.SaySomething("Watch out for its tendrils! We'll need a big arena to fight this one.", true);
                    seenPlantera = true;
                    break;
                case NPCID.Golem:
                    if (seenGolem) { return; }
                    companion.SaySomething("I've heard legends of the Ancient Golem... He's seems slow, but keep your guard up!", true);
                    seenGolem = true;
                    break;
                case NPCID.DukeFishron:
                    if (seenDukeFishron) { return; }
                    List<string> texts = new List<string>
                    {
                        "Uh, I think you reeled in the wrong fish!",
                        "That's not a fish I want to eat!",
                        "I never really liked seafood..."
                    };
                    string text = texts[rng.Next(texts.Count)];
                    companion.SaySomething(text, true);
                    seenDukeFishron = true;
                    break;
                case NPCID.CultistBoss:
                    if (seenCultist) { return; }
                    companion.SaySomething("These cultists are a scourge on this world... I won't let them live!", true);
                    seenDukeFishron = true;
                    break;
                case NPCID.MoonLordHead:
                    if (seenMoonLord) { return; }
                    companion.SaySomething("This is it... No matter what happens, we need to beat him.", true);
                    seenMoonLord = true;
                    break;
            }
            #endregion
        }
    }
}
