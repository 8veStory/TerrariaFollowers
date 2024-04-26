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

namespace gaomonmod1dot4.Companion.Glory
{
    public class GloryBase : TerrarianBase
    {
        public override CompanionGroup GetCompanionGroup => MainMod.followersGroup; // Assigning the Digimon group to the companion. You can take TerraGuardian groups from the MainMod script of TerraGuardian.
        private static Random rng = new Random();

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

        // Hurt Dialogue
        public override void OnAttackedByNpc(terraguardians.Companion companion, NPC attacker, int Damage, bool Critical)
        {
            // Should we generate dialogue?
            if (rng.NextDouble() <= 0.85) { return; }

            // Get dialogue randomly
            float currHealthPercentage = (float) companion.Health / (float) companion.MaxHealth;
            List<string> texts = new List<string>();
            if (currHealthPercentage > 0.7)
            {
                texts.Add("Ow!");
                texts.Add("Come at me!");
                texts.Add("You'll have to hit harder than that!");
            } else if (currHealthPercentage > 0.4)
            {
                texts.Add("Ow!");
                texts.Add("You'll pay for that!");
            } else
            {
                texts.Add("I'm wounded...");
                texts.Add("Augh!");
                texts.Add("Damn it!");
            }
            string text = texts[rng.Next(texts.Count)];

             // Display dialogue in chat and as a popoup
            companion.SaySomething(text, true);
        }

        public override void OnCompanionDeath(terraguardians.Companion companion, terraguardians.Companion target)
        {
            if (rng.NextDouble() <= 0.50) { return; }
            List<string> texts = new List<string>
            {
                $"No, {target.name} is down!",
                $"Damn it, {target.name} is down!",
                $"Damn it.",
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
        }

        public override void UpdateCompanion(terraguardians.Companion companion)
        {
            OnBossStart(companion);
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
        public bool seenTwins;
        public bool seenDestroyer;
        public bool seenSkeletronPrime;
        public bool seenPlantera;
        public bool seenGolem;
        public bool seenDukeFishron;
        public bool seenCultist;
        public bool seenEmpress;
        public bool seenMoonLord;

        public Dictionary<short, bool> seenBosses;
        public void OnBossStart(terraguardians.Companion companion)
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
               companion.SaySomething("Augh, my head...! Let's end this thing!", true);
               seenBrainOfCthulhu = true;
           }
           if (NPC.AnyNPCs(NPCID.QueenBee) && !seenQueenBee)
           {
               companion.SaySomething("It's fast, keep your eyes peeled!", true);
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
               seenSkeletron = true;
           }
           if (NPC.AnyNPCs(NPCID.WallofFlesh) && !seenWallOfFlesh)
           {
               companion.SaySomething("So this is it... Let's give it a fight to remember!", true);
               seenWallOfFlesh = true;
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
               companion.SaySomething("I've heard legends of this Golem... He's seems slow, but keep your guard up!", true);
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
               companion.SaySomething("These cultists killed my family... I won't let them live!", true);
               seenDukeFishron = true;
           }
           if (NPC.AnyNPCs(NPCID.MoonLordHead) && !seenMoonLord)
           {
               companion.SaySomething("This is it. No matter what happens... I'm glad I could fight him beside you.", true);
               seenMoonLord = true;
           }
        }
        #endregion
    }
}
