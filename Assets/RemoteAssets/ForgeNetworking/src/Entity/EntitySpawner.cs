﻿using Forge.Networking.Players;
using Forge.Networking.Unity.Messages;
using UnityEngine;

namespace Forge.Networking.Unity
{
    public static class EntitySpawner
    {
        public static IUnityEntity SpawnEntityFromData(IEngineFacade engine, int id, int prefabId, IPlayerSignature ownerId, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            var go = SpawnGameObjectWithInfo(engine, prefabId, pos, rot, scale);
            return SetupNetworkEntity(engine, go, id, prefabId, ownerId);
        }

        public static IUnityEntity SpawnEntityFromMessage(IEngineFacade engine, SpawnEntityMessage message)
        {
            GameObject go = SpawnGameObject(message, engine);
            return SetupNetworkEntity(engine, go, message.Id, message.PrefabId, message.OwnerId);
        }

        private static GameObject SpawnGameObject(SpawnEntityMessage message, IEngineFacade engine)
        {
            return SpawnGameObjectWithInfo(engine, message.PrefabId, message.Position, message.Rotation, message.Scale);
        }

        private static GameObject SpawnGameObjectWithInfo(IEngineFacade engine, int prefabId, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            var prefab = engine.PrefabManager.GetPrefabById(prefabId);
            Transform t = GameObject.Instantiate(prefab, pos, rot);
            t.localScale = scale;
            return t.gameObject;
        }

        private static IUnityEntity SetupNetworkEntity(IEngineFacade engine, GameObject go, int id, int prefabId, IPlayerSignature ownerId)
        {
            var entity = go.gameObject.GetComponent<NetworkEntity>();

            foreach (var networkBeh in go.GetComponents<NetworkBehaviour>())
            {
                networkBeh.SetEntity(entity);
            }

            entity.OwnerID = ownerId;
            entity.Id = id;
            entity.PrefabId = prefabId;
            engine.EntityRepository.Add(entity);
            return entity;
        }
    }
}