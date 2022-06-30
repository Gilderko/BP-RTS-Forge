using Forge.Networking.Unity;
using System.Linq;
using UnityEngine;

/// <summary>
/// Prefab manager allows to get prefabs by Id in order to Instantiate them over the network.
/// </summary>
[CreateAssetMenu(fileName = "RTSForgeNetworking",
    menuName = "RTS/Scriptable Objects/PrefabManager", order = 1)]
public class PrefabManager : ScriptableObject, IPrefabManager
{
    [SerializeField]
    private NetworkEntity[] _prefabs = new NetworkEntity[0];

    public Transform GetPrefabById(int id)
    {
        return _prefabs.First(entity => entity.PrefabId == id).transform;
    }
}
