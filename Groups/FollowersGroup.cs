using gaomonmod1dot4.Companion.Glory;
using terraguardians;

namespace gaomonmod1dot4.Groups
{
    public class FollowersGroup : CompanionGroup
    {
        public override string ID => "followers";
        public override string Name => "Followers Group";
        public override string Description => "Various Terrarian Followers.";
        public GloryBase Glory { get; set; }
    }
}