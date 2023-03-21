#if ECS_ISO_HUMAN
using System;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Iso
{
    [RequireComponent(typeof(PicsSetup)), Obsolete(PicsSetup.PicsReplaceObsoleteWarning)]
    public class PicsSetupReplaceFolder : MonoBehaviour
    {
        public string originalFolder;
        public string replacedFolder;
    }
}
#endif