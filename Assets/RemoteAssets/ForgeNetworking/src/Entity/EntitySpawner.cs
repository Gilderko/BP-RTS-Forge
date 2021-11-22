  
using Forge.Networking.Players;
using Forge.Networking.Unity.Messages;
using UnityEngine;

namespace Forge.Networking.Unity
{
    public static class EntitySpawner
    {
        public static IUnityEntity SpawnEntityFromData(IEngineFacade engine, int id, int prefabId, IPlayerSignature ownerId, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            var go = SpawnGameObjectWithInfo(engine, prefabId, ownerId.GetId(), pos, rot, scale);
            return SetupNetworkEntity(engine, go, id, prefabId, ownerId);
        }

        public static IUnityEntity SpawnEntityFromMessage(IEngineFacade engine, SpawnEntityMessage message)
        {
            GameObject go = SpawnGameObject(message, engine, message.OwnerId.GetId());
            return SetupNetworkEntity(engine, go, message.Id, message.PrefabId, message.OwnerId);
        }

        private static GameObject SpawnGameObject(SpawnEntityMessage message, IEngineFacade engine, int ownerId)
        {
            return SpawnGameObjectWithInfo(engine, message.PrefabId, ownerId, message.Position, message.Rotation, message.Scale);
        }

        private static GameObject SpawnGameObjectWithInfo(IEngineFacade engine, int prefabId, int ownderId, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            var parentObject = RTSNetworkManager.Instance.GetRTSPlayerById(ownderId);
            var prefab = engine.PrefabManager.GetPrefabById(prefabId);

            if (parentObject == null)
            {                
                Transform t = GameObject.Instantiate(prefab, pos, rot);
                t.localScale = scale;
                return t.gameObject;
            }
            else
            {
                Transform t = GameObject.Instantiate(prefab, pos, rot, parentObject.transform);
                t.localScale = scale;
                return t.gameObject;
            }
        }
    

        private static IUnityEntity SetupNetworkEntity(IEngineFacade engine, GameObject go, int id, int prefabId, IPlayerSignature ownerId)
        {
            var entity = go.gameObject.GetComponent<NetworkEntity>();

            foreach (var networkBeh in go.GetComponents<NetworkBehaviour>())
            {
                networkBeh.SetEntity(entity);
            }

            entity.engine = engine;
            entity.OwnerId = ownerId;
            entity.Id = id;
            entity.PrefabId = prefabId;
            engine.EntityRepository.Add(entity);
            return entity;
        }
    }
}
