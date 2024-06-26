using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using System.Collections.Generic;
using terraguardians;
using gaomonmod1dot4.Companions.Glory;
using Terraria.Graphics;
using System.Linq;
using System;
using System.Configuration;
using Terraria.Utilities;
using Terraria.DataStructures;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace gaomonmod1dot4.Companion.Glory
{
    public class GloryBase : TerrarianBase
    {
        public override CompanionGroup GetCompanionGroup => MainMod.followersGroup; // Assigning the Digimon group to the companion. You can take TerraGuardian groups from the MainMod script of TerraGuardian.
        private static Random rng = new Random();

        public GloryBase()
        {
            NpcMod.NpcSpawned += OnBossStart;
            PlayerMod.PlayerHurt += OnPlayerHurt;
        }

        private terraguardians.Companion _companion = null;
        private terraguardians.Companion Companion
        {
            get
            {
                if (_companion == null)
                {
                    _companion = terraguardians.MainMod.GetActiveCompanions.FirstOrDefault(c => c.Base.Equals(this));
                }
                return _companion;
            }
        }

        public override void UpdateCompanion(terraguardians.Companion companion)
        {
            if (!companion.IsFollower) { return; }
            if (GenericNpcDeathDialogueTimer > 0) { GenericNpcDeathDialogueTimer--; }
            if (RandomDialogueTimer > 0) { RandomDialogueTimer--; }
            if (OnPlayerHitDialogueTimer > 0) { OnPlayerHitDialogueTimer--; }
            OnBiomePrompt(companion);
            OnRandomDialogue(companion);
        }

        // DESCRIPTION - Name, and info that shows up in the bestiary
        #region Description
        public override string Name => "Glory";
        public override string Description => """
An orphaned warrior in training who excels at close quarters combat and tanking. She stalks the land to put an end to the Lunar Cults' world-ending plans.
""";
        public override int Age => 18;
        public override Genders Gender => Genders.Female;
        public override BirthdayCalculator SetBirthday => new BirthdayCalculator(Seasons.Winter, 19);
        #endregion

        // STATS - HP, Mana, Speed, scaling, etc.
        #region Stats
        public override int InitialMaxHealth => 160; // Glory is more tanky.
        public override int ManaPerManaCrystal => 10; // Glory is incompetent at magic
        public override float AccuracyPercent => 0.70f; // Glory is decent with ranged weaponry
        public override float AgilityPercent => base.AgilityPercent; // Glory has average Terrarian speed

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
                    HairStyle = 9,
                    SkinVariant = 5,
                    HairColor = new Color(139, 69, 19),
                    EyeColor = new Color(47, 217, 64),
                    SkinColor = new Color(229, 189, 35),
                    ShirtColor = new Color(184, 61, 29),
                    UndershirtColor = new Color(169, 185, 211),
                    PantsColor = new Color(119, 82, 0),
                    ShoesColor = new Color(160, 105, 60)
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
                new InitialItemDefinition(ItemID.CopperBroadsword),
                new InitialItemDefinition(ItemID.WoodenBow),
                new InitialItemDefinition(ItemID.WoodenArrow, 85),
                new InitialItemDefinition(ItemID.HealingPotion, 15)
            };
        }
        #endregion

        // DIALOGUE - All dialogue, including situational, periodic, on hit, etc.
        #region Dialogue
        protected override CompanionDialogueContainer GetDialogueContainer => new GloryDialogues();
        private const double OnHurtByNpcDialogueChance = 0.13;
        private const double OnOtherCompanionDeathDialogueChance = 1.00;
        private const double OnDeathDialogueChance = 1.00;


        private const int OnPlayerHitDialogueWaitTime = 20 * 60;
        private int OnPlayerHitDialogueTimer = 0;
        private const double OnPlayerHurtDialogueChance = 0.75;

        private const int GenericNPCDefeatWaitTime = 360; // 60 = 1sec because we do 60FPS updates
        private int GenericNpcDeathDialogueTimer = 0;


        private const int RandomDialogueWaitTimeMin = 180 * 60;
        private const int RandomDialogueWaitTimeMax = 360 * 60;
        private const double OnGenericNPCDefeatDialogueChance = 0.08;
        private int RandomDialogueTimer = 0;

        // Hurt Dialogue
        // public override void OnAttackedByNpc(terraguardians.Companion companion, NPC attacker, int Damage, bool Critical)
        // {
        //     Main.NewText($"nani1");
        //     List<string> texts = new List<string>();
        //     string text;

        //     // DEATH
        //     Main.NewText($"Potential Killing Blow: {companion.Health}");
        //     Main.NewText($"After the fact: {companion.Health - Damage}");
        //     bool isLethal = (companion.Health - Damage) <= 0;
        //     if (isLethal)
        //     {
        //         if (rng.NextDouble() <= 1.0 - OnDeathDialogueChance) { return; }
        //         texts.Add("No, I can still...");
        //         texts.Add("No... Not now...");
        //         texts.Add("Damn it...");
        //         texts.Add("It's dark...");
        //         text = texts[rng.Next(texts.Count)];
        //         companion.SaySomething(text, companion.IsFollower);
        //         return;
        //     }

        //     // Should we generate dialogue?
        //     if (rng.NextDouble() <= 1.0 - OnAttackDialogueChance) { return; }

        //     // Get dialogue randomly
        //     float currHealthPercentage = (float)companion.Health / (float)companion.MaxHealth;
        //     if (currHealthPercentage > 0.7)
        //     {
        //         texts.Add("Ow!");
        //         texts.Add("Ouch!");
        //         texts.Add("Come at me!");
        //         texts.Add("You'll have to hit harder than that!");
        //     }
        //     else if (currHealthPercentage > 0.4)
        //     {
        //         texts.Add("Ow!");
        //         texts.Add("Come at me!");
        //         texts.Add("You'll pay for that!");
        //     }
        //     else
        //     {
        //         texts.Add("I'm wounded...");
        //         texts.Add("I'm fading...");
        //         texts.Add("Augh!");
        //         texts.Add("Damn it!");
        //     }
        //     text = texts[rng.Next(texts.Count)];

        //     // Display dialogue in chat and as a popoup
        //     companion.SaySomething(text, companion.IsFollower);
        // }

        public override void OnCompanionDeath(terraguardians.Companion companion, terraguardians.Companion target)
        {
            if (rng.NextDouble() <= 1.0 - OnOtherCompanionDeathDialogueChance) { return; }
            List<string> texts = new List<string>
            {
                $"No, {target.name} is down!",
                $"Damn it, {target.name} is down!"
            };
            string text = texts[rng.Next(texts.Count)];

            companion.SaySomething(text, true);
        }

        public void OnPlayerHurt(Player player, Player.HurtInfo hurtInfo)
        {
            bool playerIsAttacked = player.whoAmI == Main.myPlayer;
            if (!playerIsAttacked) { 
                if (Companion.whoAmI == player.whoAmI) {
                    Main.NewText("It's GLORY!");
                    OnCompanionAttacked(hurtInfo.Damage);
                } else {
                    Main.NewText("IT's ANOHTER COMPANIONA;");
                }
                return;
            }
            Main.NewText("It's PLAYER!");

            // DEATH MESSAGES
            List<string> texts = new List<string>();
            string text;

            bool isLethal = (player.statLife - hurtInfo.Damage) <= 0;
            if (isLethal)
            {
                texts.Add("No, [nickname]!");
                texts.Add("Don't die on me, [nickname]!");
                texts.Add("Stay with me, [nickname]!");
                texts.Add("You... I'll kill you all!");
                text = texts[rng.Next(texts.Count)];
                Companion.SaySomething(text, Companion.IsFollower);
                return;
            }

            // GENERIC HIT MESSAGES
            Main.NewText("LOL");
            if (rng.NextDouble() <= 1.0 - OnPlayerHurtDialogueChance) { return; }

            if (player.statLife < player.statLifeMax2 * 0.4) // Player is at 40% health or less
            {
                if (OnPlayerHitDialogueTimer > 0) { return; }
                OnPlayerHitDialogueTimer = OnPlayerHitDialogueWaitTime;
                texts.Add("Hey, take a potion before you faint!");
                texts.Add("You're hurt!");
                texts.Add("[nickname], watch out!");
                text = texts[rng.Next(texts.Count)];
                Companion.SaySomething(text, Companion.IsFollower);
            } else {
                return;
            }
            return;
        }

        public void OnCompanionAttacked(int Damage)
        {
            terraguardians.Companion companion = Companion;
            List<string> texts = new List<string>();
            string text;

            // DEATH
            bool isLethal = (companion.Health - Damage) <= 0;
            if (isLethal)
            {
                if (rng.NextDouble() <= 1.0 - OnDeathDialogueChance) { return; }
                texts.Add("No, I can still...");
                texts.Add("No... Not now...");
                texts.Add("Damn it...");
                texts.Add("It's dark...");
                texts.Add("Not again... I...");
                text = texts[rng.Next(texts.Count)];
                companion.SaySomething(text, companion.IsFollower);
                return;
            }

            // Should we generate dialogue?
            if (rng.NextDouble() <= 1.0 - OnHurtByNpcDialogueChance) { return; }

            // Get dialogue randomly
            float currHealthPercentage = (float)companion.Health / (float)companion.MaxHealth;
            if (currHealthPercentage > 0.7)
            {
                texts.Add("Ow!");
                texts.Add("Ouch!");
                texts.Add("Ugh!");
                texts.Add("Hmpf!");
                texts.Add("Come at me!");
                texts.Add("You'll have to hit harder than that!");
                texts.Add("You'll need more than that!");
            }
            else if (currHealthPercentage > 0.4)
            {
                texts.Add("Ow!");
                texts.Add("Ugh!");
                texts.Add("Hmpf!");
                texts.Add("Come at me!");
                texts.Add("You'll pay for that!");
                texts.Add("You'll need more than that!");
            }
            else
            {
                texts.Add("I'm wounded...");
                texts.Add("I'm hurt...");
                texts.Add("I'm fading...");
                texts.Add("Augh!");
                texts.Add("Damn it!");
            }
            text = texts[rng.Next(texts.Count)];

            // Display dialogue in chat and as a popoup
            companion.SaySomething(text, companion.IsFollower);
        }

        // public void OnPlayerDeath(terraguardians.Companion companion, Player player)
        // {
        //     List<string> texts = new List<string>
        //     {
        //         "No, [nickname]!",
        //         "Don't die on me, [nickname]!",
        //         "Stay with me, [nickname]!",
        //         "You... I'll kill you all!"
        //     };
        //     string text = texts[rng.Next(texts.Count)];
        //     companion.SaySomething(text, true);
        //     base.OnPlayerDeath(companion, player);
        // }

        // public override void OnDeath(terraguardians.Companion companion)
        // {
        //     if (rng.NextDouble() <= 1.0 - OnDeathDialogueChance) { return; }
        //     List<string> texts = new List<string>
        //     {
        //         "No, I can still...",
        //         "No... Not now...",
        //         "Damn it...",
        //         "It's dark...",
        //     };
        //     string text = texts[rng.Next(texts.Count)];
        //     companion.SaySomething(text, true);
        //     base.OnDeath(companion);
        // }

        public override void OnNpcDeath(terraguardians.Companion companion, NPC npc)
        {
            if (!companion.IsFollower) { return; }

            // BOSS DEATH
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    companion.SaySomething("Great job! I admit, when it showed its teeth, I had my doubts...", true); return;
                case NPCID.KingSlime:
                    companion.SaySomething("I feel... really sticky after that...", true); return;
                case NPCID.EaterofWorldsHead:
                    companion.SaySomething("The world is a little more at peace now.", true); return;
                case NPCID.BrainofCthulhu:
                    companion.SaySomething("I can finally think... I'm glad we could put a stop to it.", true); return;
                case NPCID.QueenBee:
                    companion.SaySomething("Well, at least it wasn't a giant worm... I hate those.", true); return;
                case NPCID.SkeletronHead:
                    companion.SaySomething("The poor clothier... I have a feeling that's not the last we'll see of him.", true); return;
                case NPCID.Deerclops:
                    companion.SaySomething("What a strange creature... I'm glad we took care of it.", true); return;
                case NPCID.WallofFlesh:
                    companion.SaySomething("The world feels... different...", true); return;
                case NPCID.WyvernHead:
                    companion.SaySomething("I have to admit, it was kind of cute...", true); return;
                case NPCID.Retinazer:
                    companion.SaySomething("We brought an end to those two. Great job, [nickname]!", true); return;
                case NPCID.QueenSlimeBoss:
                    companion.SaySomething("I feel... really, really sticky right now...", true); return;
                case NPCID.HallowBoss:
                    companion.SaySomething("I feel kind of feel bad putting an end to such a pretty creature...", true); return;
                case NPCID.TheDestroyer:
                    companion.SaySomething("We took it down, well-fought [nickname]!", true); return;
                case NPCID.SkeletronPrime:
                    companion.SaySomething("Great job, [nickname].", true); return;
                case NPCID.Plantera:
                    companion.SaySomething("That was difficult...", true); return;
                case NPCID.Golem:
                    companion.SaySomething("We did it!", true); return;
                case NPCID.DukeFishron:
                    companion.SaySomething("I'm never touching another fish again...", true); return;
                case NPCID.CultistBoss:
                    companion.SaySomething("Good riddance...", true); return;
                case NPCID.MoonLordHead:
                    companion.SaySomething("We... actually did it!", true); return;
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
                case NPCID.Nymph:
                    {
                        List<string> msgs = new List<string>
                        {
                            "I really thought she was friendly at first...",
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

            // CRITTERS 
            List<string> texts = new List<string>();
            string text;
            if (npc.CountsAsACritter) {
                texts.Add("That was a close one...");
                texts.Add("Poor thing...");
                text = texts[rng.Next(texts.Count)];
                companion.SaySomething(text, true);
            }

            texts.Add("Take that!");
            texts.Add("Another one down!");
            texts.Add("That's one.");
            texts.Add("Got it!");
            texts.Add($"{npc.FullName} down!");
            float currHealthPercentage = (float)companion.Health / (float)companion.MaxHealth;
            if (currHealthPercentage < 0.4)
            {
                texts.Add("That was a close one...");
                texts.Add("Whew...");
            }
            switch (npc.type)
            {
                case NPCID.Zombie:
                    texts.Add("Rest in peace...");
                    break;
                case NPCID.DemonEye:
                    texts.Add("I don't like how they stare at me...");
                    break;

                // Blood moon
                case NPCID.BloodSquid:
                case NPCID.Drippler:
                case NPCID.BloodZombie:
                    texts.Add("So much blood!");
                    texts.Add("Blood everywhere...");
                    break;

                // Corruption
                case NPCID.EaterofSouls:
                    texts.Add("Disgusting!");
                    texts.Add("Go eat someone else's soul!");
                    texts.Add("Die!");
                    break;
                case NPCID.Corruptor:
                    texts.Add("Disgusting!");
                    texts.Add("Die!");
                    break;
                case NPCID.Wraith:
                    texts.Add("How creepy.");
                    break;

                // Underworld
                case NPCID.FireImp:
                    texts.Add("Little rat.");
                    break;

                // Crimson
                case NPCID.FaceMonster:
                    texts.Add("How terrifying...");
                    break;
                case NPCID.Crimera:
                    texts.Add("Disgusting!");
                    break;
                case NPCID.IchorSticker:
                    texts.Add("Ew, disgusting!");
                    texts.Add("These ones give me the creeps...");
                    break;

                // Hallow
                case NPCID.EnchantedSword:
                    texts.Add("Where can I get a sword like that?");
                    break;
                case NPCID.Gastropod:
                    texts.Add("These things would be cuter if they didn't shoot lasers.");
                    texts.Add("How do these things even shoot lasers?");
                    texts.Add("They're kinda cute...");
                    break;
                case NPCID.Unicorn:
                    texts.Add("I used to want to ride a unicorns as a kid...");
                    break;

                // Sky
                case NPCID.Harpy:
                    texts.Add("They almost look human...");
                    break;

                // Others
                case NPCID.Ghost:
                    texts.Add("Creepy...");
                    break;
                case NPCID.BlueSlime:
                case NPCID.GreenSlime:
                case NPCID.DungeonSlime:
                case NPCID.MotherSlime:
                case NPCID.BlackSlime:
                    texts.Add("So... squishy...");
                    texts.Add("Ugh... so sticky...");
                    texts.Add("I swear I smell like gel...");
                    break;
                case NPCID.IceSlime:
                    texts.Add("At least ice slimes aren't so sticky.");
                    break;
                case NPCID.WallCreeper:
                case NPCID.BloodCrawler:
                    texts.Add("(Shivers) I hate those things...");
                    texts.Add("Gosh I hate these bugs!");
                    texts.Add("So creepy...");
                    break;

                // Goblin Army
                case NPCID.GoblinArcher:
                case NPCID.GoblinPeon:
                case NPCID.GoblinScout:
                case NPCID.GoblinSorcerer:
                case NPCID.GoblinThief:
                    texts.Add("There are so many of them!");
                    texts.Add("They're weak, but they're plentiful!");
                    texts.Add("There's a lot of them!");
                    break;

                // Underground
                case NPCID.GiantWormBody:
                case NPCID.GiantWormHead:
                case NPCID.GiantWormTail:
                    texts.Add("Ugh, I hate worms.");
                    break;
            }

            text = texts[rng.Next(texts.Count)];
            companion.SaySomething(text, true);
            base.OnNpcDeath(companion, npc);
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
            List<string> texts = new List<string>();
            string text;
            if (Player.ZoneBeach && !seenBeachBiome)
            {
                companion.SaySomething("Wow, the ocean... It's so beautiful!", true);
                seenBeachBiome = true;
            } else if (Player.ZoneSnow && !seenSnowBiome)
            {
                companion.SaySomething("Look, Snow! If anything can survive this extreme weather, it must be tougher than normal.", true);
                seenSnowBiome = true;
            } else if (Player.ZoneSnow && Player.ZoneRain && !seenSnow)
            {
                companion.SaySomething("A blizzard. Try and keep warm, [nickname].", true);
                seenSnow = true;
            } else if (Player.ZoneCorrupt && !seenCorruptionBiome)
            {
                companion.SaySomething("I can feel the evil aura in this place... stay close to me, [nickname], we're being watched.", true);
                seenCorruptionBiome = true;
            } else if (Player.ZoneCrimson && !seenCrimsonBiome)
            {
                companion.SaySomething("Gods that smell... is this... flesh...?", true);
                seenCrimsonBiome = true;
            } else if (Player.ZoneDesert && !seenDesertBiome)
            {
                companion.SaySomething("A desert!", true);
                seenDesertBiome = true;
            } else if (Player.ZoneSandstorm && !seenSandstorm)
            {
                companion.SaySomething("A sandstorm! Try not to get any in your eyes.", true);
                seenSandstorm = true;
            } else if (Player.ZoneJungle && !seenJungleBiome)
            {
                companion.SaySomething("It's the Jungle. Watch out, [nickname], I've heard this place is really dangerous.", true);
                seenJungleBiome = true;
            } else if (Player.ZoneGlowshroom && !seenGlowingMushroomBiome)
            {
                companion.SaySomething("Are those... glowing mushrooms? It's beautiful...", true);
                seenGlowingMushroomBiome = true;
            } else if (Player.ZoneMeteor && !seenMeteoriteBiome)
            {
                companion.SaySomething("Looks like you could make something useful out of this hunk of space rock.", true);
                seenMeteoriteBiome = true;
            } else if (Player.ZoneUnderworldHeight && !seenUnderworldBiome)
            {
                companion.SaySomething("We've reached the Underworld, where dead souls rest...", true);
                seenUnderworldBiome = true;
            } else if (Player.ZoneHallow && !seenHallowBiome)
            {
                companion.SaySomething("What a magical place.", true);
                seenHallowBiome = true;
            } else if (Player.ZoneSkyHeight && !seenSpaceBiome)
            {
                texts.Add("The stars are really beautiful, aren't they?");
                texts.Add("It's like I can almost touch the clouds up here!");
                texts.Add("I hope I don't slip and fall...");
                text = texts[rng.Next(texts.Count)];
                companion.SaySomething(text, true);
                seenSpaceBiome = true;
            } else if (Player.ZoneDungeon && !seenDungeonBiome)
            {
                companion.SaySomething("It's the dungeon! I hear it's as bountiful as it is dangerous.", true);
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
            msgs.Add("Ever wonder who leaves all the chests around?");
            msgs.Add("I wonder where slime comes from...");
            msgs.Add("I wish I could train all day.");
            msgs.Add("You ever get the feeling that we're being watched?");
            msgs.Add("*Hums*");

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
            bool isUnderground = Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight || Player.ZoneUnderworldHeight;
            int CompanionsCount = WorldMod.GetCompanionsCount;
            bool HasCompanions = CompanionsCount > 0;
            if (Main.bloodMoon)
            {
                msgs.Add("There are a lot of enemies around.");
                msgs.Add("Keep your guard up.");
                msgs.Add("I hate nights like these.");
            }
            else if (isUnderground) {

            }
            {
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
            // Number of enemies

            // Biome Messages
            if (Player.ZoneBeach)
            {
                msgs.Add("I love going to the beach!");
                msgs.Add("The beach is always nice to be at.");
            } else if (Player.ZoneSnow && Player.ZoneRain)
            {
                msgs.Add("It's freezing!");
                msgs.Add("I can barely see here...");
            } else if (Player.ZoneSnow)
            {
                msgs.Add("*Shivers* So cold!");
            } else if (Player.ZoneCorrupt)
            {
                msgs.Add("Stay on your guard here.");
                msgs.Add("This place gives me the creeps!");
                msgs.Add("I feel like I'm being watched.");
                msgs.Add("We musn't let this Corruption spread.");
            } else if (Player.ZoneCrimson)
            {
                msgs.Add("Man, this place is disgusting.");
                msgs.Add("Gosh, the smell of this place!");
                msgs.Add("I feel like I'm being watched.");
                msgs.Add("We musn't let this Crimson spread.");
            } else if (Player.ZoneDesert)
            {
                msgs.Add("So hot...");
                msgs.Add("I think there's sand in my shoes.");
            } else if (Player.ZoneSandstorm)
            {
                msgs.Add("I can barely see in front of me!");
                msgs.Add("Try not to get sand in your eyes.");
            } else if (Player.ZoneJungle)
            {
                msgs.Add("It's so humid in here.");
                msgs.Add("So many bugs!");
                msgs.Add("I think I got bitten by something.");
                msgs.Add("Monsters here are pretty strong.");
            } else if (Player.ZoneGlowshroom)
            {
                msgs.Add("Mm... mushrooms.");
            } else if (Player.ZoneMeteor)
            {
                msgs.Add("Glad this space rock didn't fall on me.");
            } else if (Player.ZoneUnderworldHeight)
            {
                msgs.Add("Do you think the dead are watching us?");
            } else if (Player.ZoneHallow)
            {
                msgs.Add("What a magical place.");
                msgs.Add("What a pretty place.");
            } else if (Player.ZoneDungeon)
            {
                msgs.Add("Be on your guard, there could be traps about.");
                msgs.Add("I wonder what we can find here?");
                seenDungeonBiome = true;
            } else if (Player.ZoneSkyHeight)
            {
                msgs.Add("Wow, you can almost touch the clouds up here!");
                msgs.Add("I hope I don't slip and fall...");
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
            terraguardians.Companion companion = Companion;
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
                    companion.SaySomething("What the heck is that?!", true);
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
