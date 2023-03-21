#if UNITY_EDITOR // FGT needs this enabled always
#pragma warning disable 4014 
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;

namespace FriendsGamesTools.EditorTools
{
    public class EditorCoroutine
    {
        private readonly IEnumerator enumerator;
        private EditorCoroutine(IEnumerator enumerator) => this.enumerator = enumerator;
        public static EditorCoroutine StartCoroutine(IEnumerator enumerator) {
            var coroutine = new EditorCoroutine(enumerator);
            EditorApplication.update += coroutine.OnEditorUpdate;
            return coroutine;
        }
        private void OnEditorUpdate() {
            if (!enumerator.MoveNext())
                EditorApplication.update -= OnEditorUpdate;
        }
    }

    public static class EditorAsync
    {
        public static async Task WaitForSeconds(float seconds)
        {
            var endTime = EditorApplication.timeSinceStartup + seconds;
            while (EditorApplication.timeSinceStartup < endTime)
                await Awaiters.EndOfFrame;
        }
        public static async Task Until(Func<bool> waitingFinishingCondition)
        {
            while (!waitingFinishingCondition())
                await Awaiters.EndOfFrame;
        }
        public static async void ExecuteAfter(float seconds, Action action)
        {
            await WaitForSeconds(seconds);
            action?.Invoke();
        }
        public static async void ExecuteAfter(Func<bool> condition, Action action)
        {
            await Until(condition);
            action?.Invoke();
        }
    }

    public static class UnityWebRequestUtils
    {
        public static async Task SendWebRequestInEditor(this UnityWebRequest www)
        {
            www.SendWebRequest();
            while (!www.isDone)
                await Awaiters.EndOfFrame;
        }
    }
}
#endif