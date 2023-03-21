#if ECSGame
using FriendsGamesTools.ECSGame.DataMigration;
using FriendsGamesTools.EditorTools.BuildModes;
using System;
using System.IO;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public static class Serialization
    {
        public static string path => Application.persistentDataPath + "/SaveWorld.dat";
        public static bool saveExists => System.IO.File.Exists(path);
        static EntityManager manager => World.Active.EntityManager;
        public static void SaveWorld()
        {
#if ECS_SAVE_MIGRATION
            if (migrationFailing)
                Debug.Log($"TODO: overwrite player's save when migrationFailing or sent it to developers for fixing?");
#endif
            var versionIndependantData = UnversioningConverter.VersionedToUnversioned();
            versionIndependantData.SaveVersionIndependant(path);
        }
        public static void DeleteSave()
        {
            if (saveExists)
                System.IO.File.Delete(path);
        }
        static bool migrationFailing;
        public static bool TryLoadWorld(Action<int, int> onMigratedFromTo)
        {
            migrationFailing = false;
            if (!saveExists)
                return false;
            try
            {
                var versionIndependantData = UnversionedWorldData.LoadVersionIndependant(path);
                if (versionIndependantData == null)
                    return false; // Save exists, but unversioned world cant be loaded - its save before migration module.
#if ECS_SAVE_MIGRATION
                if (!Migration.EnsureUpToDate(ref versionIndependantData, onMigratedFromTo))
                {
                    //Debug.LogError("save is invaid, EnsureUpToDate failed");
                    //migrationFailing = true;
                    return false;
                }
#endif
                if (!UnversioningConverter.UnversionedToVersioned(versionIndependantData))
                {
                    Debug.LogError("save is invaid, UnversionedToVersioned failed");
                    migrationFailing = true;
                    return false;
                }
                //Debug.Log($"load success!");
            } catch (Exception e)
            {
                // File.OpenRead(path)
                Debug.LogError($"{e.Message} at {e.StackTrace}");
                if (e.InnerException != null)
                    Debug.LogError($"Inner exception:\n{e.InnerException.Message} at {e.InnerException.StackTrace}");
                migrationFailing = true;
                return false;
            }
            finally
            {
                if (BuildModeSettings.release && migrationFailing)
                    SendSaveToDeveloper("Broken save");
            }
            return true;
        }
        public static void SendSaveToDeveloper(string reason)
        {
            var bytes = File.ReadAllBytes(path);
            var str = bytes.EncodeToString();
            EmailToDevs.Send(reason, str).WrapErrors();
        }
        public static void ApplySaveFromStr(string saveStr)
        {
            var bytes = SerializationUtils.DecodeFromString(saveStr);
            File.WriteAllBytes(path, bytes);
        }
        public static void ClearWorld()
        {
            var entities = manager.GetAllEntities();
            manager.DestroyEntity(entities);
            entities.Dispose();
        }
        public static bool canLoad => false;
    }
}
#endif