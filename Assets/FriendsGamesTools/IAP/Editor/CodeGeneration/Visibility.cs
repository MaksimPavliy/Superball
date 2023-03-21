namespace FriendsGamesTools.CodeGeneration
{
    public enum VisibilityType { NotSet, Public, Private, Protected }
    public class Visibility<TOwner> where TOwner : GenerationRequirement
    {
        TOwner owner;
        public Visibility(TOwner owner) => this.owner = owner;
        public VisibilityType visibility { get; private set; }
        public TOwner RequireVisibility(VisibilityType visibility)
        {
            this.visibility = visibility;
            return owner;
        }
        public TOwner RequirePublic() => RequireVisibility(VisibilityType.Public);
        public TOwner RequirePrivate() => RequireVisibility(VisibilityType.Private);
        public TOwner RequireProtected() => RequireVisibility(VisibilityType.Protected);
        public override string ToString() => visibility == VisibilityType.NotSet ? "" : visibility.ToString().ToLower() + " ";
    }
}