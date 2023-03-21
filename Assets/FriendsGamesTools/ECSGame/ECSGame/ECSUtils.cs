#if ECSGame
using System;
using System.Collections.Generic;
using System.Text;
using FriendsGamesTools.ECSGame.DataMigration;
using Unity.Entities;

namespace FriendsGamesTools.ECSGame
{
    public static partial class ECSUtils
    {
        private static EntityManager manager => World.Active.EntityManager;        
        public static void ModifyComponent<T>(this Entity entity, RefAction<T> change) where T : struct, IComponentData
        {
            var data = manager.GetComponentData<T>(entity);
            change(ref data);
            manager.SetComponentData(entity, data);
        }
        public static void Modify<T>(this Entity entity, RefAction<T> change) where T : struct, IComponentData
            => entity.ModifyComponent(change);
        public delegate void ChangeComponent<T1, T2>(ref T1 c1, ref T2 c2) where T1 : struct, IComponentData where T2 : struct, IComponentData;
        public static void ModifyComponent<T1, T2>(this Entity entity, ChangeComponent<T1,T2> change) where T1 : struct, IComponentData where T2 : struct, IComponentData
        {
            var data1 = manager.GetComponentData<T1>(entity);
            var data2 = manager.GetComponentData<T2>(entity);
            change(ref data1, ref data2);
            manager.SetComponentData(entity, data1);
            manager.SetComponentData(entity, data2);
        }
        public delegate void ChangeComponent<T1, T2, T3>(ref T1 c1, ref T2 c2, ref T3 c3) where T1 : struct, IComponentData where T2 : struct, IComponentData where T3 : struct, IComponentData;
        public static void ModifyComponent<T1, T2, T3>(this Entity entity, ChangeComponent<T1, T2, T3> change) where T1 : struct, IComponentData where T2 : struct, IComponentData where T3 : struct, IComponentData
        {
            var data1 = manager.GetComponentData<T1>(entity);
            var data2 = manager.GetComponentData<T2>(entity);
            var data3 = manager.GetComponentData<T3>(entity);
            change(ref data1, ref data2, ref data3);
            manager.SetComponentData(entity, data1);
            manager.SetComponentData(entity, data2);
            manager.SetComponentData(entity, data3);
        }
        public static void AddOrModifyComponent<T>(this Entity entity, T data) where T : struct, IComponentData
        {
            if (entity.HasComponent<T>())
                entity.ModifyComponent((ref T d) => d = data);
            else
                entity.AddComponent(data);
        }
        public static void AddOrModifyComponent<T>(this Entity entity, RefAction<T> change) where T : struct, IComponentData
        {
            if (entity.HasComponent<T>())
                entity.ModifyComponent(change);
            else
            {
                var val = default(T);
                change(ref val);
                entity.AddComponent(val);
            }
        }
        public static void AddComponent<T>(this Entity entity, T data) where T : struct, IComponentData
        {
            manager.AddComponent<T>(entity);
            manager.SetComponentData<T>(entity, data);
        }
        public static void Add<T>(this Entity entity, T data) where T : struct, IComponentData => entity.AddComponent(data);
        public static bool TryGetComponent<T>(this Entity entity, out T data) where T : struct, IComponentData
        {
            if (!entity.HasComponent<T>())
            {
                data = default;
                return false;
            }
            data = entity.GetComponentData<T>();
            return true;
        }
        public static bool TryGet<T>(this Entity entity, out T data) where T : struct, IComponentData
        => entity.TryGetComponent(out data);
        public static bool TryGetComponent(this Entity entity, Type componentType, out object data)
        {
            if (!entity.HasComponent(componentType)) {
                data = null;
                return false;
            }
            data = entity.GetComponentData(componentType);
            return true;
        }
        static object[] emptyArgs = new object[] { };
        static object[] oneArg = new object[] { null };
        static object[] twoArgs = new object[] { null, null };
        public static object GetComponentData(this Entity entity, Type componentType)
        {
            var genericMethod = UnversioningConverter.GenericGetComponentDataMethodInfo();
            var method = genericMethod.MakeGenericMethod(componentType);
            oneArg[0] = entity;
            return method.Invoke(manager, oneArg);
        }
        public static void SetComponentData(this Entity entity, Type componentType, object data)
        {
            var genericMethod = UnversioningConverter.GenericSetComponentDataMethodInfo();
            var method = genericMethod.MakeGenericMethod(componentType);
            twoArgs[0] = entity;
            twoArgs[1] = data;
            method.Invoke(manager, twoArgs);
        }
        public static void AddComponentData(this Entity entity, Type componentType, object data)
        {
            var genericMethod = UnversioningConverter.GenericAddComponentDataMethodInfo();
            var method = genericMethod.MakeGenericMethod(componentType);
            twoArgs[0] = entity;
            twoArgs[1] = data;
            method.Invoke(manager, twoArgs);
        }
        public static T GetComponentData<T>(this Entity entity) where T : struct, IComponentData
            => manager.GetComponentData<T>(entity);
        public static T Get<T>(this Entity entity) where T : struct, IComponentData => entity.GetComponentData<T>();
        public static void SetComponentData<T>(this Entity entity, T data) where T : struct, IComponentData
            => manager.SetComponentData<T>(entity, data);
        public static void Set<T>(this Entity entity, T data) where T : struct, IComponentData
            => entity.SetComponentData(data);
        public static bool HasComponent<T>(this Entity entity) => manager.HasComponent<T>(entity);
        public static bool HasComponent(this Entity entity, Type componentType)
            => manager.HasComponent(entity, componentType);
        public static void RemoveComponent<T>(this Entity entity) => manager.RemoveComponent<T>(entity);
        public static bool TryRemoveComponent<T>(this Entity entity)
        {
            if (!entity.HasComponent<T>()) return false;
            entity.RemoveComponent<T>(); return true;
        }
        public static DynamicBuffer<T> AddBuffer<T>(this Entity entity) where T : struct, IBufferElementData
            => manager.AddBuffer<T>(entity);
        public static void RemoveBuffer<T>(this Entity entity) where T : struct, IBufferElementData
        {
            var arrayType = ComponentType.ReadWrite<T>();
            manager.RemoveComponent(entity, arrayType);
        }
        public static void TryRemoveBuffer<T>(this Entity entity) where T : struct, IBufferElementData
        {
            if (entity.HasBuffer<T>())
                entity.RemoveBuffer<T>();
        }
        public static DynamicBuffer<T> GetBuffer<T>(this Entity entity) where T : struct, IBufferElementData
            => manager.GetBuffer<T>(entity);
        public static bool HasBuffer<T>(this Entity entity) where T : struct, IBufferElementData
        {
            var arrayType = ComponentType.ReadWrite<T>();
            return manager.HasComponent(entity, arrayType);
        }
        public static void ModifyItem<T>(this DynamicBuffer<T> items, int ind, RefAction<T> modify)
            where T : struct, IBufferElementData
        {
            var item = items[ind];
            modify(ref item);
            items[ind] = item;
        }
#if ECS_GAMEROOT
        public static bool IsInProcess(this Entity entity)
            => entity.TryGetComponent<DurableProcess>(out var process) && !process.finished;
#endif
        public static int Count<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate)
            where T : struct, IBufferElementData
        {
            // TODO: Remove when unity implements GetEnumerator for buffers
            int count = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (predicate(buffer[i]))
                    count++;
            }
            return count;
        }
        public static int FindIndex<T>(this DynamicBuffer<T> buffer, Predicate<T> predicate)
            where T : struct, IBufferElementData
        {
            // TODO: Remove when unity implements GetEnumerator for buffers
            for (int i = 0; i < buffer.Length; i++)
            {
                if (predicate(buffer[i]))
                    return i;
            }
            return -1;
        }
        public static List<T> ToList<T>(this DynamicBuffer<T> buffer) 
            where T : struct, IBufferElementData
        {
            // TODO: Remove when unity implements GetEnumerator for buffers
            List<T> list = new List<T>();
            for (int i = 0; i < buffer.Length; i++)
                list.Add(buffer[i]);
            return list;
        }
        public static int Count<T>() where T : struct, IComponentData
        {
            var count = 0;
            ForEachEntityWith<T>(_ => count++);
            return count;
        }
        public static void ForEachEntityWith<T>(Action<Entity> action)
            where T : struct, IComponentData
        {
            using (var queryResult = manager.CreateEntityQuery(new ComponentType(typeof(T)))
                .ToEntityArray(Unity.Collections.Allocator.Persistent))
            {
                foreach (var entity in queryResult)
                    action(entity);
            }
        }
        public static void ForEachEntityWith<T1, T2>(Action<Entity> action)
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
        {
            using (var queryResult = manager.CreateEntityQuery(new ComponentType(typeof(T1)), new ComponentType(typeof(T2)))
                .ToEntityArray(Unity.Collections.Allocator.Persistent))
            {
                foreach (var entity in queryResult)
                    action(entity);
            }
        }
        public static List<Entity> GetAllEntitiesWith<T>(List<Entity> items = null)
            where T : struct, IComponentData
        {
            if (items == null)
                items = new List<Entity>();
            else
                items.Clear();
            ForEachEntityWith<T>(e => items.Add(e));
            return items;
        }
        public static List<Entity> GetAllEntitiesWith<T1, T2>(List<Entity> items = null)
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
        {
            if (items == null)
                items = new List<Entity>();
            else
                items.Clear();
            ForEachEntityWith<T1, T2>(e => items.Add(e));
            return items;
        }
        public static Entity GetEntityWith<T>(Func<T, bool> condition) where T : struct, IComponentData
        {
            using (var queryResult = manager.CreateEntityQuery(new ComponentType(typeof(T)))
                .ToEntityArray(Unity.Collections.Allocator.Persistent)) {
                foreach (var entity in queryResult)
                {
                    if (condition(entity.GetComponentData<T>()))
                        return entity;
                }
            }
            return Entity.Null;
        }
        public static T GetDataWith<T>(Func<T, bool> condition) where T : struct, IComponentData
            => GetEntityWith<T>(condition).Get<T>();
        public static Dictionary<TKey, Entity> GetAllEntitiesWith<T, TKey>(Func<Entity, TKey> getId, 
            Dictionary<TKey, Entity> items = null)
            where T : struct, IComponentData
        {
            if (items == null)
                items = new Dictionary<TKey, Entity>();
            else
                items.Clear();
            ForEachEntityWith<T>(e => items.Add(getId(e), e));
            return items;
        }
        public static void RemoveAllEntitiesWith<T>()
            where T : struct, IComponentData
            => GetAllEntitiesWith<T>().ForEach(t => t.RemoveEntity());
        public static bool TryGetSingleEntity<T>(out Entity entity) where T : struct, IComponentData
        {
            var queryResult = manager.CreateEntityQuery(new ComponentType(typeof(T)))
                .ToEntityArray(Unity.Collections.Allocator.Persistent);
            if (queryResult.Length > 1)
                throw new System.Exception($"GetSingleEntity<{typeof(T).Name}>() failed, its {queryResult.Length} entities actually");
            if (queryResult.Length == 0)
                entity = Entity.Null;
            else
                entity = queryResult[0];
            queryResult.Dispose();
            return entity != Entity.Null;
        }
        public static Entity GetSingleEntity<T>(bool canBeNull = false) where T : struct, IComponentData
        {
            var queryResult = manager.CreateEntityQuery(new ComponentType(typeof(T)))
                .ToEntityArray(Unity.Collections.Allocator.Persistent);
            if (queryResult.Length > 1 || (queryResult.Length == 0 && !canBeNull))
                throw new System.Exception($"GetSingleEntity<{typeof(T).Name}>() failed, its {queryResult.Length} entities actually");
            var entity = queryResult.Length > 0 ? queryResult[0] : Entity.Null;
            queryResult.Dispose();
            return entity;
        }
        public static T GetSingleComponentData<T>(bool canBeNull = false) where T : struct, IComponentData {
            var e = GetSingleEntity<T>(canBeNull);
            if (canBeNull && e == Entity.Null) return default;
            return e.GetComponentData<T>();
        }
        public static Entity CreateEntity() => manager.CreateEntity();
        public static Entity CreateEntity<T>(T data) where T : struct, IComponentData
        {
            var e = CreateEntity();
            e.AddComponent(data);
            return e;
        }
        public static Entity CreateEntity<T1, T2>(T1 data1, T2 data2) 
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
        {
            var e = CreateEntity(data1);
            e.AddComponent(data2);
            return e;
        }
        public static Entity CreateEntity<T1, T2, T3>(T1 data1, T2 data2, T3 data3)
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData
        {
            var e = CreateEntity(data1, data2);
            e.AddComponent(data3);
            return e;
        }
        public static Entity CreateEntity<T1, T2, T3, T4>(T1 data1, T2 data2, T3 data3, T4 data4)
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData
            where T4 : struct, IComponentData
        {
            var e = CreateEntity(data1, data2, data3);
            e.AddComponent(data4);
            return e;
        }
        public static void RemoveEntity(this Entity e) => manager.DestroyEntity(e);
        public static bool Exists(this Entity e) => manager.Exists(e);


        public static void ForEach<T>(this EntityQueryBuilder entities, Action<Entity, T> action)
            where T : struct, IComponentData
            => entities.WithAllReadOnly<T>().ForEach((Entity e, ref T t) => action(e, t));
        public static void ForEach<T1, T2>(this EntityQueryBuilder entities, Action<Entity, T1, T2> action)
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
            => entities.WithAllReadOnly<T1,T2>().ForEach((Entity e, ref T1 t1, ref T2 t2) => action(e, t1, t2));

        public const int MaxStringChars = 100;
        public static void AddString(this Entity e, string str)
        {
            e.AddBuffer<StringChar>();
            e.ChangeString(str);
        }
        static StringBuilder sb;
        static void InitSB()
        {
            if (sb == null)
                sb = new StringBuilder();
            else
                sb.Clear();
        }
        public static void ChangeString(this Entity e, string str)
        {
            var chars = e.GetBuffer<StringChar>();
            chars.Clear();
            for (int i = 0; i < str.Length; i++)
                chars.Add(new StringChar { charInt = str[i] });
        }
        public static void SetString(this Entity e, string str)
        {
            if (!e.HasString())
                e.AddString(str);
            else
                e.ChangeString(str);
        }
        public static string GetString(this Entity e)
        {
            InitSB();
            var chars = e.GetBuffer<StringChar>();
            for (int i = 0; i < chars.Length; i++)
                sb.Append((char)chars[i].charInt);
            return sb.ToString();
        }
        public static bool HasString(this Entity e) => e.HasComponent<StringChar>();
        public static Entity Clone(this Entity source)
        {
            if (source == Entity.Null) return Entity.Null;
            var clone = CreateEntity();
            var types = manager.GetComponentTypes(source);
            foreach (var compType in types) {
                var type = compType.GetManagedType();
                clone.AddComponentData(type, source.GetComponentData(type));
            }
            return clone;
        }
    }

    [InternalBufferCapacity(ECSUtils.MaxStringChars)]
    public struct StringChar : IBufferElementData { public int charInt; }
}
#endif