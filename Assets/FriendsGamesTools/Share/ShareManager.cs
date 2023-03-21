#if SHARE
using System;
namespace FriendsGamesTools.Share
{
    public static class ShareManager
    {
        public static void Share(string text) => Share(text, null, null);
        public static void Share(string text, string title) => Share(text, title, null);
        public static void Share(string text, Action<bool> onResponse) => Share(text, null, onResponse);
        public static void Share(string text, string title, Action<bool> onResponse)
        {
#if UNITY_EDITOR
            ShowEmulation(text, title, onResponse);
#else
            var share = new NativeShare().SetText(text);
            if (!title.IsNullOrEmpty())
                share.SetTitle(title).SetSubject(title);
            if (onResponse != null)
                share.SetCallback((result, sharedToAndroidAppId)=> {
                    //Debug.Log($"result={result}, shareTarget={sharedToAndroidAppId}");
                    onResponse(result != NativeShare.ShareResult.NotShared);
                });
            share.Share();
#endif
        }

        private static async void ShowEmulation(string text, string title, Action<bool> onResponse)
        {
            var shared = await ShareNativeWindow.Show(title, text);
            onResponse?.Invoke(shared);
        }
    }
}
#endif