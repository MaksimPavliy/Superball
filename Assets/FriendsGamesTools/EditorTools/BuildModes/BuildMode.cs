namespace FriendsGamesTools.EditorTools.BuildModes
{
    public enum BuildModeType
    {
        /// <summary>
        /// For programmers, debug, own tests, optimized for developing conveniance and speed.
        /// </summary>
        Develop = 0,
        /// <summary>
        /// For builds for showing to PM, for tests.
        /// </summary>
        Test = 1,
        /// <summary>
        /// Release for stores.
        /// </summary>
        Release = 2
    }
}
