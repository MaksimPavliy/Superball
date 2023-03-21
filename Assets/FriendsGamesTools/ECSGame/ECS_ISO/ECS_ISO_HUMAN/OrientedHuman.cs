#if ECS_ISO_HUMAN
using System;
using System.Collections.Generic;

namespace FriendsGamesTools.ECSGame.Iso
{
    [Serializable]
    public class OrientedHuman
    {
        public SpriteAndShadow standing;
        public List<SpriteAndShadow> walk;
        public List<SpriteAndShadow> seating;
    }
}
#endif