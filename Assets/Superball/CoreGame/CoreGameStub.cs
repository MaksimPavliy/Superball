using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using TMPro;
using UnityEngine;

namespace Superball
{
    public class CoreGameStub : MonoBehaviour
    {
        LevelBasedController levels => SuperballRoot.instance.levels;
        [SerializeField] GameObject buttonsParent;
        [SerializeField] TextMeshProUGUI instanceLabel;
        static int instancesCount;
        private void Awake()
        {
            instanceLabel.Safe(() => instanceLabel.text = instancesCount.ToString());
            instancesCount++;
        }
        private void Update()
        {
            var isPlaying = levels.state == Level.State.playing;
            buttonsParent.SetActive(isPlaying);
        }
        public void OnWinPressed() => levels.DebugWin();
        public void OnLosePressed() => levels.DebugLose();
    }
}