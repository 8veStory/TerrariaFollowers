using Terraria.ModLoader;
using terraguardians;
using Terraria;

namespace gaomonmod1dot4
{
	public class MainMod : Mod
	{
		public static Groups.FollowersGroup followersGroup; //Custom companion group, so you can distinguish companions of a certain type by their group.
		public static Player GetLocalPlayer { get { return Main.player[Main.myPlayer]; } }

		public override void Load()
		{
			followersGroup = new Groups.FollowersGroup(); // Creating the companion group, so I can assign it to companions.
		}

		public override void Unload()
		{
			followersGroup = null;
		}

		public override void PostSetupContent()
		{
			terraguardians.MainMod.AddCompanionDB(new FollowersContainer(), this); //Adding the companion container I made to the companion database. That will control the companions list of the mod. Check DigimonContainer.cs.
			terraguardians.MainMod.AddCompanionHookContainer(new CustomModHooks(), this); //Adds custom mod hooks to TerraGuardians Mod. The Custom Mod Hook only adds a new skin to Blue.
		}
	}
}