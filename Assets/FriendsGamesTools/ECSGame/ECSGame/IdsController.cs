#if ECSGame
using Unity.Entities;

namespace FriendsGamesTools.ECSGame
{
    public struct Id : IComponentData { public long id; }
    public struct Ids : IComponentData { public long maxUsedId; }
    public class IdsController : Controller
    {
        public static IdsController instance { get; private set; }
        public IdsController() : base() => instance = this;
        static Entity e => ECSUtils.GetSingleEntity<Ids>(true);
        public static Id CreateId()
        {
            if (e == Entity.Null)
                ECSUtils.CreateEntity(new Ids { });
            Id newId = new Id { };
            e.ModifyComponent((ref Ids ids) =>
            {
                newId.id = ++ids.maxUsedId;
            });
            return newId;
        }
        public static Entity GetEntity(long id)
        {
            if (id == -1) return Entity.Null;
            Entity res = Entity.Null;
            instance.Entities.ForEach((Entity currE, ref Id Id)=> {
                if (Id.id == id)
                    res = currE;
            });
            return res;
        }
        public static void RemoveEntity(long id)
        {
            var e = GetEntity(id);
            if (e != Entity.Null)
                e.RemoveEntity();
        }
    }
    public static class IdsUtils
    {
        public static long GetId(this Entity e) => e == Entity.Null ? -1 : e.TryGet(out Id id) ? id.id : -1;
    }
}
#endif