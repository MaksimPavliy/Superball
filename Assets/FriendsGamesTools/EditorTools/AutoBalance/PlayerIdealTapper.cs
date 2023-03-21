#if AUTO_BALANCE
using FriendsGamesTools.ECSGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.AutoBalance
{
    // Makes list of IAITappable controllers from game root.
    // Taps everything that is tappable.
    public class PlayerIdealTapper : PlayerAI
    {
        private class Tappable
        {
            public Controller c;
            public Func<Controller, bool> tapIfPossible;
        }
        List<Tappable> tappables = new List<Tappable>();
        protected void AddTappable<T>(Func<T, bool> tapIfPossible) where T : Controller
        {
            var controller = root.controllers.Find(c => c is T);
            Debug.Assert(controller != null);
            tappables.Add(new Tappable {
                c = controller,
                tapIfPossible = (c) => tapIfPossible((T)controller)
            });
        }

        public override void Update()
        {
            foreach (var t in tappables) {
                if (t.tapIfPossible(t.c))
                    break;
            }
        }
    }
}
#endif