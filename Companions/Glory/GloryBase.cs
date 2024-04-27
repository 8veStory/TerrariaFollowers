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

namespace gaomonmod1dot4.Companion.Glory
{
    public class GloryBase : TerrarianBase
    {
        public override CompanionGroup GetCompanionGroup => MainMod.followersGroup; // Assigning the Digimon group to the companion. You can take TerraGuardian groups from the MainMod script of TerraGuardian.
        private static Random rng = new Random();

        public GloryBase()
        {
            NpcMod.NpcSpawned += OnBossStart2;
        }

        // DESCRIPTION - Name, and info that shows up in the bestiary
        #region Description
        public override string Name => "Glory";
        public override string Description => """
Glory hails from a great family of warriors who lost their lives defending against a Moon Cultist invasion when she was young. Orphaned, she struck out on her own as a vagabond, honing her craft and learning the way of combat. She now stalks the land where rumours of cult activity appear, and seeks to ruin their world-ending plans.

Stats:
- Having endured difficult martial training, Glory can take more damage than the average Terrarian.
- She is proficient with melee weapons, but not with the arts of magic of mana, though she can be decent with the bow.
""";
        public override int Age => 17;
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
                new InitialItemDefinition(ItemID.HealingPotion, 10)
            };
        }
        #endregion

        // DIALOGUE - All dialogue, including situational, periodic, on hit, etc.
        #region Dialogue
        protected override CompanionDialogueContainer GetDialogueContainer => new GloryDialogues();
        private static double OnAttackDialogueChance = 0.10;
        private static double OnOtherCompanionDeathDialogueChance = 0.50;
        private static double OnDeathDialogueChance = 0.30;
        private static double OnGenericNPCDefeatDialogueChance = 0.08;

        private static int OnGenericNPCDefeatTimer = 360; // 60 = 1sec because we do 60FPS updates
        private int GenericNpcDeathDialogueTimer = 0;

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
                texts.Add("Ow!");
                texts.Add("Come at me!");
                texts.Add("You'll have to hit harder than that!");
            }
            else if (currHealthPercentage > 0.4)
            {
                texts.Add("Ow!");
                texts.Add("You'll pay for that!");
            }
            else
            {
                texts.Add("I'm wounded...");
                texts.Add("I'm fading...");
                texts.Add("Augh!");
                texts.Add("Damn it!");
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
                $"No, {target.name} is down!",
                $"Damn it, {target.name} is down!"
            };
            string text = texts[rng.Next(texts.Count)];

            companion.SaySomething(text, true);
        }

        public override void OnPlayerDeath(terraguardians.Companion companion, Player player)
        {
            List<string> texts = new List<string>
            {
                "No, [nickname]!",
                "Don't die on me, [nickname]!",
                "Stay with me, [nickname]!",
                "You... I'll kill you all!"
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
                "No, I can still...",
                "No... Not now...",
                "Damn it...",
                "It's dark...",
            };
            string text = texts[rng.Next(texts.Count)];
            companion.SaySomething(text, true);
            base.OnDeath(companion);
        }

        public override void OnNpcDeath(terraguardians.Companion companion, NPC npc)
        {
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
                    companion.SaySomething("I can finally think... I'm glad we could stop it.", true); return;
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
                    companion.SaySomething("We... actually did it...", true); return;
            }

            if (GenericNpcDeathDialogueTimer > 0) { return; } // Don't speak if we recently spoke
            if (npc.boss)
            {
                companion.SaySomething("Great job [nickname]! That was a powerful foe.", true);
                GenericNpcDeathDialogueTimer = OnGenericNPCDefeatTimer;
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
                        GenericNpcDeathDialogueTimer = OnGenericNPCDefeatTimer;
                        return;
                    }
                case NPCID.Tim:
                    companion.SaySomething("What a funny looking skeleton...", true);
                    GenericNpcDeathDialogueTimer = OnGenericNPCDefeatTimer;
                    return;
                case NPCID.TheBride:
                    companion.SaySomething("She had a really pretty dress...", true);
                    GenericNpcDeathDialogueTimer = OnGenericNPCDefeatTimer;
                    return;
                case NPCID.TheGroom:
                    companion.SaySomething("That one had a fancy suit on.", true);
                    GenericNpcDeathDialogueTimer = OnGenericNPCDefeatTimer;
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
                        GenericNpcDeathDialogueTimer = OnGenericNPCDefeatTimer;
                        return;
                    }
            }

            // GENERIC NPC DEATH
            if (rng.NextDouble() <= 1.0 - OnGenericNPCDefeatDialogueChance) { return; } // Randomly speak
            GenericNpcDeathDialogueTimer = OnGenericNPCDefeatTimer;
            List<string> texts = new List<string>
            {
                "Take that!",
                "Another one down!",
                "That's one.",
                "Got it!",
                $"{npc.FullName} down!"
            };
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
                case NPCID.BloodZombie:
                    texts.Add("So much blood!");
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
                    texts.Add("Terrifying beings...");
                    break;

                // Underworld
                case NPCID.FireImp:
                    texts.Add("Mischevous little thing!");
                    break;

                // Crimson
                case NPCID.FaceMonster:
                    texts.Add("How terrifying...");
                    texts.Add("These things are terrifying!");
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
                    texts.Add("These flying swords sure are dangerous.");
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
                    texts.Add("Weak!");
                    break;

                // Underground
                case NPCID.GiantWormBody:
                case NPCID.GiantWormHead:
                case NPCID.GiantWormTail:
                    texts.Add("Ugh, I hate worms.");
                    break;
            }

            string text = texts[rng.Next(texts.Count)];
            companion.SaySomething(text, true);
            base.OnNpcDeath(companion, npc);
        }

        public override void UpdateCompanion(terraguardians.Companion companion)
        {
            if (GenericNpcDeathDialogueTimer > 0) { GenericNpcDeathDialogueTimer--; }
            OnBiomePrompt(companion);
        }

        private Player Player => MainMod.GetLocalPlayer;
        //Biomes
        public bool seenDesertBiome;
        public bool seenSnowBiome;
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
                companion.SaySomething("The stars are really beautiful, aren't they?", true);
                seenSpaceBiome = true;
            }
            if (Player.ZoneDungeon && !seenDungeonBiome)
            {
                companion.SaySomething("I've heard the Dungeon holds powerful grimoires and swords... as well as challenging traps and foes. Stay vigilant!", true);
                seenDungeonBiome = true;
            }
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

        public void OnBossStart(terraguardians.Companion companion, IEntitySource source)
        {
            if (NPC.AnyNPCs(NPCID.EyeofCthulhu) && !seenEyeOfCthulhu)
            {
                companion.SaySomething("It can't be... Be careful! I have your back!", true);
                seenEyeOfCthulhu = true;
            }
            if (NPC.AnyNPCs(NPCID.KingSlime) && !seenKingSlime)
            {
                companion.SaySomething("A giant slime!\n Surely it can't be that hard to kill... right?", true);
                seenKingSlime = true;
            }
            if (NPC.AnyNPCs(NPCID.EaterofWorldsHead) && !seenEaterOfWorlds)
            {
                companion.SaySomething("This world is not for yours to take, worm!", true);
                seenEaterOfWorlds = true;
            }
            if (NPC.AnyNPCs(NPCID.BrainofCthulhu) && !seenBrainOfCthulhu)
            {
                companion.SaySomething("Augh, it's getting into my head...! Let's end this thing!", true);
                seenBrainOfCthulhu = true;
            }
            if (NPC.AnyNPCs(NPCID.QueenBee) && !seenQueenBee)
            {
                companion.SaySomething("It's fast, keep your eyes peeled for its dashes!", true);
                seenQueenBee = true;
            }
            if (NPC.AnyNPCs(NPCID.SkeletronHead) && !seenSkeletron)
            {
                companion.SaySomething("He looks powerful, stay away from his limbs!", true);
                seenSkeletron = true;
            }
            if (NPC.AnyNPCs(NPCID.Deerclops) && !seenDeerclops)
            {
                companion.SaySomething("What the heck is that?! Be on your guard!", true);
                seenDeerclops = true;
            }
            if (NPC.AnyNPCs(NPCID.WallofFlesh) && !seenWallOfFlesh)
            {
                companion.SaySomething("So this is it... Keep running, and let's give it a fight to remember!", true);
                seenWallOfFlesh = true;
            }

            if (NPC.AnyNPCs(NPCID.WyvernHead) && !seenWyvern)
            {
                companion.SaySomething("Uh... do you see that thing in the sky?", true);
                seenWyvern = true;
            }
            if (NPC.AnyNPCs(NPCID.Retinazer) && !seenTwins)
            {
                companion.SaySomething("How is this possible...\nthey're just like the Eye... except with more lasers!", true);
                seenTwins = true;
            }
            if (NPC.AnyNPCs(NPCID.QueenSlimeBoss) && !seenQueenSlime)
            {
                companion.SaySomething("This one kind of looks... tasty?", true);
                seenQueenSlime = true;
            }
            if (NPC.AnyNPCs(NPCID.HallowBoss) && !seenEmpress)
            {
                companion.SaySomething("Maybe we shouldn't have touched that butterfly...", true);
                seenEmpress = true;
            }
            if (NPC.AnyNPCs(NPCID.TheDestroyer) && !seenDestroyer)
            {
                companion.SaySomething("The Eater of Worlds... resurrected? We must put this abomination down!", true);
                seenDestroyer = true;
            }
            if (NPC.AnyNPCs(NPCID.SkeletronPrime) && !seenSkeletronPrime)
            {
                companion.SaySomething("He's much more powerful than before... I've got your back!", true);
                seenSkeletronPrime = true;
            }
            if (NPC.AnyNPCs(NPCID.Plantera) && !seenPlantera)
            {
                companion.SaySomething("Watch out for its tendrils! We'll need a big arena to fight this one.", true);
                seenPlantera = true;
            }
            if (NPC.AnyNPCs(NPCID.Golem) && !seenGolem)
            {
                companion.SaySomething("I've heard legends of the Ancient Golem... He's seems slow, but keep your guard up!", true);
                seenGolem = true;
            }
            if (NPC.AnyNPCs(NPCID.DukeFishron) && !seenDukeFishron)
            {
                List<string> texts = new List<string>
                {
                    "Uh, I think you reeled in the wrong fish!",
                    "That's not a fish I want to eat!",
                    "I never really liked seafood..."
                };
                string text = texts[rng.Next(texts.Count)];
                companion.SaySomething(text, true);
                seenDukeFishron = true;
            }
            if (NPC.AnyNPCs(NPCID.CultistBoss) && !seenCultist)
            {
                companion.SaySomething("These cultists are a scourge on this world... I won't let them live!", true);
                seenDukeFishron = true;
            }
            if (NPC.AnyNPCs(NPCID.MoonLordHead) && !seenMoonLord)
            {
                companion.SaySomething("This is it... No matter what happens, we need to beat him.", true);
                seenMoonLord = true;
            }
        }

        public void OnBossStart2(NPC spawnedNpc, IEntitySource source)
        {
            Main.NewText("IS IT A BOSS???");
            if (!spawnedNpc.boss) { return; }
            Main.NewText("WOW IT IS");
            terraguardians.Companion companion = terraguardians.MainMod.GetActiveCompanions.FirstOrDefault(c => c.Base.Equals(this));
            if (companion == null) { return; }

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
