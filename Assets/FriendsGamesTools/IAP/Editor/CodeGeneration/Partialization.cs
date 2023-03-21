namespace FriendsGamesTools.CodeGeneration
{
    public class Partialization<TOwner> where TOwner : GenerationRequirement
    {
        TOwner owner;
        public Partialization(TOwner owner) => this.owner = owner;
        public bool isPartial;
        public TOwner RequirePartial()
        {
            isPartial = true;
            return owner;
        }
        public override string ToString() => isPartial?"partial ":"";
    }
}