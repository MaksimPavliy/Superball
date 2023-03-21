#if TOUCHES
using UnityEngine;

namespace FriendsGamesTools
{
    public class Touch
    {
        public readonly int id;
        public readonly Vector3 startWorldCoo;
        public readonly Vector2 startScreenCoo;
        public Vector2 deltaScreenCoo { get; private set; }
        public Vector2 currScreenCoo { get; private set; }
        public bool exists;

        public Touch(int id, Vector2 screenCoo, Vector3 startWorldCoo)
        {
            this.id = id;
            startScreenCoo = screenCoo;
            currScreenCoo = screenCoo;
            this.startWorldCoo = startWorldCoo;
        }
        public void ChangeCurrScreenCoo(Vector2 curr)
        {
            deltaScreenCoo = curr - currScreenCoo;
            currScreenCoo = curr;
        }
    }
}
#endif