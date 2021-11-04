using Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
