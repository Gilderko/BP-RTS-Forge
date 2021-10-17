using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Needs to be implemented with the Entity stuff from Forge
public class NetworkBehaviour : MonoBehaviour
{
    public bool IsOwner { get; set; }

    public bool IsServer { get; set; }

    public bool IsClient { get; set; }

    public bool IsLocalPlayer { get; set; }

    public ulong OwnerClientId { get; set; }
}
