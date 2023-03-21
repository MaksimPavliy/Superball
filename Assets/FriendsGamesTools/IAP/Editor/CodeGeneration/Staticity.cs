namespace FriendsGamesTools.CodeGeneration
{
    public class Staticity<TOwner> where TOwner : GenerationRequirement
    {
        TOwner owner;
        public Staticity(TOwner owner) => this.owner = owner;
        public bool isStatic { get; private set; }
        public TOwner RequireStatic()
        {
            isStatic = true;
            return owner;
        }
        public override string ToString() => isStatic ? "static " : "";
    }
}