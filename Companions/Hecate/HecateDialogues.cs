using Terraria;
using terraguardians;
using System.Collections.Generic;

namespace gaomonmod1dot4.Companions.Hecate
{
    //Contains the dialogues of the companion. Must extend CompanionDialogueContainer.
    public class HecateDialogues : CompanionDialogueContainer //Must be assigned on the companion base file, setting it as the value of "GetDialogueContainer" overrideable method.
    {
        public override string GreetMessages(terraguardians.Companion companion) //Messages for when you just met the companion.
        {
            List<string> Mes = new List<string>();
            Mes.Add("(You see a young warrior, confident and capable) Oh, another traveller? Nice to meet you, I'm Glory! It's a dangerous world as of recent, so if you need help putting down monsters, I'm your girl.");
            return Mes[Main.rand.Next(Mes.Count)];
        }

        public override string NormalMessages(terraguardians.Companion guardian)
        {
            List<string> Mes = new List<string>();

            Mes.Add("You don't need to fight alone, [nickname], you can always rely on me.");
            Mes.Add("I'm a long way from home, but I won't stop till our world finds peace.");
            Mes.Add("Our enemies are numerous, [nickname]. You don't need to fight them alone.");
            Mes.Add("Nice to see you [nickname]! what can I do for you?");
            Mes.Add("Hey [nickname], what can I do for you?");

            if (guardian.FriendshipLevel > 5)
            {
                Mes.Add("We've been fighting together for a little while now [nickname], and I've got to say, you're really strong!");
            }
            if (guardian.FriendshipLevel > 10)
            {
                Mes.Add("Hi [nickname]! How's your day been?");
                Mes.Add("I'm glad I met you, [nickname]. Fighting monsters isn't so bad with friends like you.");
            }
            if (guardian.FriendshipLevel > 15)
            {
                Mes.Add("Hi [nickname]! How can I help:?");
            }

            if (Main.bloodMoon)
            {
                Mes.Add("My master once told me that becoming a warrior is like forging a sword. You need pressure and heat to make a strong one. Tonight is a good night for such a task.");
                Mes.Add("I lost my family on a night much like this, I won't let the same thing happen again.");
                Mes.Add("Stay safe, [nickname], the Blood Moon watches over us.");
            }
            else
            {
                int CompanionsCount = WorldMod.GetCompanionsCount;
                Mes.Add("Oh, you're checking up on me? I'm fine, really, thanks for asking.");
                Mes.Add("Oh, you're checking up on me, [nickname]? I'm fine, really, thanks for asking.");

                bool HasCompanions = CompanionsCount > 0;
                if (Main.dayTime)
                {
                    if (Main.eclipse)
                    {
                        Mes.Add("(She gazes up at the eclipse, brows furrowed in worry, but ready to fight)");
                        Mes.Add("Do you see what happened to the Sun? Something bad is happening...");
                    }
                    else
                    {
                        Mes.Add("It's nice to be in the Sun, wouldn't you agree?");
                        Mes.Add("Vitamin D sure is nice.");
                    }
                }
                else
                {
                    Mes.Add("I surely would like to take a nice nap and enjoy this night. What about you?");
                    Mes.Add("You going to try and get a good rest tonight, [nickname]?");
                    Mes.Add("Don't work too hard, [nickname], breaks are important too.");
                }
                if (Main.raining)
                {
                    Mes.Add("I always loved rain. I used to run outside in it as a kid all the time. I always got sick, mind you, but it was good fun.");
                    Mes.Add("Training in the rain gives a whole new vibe to it. You should try it sometime!");
                }

                if (NPC.AnyNPCs(Terraria.ID.NPCID.Stylist))
                {
                    Mes.Add("Hey, [nickname], I don't look too bad... do I? [nn:" + Terraria.ID.NPCID.Stylist + "] keeps saying I should change up my appearance...");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.Merchant))
                {
                    Mes.Add("It's good to have [nn:" + Terraria.ID.NPCID.Merchant + "] around. Having the right tools and potions is just as important as a good weapon.");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.Mechanic))
                {
                    Mes.Add("I respect [nn:" + Terraria.ID.NPCID.Mechanic + "]. I don't really understand what she makes, but her dedication to her craft inspires me.");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.Nurse))
                {
                    Mes.Add("[nn:" + Terraria.ID.NPCID.Nurse + "] really is indispensible. She healed my injuries a little while back, I'm not sure what we'd do without her.");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.ArmsDealer))
                {
                    Mes.Add("If we're going to beat the monsters of this world, having some of the guns [nn:" + Terraria.ID.NPCID.ArmsDealer + "] sell will surely be helpful.");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.Guide))
                {
                    Mes.Add("Did you know [nn:" + Terraria.ID.NPCID.Guide + "] comes from the 'Order of the Guides'? They're a very well-respected organisation in the world.");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.Dryad))
                {
                    Mes.Add("Did you know [nn:" + Terraria.ID.NPCID.Dryad + "] is one of the last of her kind? It's really sad what happened to her people... We can't let her down!");
                    Mes.Add("I was talking to [nn:" + Terraria.ID.NPCID.Dryad + "] recently, and she told me she's over a 100 years old! She might even have been around when Cthulu was still here...");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.WitchDoctor))
                {
                    Mes.Add("[nn:" + Terraria.ID.NPCID.WitchDoctor + "] sure is a strange one, aren't they?");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.PartyGirl))
                {
                    Mes.Add("I love having [nn:" + Terraria.ID.NPCID.PartyGirl + "] around, she reminds me of my sister!");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.Clothier))
                {
                    Mes.Add("I'm glad [nn:" + Terraria.ID.NPCID.Clothier + "] is alright after defeating his curse. He had such a cruel twist of fate...");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.Demolitionist))
                {
                    Mes.Add("The grenades [nn:" + Terraria.ID.NPCID.Demolitionist + "] sells really do pack a punch! Just make sure not to blow yourself up.");
                }
                if (NPC.AnyNPCs(Terraria.ID.NPCID.Steampunker))
                {
                    Mes.Add("The stuff [nn:" + Terraria.ID.NPCID.Steampunker + "] makes is amazing! I never knew such technology existed...");
                }

                if (CompanionsCount >= 1 && CompanionsCount < 3)
                {
                    Mes.Add("It's pretty dangerous around here, [nickname]. If we could find more people to join us we could keep each other safe.");
                }
                if (CompanionsCount >= 3)
                {
                    Mes.Add("Thank you for your effort in building the town, [nickname], it's wonderful having people around.");
                }
                if (CompanionsCount >= 5)
                {
                    Mes.Add("It's so lively with so many people around. I think you did a great job, [nickname]!");
                }
            }
            if (guardian.IsSleeping)
            {
                Mes.Clear();
                Mes.Add("(She's on her side, deep in sleep. It looks like she's resting well)");
                Mes.Add("(You notice her blushing, and with a happy face, she must be having a good dream)");
            }
            if (terraguardians.PlayerMod.IsHauntedByFluffles(MainMod.GetLocalPlayer) && Main.rand.NextDouble() < 0.75)
            {
                Mes.Clear();
                Mes.Add("Why there is a TerraGuardian on your shoulder?");
            }
            return Mes[Main.rand.Next(Mes.Count)];
        }

        public override string TalkMessages(terraguardians.Companion companion) // This message only appears if you speak with a companion whose friendship exp increases.
        {
            List<string> Mes = new List<string>();
            Mes.Add("You remind me of my master who taught me how to fight. He was always there for me, and so are you. Thank you.");
            Mes.Add("Have I told you recently that I'm glad to have met you, [nickname]? You're a good friend.");
            Mes.Add("Fighting deadly monsters everyday isn't the nicest thing in the world, but it's much better when you're around, [nickname].");
            return Mes[Terraria.Main.rand.Next(Mes.Count)];
        }

        public override string RequestMessages(terraguardians.Companion companion, RequestContext context) //Messages regarding requests. The contexts are used to let you know which cases the message will use.
        {
            switch(context)
            {
                case RequestContext.NoRequest:
                    {
                        List<string> Mes = new List<string>();
                        Mes.Add("I'm fine right now, [nickname], but thank you for asking!");
						Mes.Add("That's nice of you to ask! But, I'm good [nickname].");
						Mes.Add("I'm fine for now. Are you sure there isn't anything I can do for you?");
                        return Mes[Terraria.Main.rand.Next(Mes.Count)];
                    }
                case RequestContext.HasRequest:
                    {
                        List<string> Mes = new List<string>();
                        Mes.Add("I do actually. If you are willing, could you [objective] for me?");
                        return Mes[Main.rand.Next(Mes.Count)];
                    }
                case RequestContext.Completed:
                    {
                        List<string> Mes = new List<string>();
                        Mes.Add("Amazing! Thank you [nickname], you're always so reliable. Here, what would you like?");
						Mes.Add("Thank you so much [nickname], you're a great help. Here, you can have some of my loot.");
						Mes.Add("Thank you [nickname]! You deserve a reward.");
                        return Mes[Terraria.Main.rand.Next(Mes.Count)];
                    }
                case RequestContext.Accepted:
                    return "Thank you [nickname], I can always count on you!";
                case RequestContext.TooManyRequests:
                    return "Sorry [nickname], but I think you have too much to do right now. I don't want you to be overworked.";
                case RequestContext.Rejected:
                    return "That's okay [nickname], I'll get to this myself later anyways.";
                case RequestContext.PostponeRequest:
                    return "Later? Of course, I'll keep it with me.";
                case RequestContext.Failed:
                    {
                        List<string> Mes = new List<string>
                        {
                            "That's okay, [nickname]! I thank you for trying at least.",
                            "That's okay! I thank you at least for trying [nickname]."
                        };
                        return Mes[Terraria.Main.rand.Next(Mes.Count)];
                    }
                case RequestContext.AskIfRequestIsCompleted:
                    {
                        List<string> Mes = new List<string>
                        {
                            "Hey [nickname], how's the request going?",
                        };
                        return Mes[Terraria.Main.rand.Next(Mes.Count)];
                    }
                case RequestContext.RemindObjective:
                    return "Oh, well I need you to [objective].";
            }
            return base.RequestMessages(companion, context);
        }

        public override string AskCompanionToMoveInMessage(terraguardians.Companion companion, MoveInContext context)
        {
            switch(context)
            {
                case MoveInContext.Success:
                    return "Sure! I'll stick with you at your town.";
                case MoveInContext.Fail:
                    return "Sorry [nickname], I've got other things I need to attend to right now.";
                case MoveInContext.NotFriendsEnough:
                    return "I appreciate the offer [nickname], but I think we should get to know each other a bit more before I move in.";
            }
            return base.AskCompanionToMoveInMessage(companion, context);
        }

        public override string AskCompanionToMoveOutMessage(terraguardians.Companion companion, MoveOutContext context)
        {
            switch(context)
            {
                case MoveOutContext.Success:
                    return "Oh, okay, I can do that...";
                case MoveOutContext.Fail:
                    return "I feel like it would be better if I stayed here longer.";
                case MoveOutContext.NoAuthorityTo:
                    return "Uh sorry, you weren't the one to give me this house...";
            }
            return base.AskCompanionToMoveOutMessage(companion, context);
        }

        public override string JoinGroupMessages(terraguardians.Companion companion, JoinMessageContext context)
        {
            switch(context)
            {
                case JoinMessageContext.Success:
                    return "Gladly! Stay close to me [nickname], I'll keep you safe.";
                case JoinMessageContext.FullParty:
                    return "Apologies [nickname], but your party is full right now.";
                case JoinMessageContext.Fail:
                    return "Sorry [nickname], but I can't join you right now.";
            }
            return base.JoinGroupMessages(companion, context);
        }

        public override string LeaveGroupMessages(terraguardians.Companion companion, LeaveMessageContext context)
        {
            switch(context)
            {
                case LeaveMessageContext.Success:
                    return "You know where to find me. Stay safe, [nickname]!";
                case LeaveMessageContext.Fail:
                    return "It'd best if I stay with you for a little more, [nickname].";
                case LeaveMessageContext.AskIfSure:
                    return "It's dangerous here, [nickname]. It'd be best if I accompanied you back to town.";
                case LeaveMessageContext.DangerousPlaceYesAnswer:
                    return "Very well, but don't go dying on me, you hear?";
                case LeaveMessageContext.DangerousPlaceNoAnswer:
                    return "Let's get back to safety.";
            }
            return base.LeaveGroupMessages(companion, context);
        }

        public override string OnToggleShareBedsMessage(terraguardians.Companion companion, bool Share)
        {
            if (Share)
            {
                List<string> Mes = new List<string>();
                Mes.Add("Sure! I haven't had a roommate since I was a kid.");
                Mes.Add("I get up really early, I hope my roommate doesn't mind that.");
                return Mes[Terraria.Main.rand.Next(Mes.Count)];

            }
            return "Great! I like having my own bed.";
        }

        public override string TacticChangeMessage(terraguardians.Companion companion, TacticsChangeContext context) //For when talking about changing their combat behavior.
        {
            switch (context)
            {
                case TacticsChangeContext.OnAskToChangeTactic:
                    return "My style? I specialise in melee weapons and am okay with ranged weapons, not magic though. [nickname], what do you propose?";
                case TacticsChangeContext.ChangeToCloseRange:
                    return "Of course! My specialty.";
                case TacticsChangeContext.ChangeToMidRanged:
                    return "Of course! I'd be glad to.";
                case TacticsChangeContext.ChangeToLongRanged:
                    return "I'll stay back then, but if you need anything sliced up, don't hesitate to bring them to me!";
                case TacticsChangeContext.Nevermind:
                    return "Let me know if there's anything else you need.";
            }
            return base.TacticChangeMessage(companion, context);
        }

        public override string TalkAboutOtherTopicsMessage(terraguardians.Companion companion, TalkAboutOtherTopicsContext context) //FOr when going to speak about other things.
        {
            switch (context)
            {
                case TalkAboutOtherTopicsContext.FirstTimeInThisDialogue:
                    return "Anything else [nickname]?";
                case TalkAboutOtherTopicsContext.AfterFirstTime:
                    return "Anything else?";
                case TalkAboutOtherTopicsContext.Nevermind:
                    return "Ok then.";
            }
            return base.TalkAboutOtherTopicsMessage(companion, context);
        }

        public override string SleepingMessage(terraguardians.Companion companion, SleepingMessageContext context)
        {
            switch (context)
            {
                case SleepingMessageContext.WhenSleeping:
                    {
                        List<string> Mes = new List<string>();
                        Mes.Add("(Glory is sound asleep. It looks like she's resting well)");
                        return Mes[Main.rand.Next(Mes.Count)];
                    }
                case SleepingMessageContext.OnWokeUp:
                    switch (Main.rand.Next(2))
                    {
                        default:
                            return "(She rubs her eyes) *Yawn* Ah... could it not wait to morning, [nickname]?";
                        case 1:
                            return "(She groggily gets up) *Yawn* Oh... morning [nickname]... Need my help with anything?";
                    }
                case SleepingMessageContext.OnWokeUpWithRequestActive:
                    switch (Main.rand.Next(2))
                    {
                        default:
                            return "(Yawns) Ah, hey [nickname]... did you get along to my request?";
                        case 1:
                            return "(She rubs her eyes) Ah, hey [nickname]... did you get along to my request?";
                    }
            }
            return base.SleepingMessage(companion, context);
        }

        public override string InteractionMessages(terraguardians.Companion companion, InteractionMessageContext context)
        {
            switch (context)
            {
                case InteractionMessageContext.OnAskForFavor:
                    return "Of course! What can I do for you, [nickname]?";
                case InteractionMessageContext.Accepts:
                    return "Leave it to me!";
                case InteractionMessageContext.Rejects:
                    return "Uh, sorry, I can't do that, [nickname].";
                case InteractionMessageContext.Nevermind:
                    return "So, nothing then?";
            }
            return base.InteractionMessages(companion, context);
        }

        public override string BuddiesModeMessage(terraguardians.Companion companion, BuddiesModeContext context)
        {
            switch (context)
            {
                case BuddiesModeContext.AskIfPlayerIsSure:
                    return "Are you sure? Bonding is a serious thing... I would never leave the side of my buddy.";
                case BuddiesModeContext.PlayerSaysYes:
                    return "I'll be by your side for as long as you need!";
                case BuddiesModeContext.PlayerSaysNo:
                    return "Okay, [nickname].";
                case BuddiesModeContext.NotFriendsEnough:
                    if (companion.FriendshipLevel > 10)
                    {
                        return "Sorry [nickname], I like you well enough, but I don't know you well enough for something like that.";
                    }
                    else if (companion.FriendshipLevel > 5)
                    {
                        return "(She shakes her head) Sorry [nickname] I don't think we've known each other long enough.";
                    }
                    else
                    {
                        return "Sorry, but no, [nickname], I don't know you well enough for something like that.";
                    }
                case BuddiesModeContext.AlreadyHasBuddy:
                    return "(She raises an eyebrow) Don't you already have one, [nickname]?";
                case BuddiesModeContext.Failed:
                    if (companion.FriendshipLevel > 10)
                    {
                        return "Sorry [nickname], I like you well enough, but I don't know you well enough for something like that.";
                    }
                    else if (companion.FriendshipLevel > 5)
                    {
                        return "(She shakes her head) Sorry [nickname] I don't think we've known each other long enough.";
                    }
                    else
                    {
                        return "Sorry, but no, [nickname], I don't know you well enough for something like that.";
                    }
            }
            return base.BuddiesModeMessage(companion, context);
        }

        public override string InviteMessages(terraguardians.Companion companion, InviteContext context)
        {
            switch (context)
            {
                case InviteContext.Success:
                    return "I'm on my way, [nickname]!";
                case InviteContext.SuccessNotInTime:
                    return "I am on my way, but it'll take me a while to get there!";
                case InviteContext.Failed:
                    return "Sorry, not right now [nickname], I am preoccupied.";
                case InviteContext.CancelInvite:
                    return "Understood.";
                case InviteContext.ArrivalMessage:
                    return "Let's venture, [nickname]!";
            }
            return "";
        }

        public override string ReviveMessages(terraguardians.Companion companion, Player target, ReviveContext context)
        {
            switch (context)
            {
                case ReviveContext.HelpCallReceived:
                    return "You'll be alright, I'm make sure of it.";
                case ReviveContext.RevivingMessage:
                    {
                        List<string> Mes = new List<string>();
                        if (!(target is terraguardians.Companion))
                        {
                            Mes.Add("You'll be okay [nickname], I'll see you through!");
                            Mes.Add("You're not dying on me, [nickname]!");
                        }
                        else
                        {
                            Mes.Add("Stay calm [nickname], I have you!");
                            Mes.Add("You're not dying on me that easy, [nickname]!");
                            Mes.Add("I'll protect you, [nickname]!");
                            Mes.Add("You're not dying on me, [nickname]!");
                        }
                        return Mes[Main.rand.Next(Mes.Count)];
                    }
                case ReviveContext.OnComingForFallenAllyNearbyMessage:
                    {
                        List<string> Mes = new List<string>
                        {
                            "Stay put, I'm coming!",
                            "I'm on my way!"
                        };
                        return Mes[Main.rand.Next(Mes.Count)];
                    }
                case ReviveContext.ReachedFallenAllyMessage:
                    {
                        List<string> Mes = new List<string>
                        {
                             "I'm okay... I've been through worse.",
                            "I ain't dying that easy!"
                        };
                        return Mes[Main.rand.Next(Mes.Count)];
                    }
                case ReviveContext.RevivedByItself:
                    return "I'm okay... I've been through worse.";
                case ReviveContext.ReviveWithOthersHelp:
                    return "Thank you... I wasn't sure if I would've made it.";
            }
            return base.ReviveMessages(companion, target, context);
        }

        public override string GetOtherMessage(terraguardians.Companion companion, string Context)
        {
            return base.GetOtherMessage(companion, Context);
        }
    }
}
