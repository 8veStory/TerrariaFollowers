using Terraria.ModLoader;
using terraguardians;
using gaomonmod1dot4.Companions;
using gaomonmod1dot4.Companion.Glory;
using gaomonmod1dot4.Companion.Hecate;

namespace gaomonmod1dot4
{
    public class FollowersContainer : CompanionContainer // Must inherit CompanionContainer
    {
        public const uint Glory = 0;
        public const uint Hecate = 1;

        public override CompanionBase GetCompanionDB(uint ID) // This overrideable method will be used to get the companion base infos. Use each ID for different companions as you seem fit.
        {
            switch(ID)
            {
                case Glory: // Glory equals to 0. So calling a companion of ID 0, and set the ModID as the name of this mod, will be refering to Glory.
                    return new GloryBase(); // The base infos of Glory. This will only be called once, and be stored internally into a database of the mod.
                case Hecate:
                    return new HecateBase();
            }
            return base.GetCompanionDB(ID);
        }
    }
}