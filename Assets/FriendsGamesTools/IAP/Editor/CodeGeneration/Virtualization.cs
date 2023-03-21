namespace FriendsGamesTools.CodeGeneration
{
    public enum VirtualizationType { NotSet, Abstract, Virtual, Override }
    public class Virtualization<TOwner> where TOwner : GenerationRequirement
    {
        TOwner owner;
        public Virtualization(TOwner owner) => this.owner = owner;
        public VirtualizationType virtualization { get; private set; }
        public TOwner RequireVirtualization(VirtualizationType virtualization)
        {
            this.virtualization = virtualization;
            return owner;
        }
        public TOwner RequireAbstract() => RequireVirtualization(VirtualizationType.Abstract);
        public TOwner RequireVirtual() => RequireVirtualization(VirtualizationType.Virtual);
        public TOwner RequireOverride() => RequireVirtualization(VirtualizationType.Override);
        public override string ToString() => virtualization == VirtualizationType.NotSet ? "" : virtualization.ToString().ToLower() + " ";
    }
}