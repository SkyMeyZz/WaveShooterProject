using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeleteAfterTime : NetworkBehaviour
{
    public float time;

    public bool isANetworkObject;

    public override void OnNetworkSpawn()
    {
        if(isANetworkObject)
        {
            Invoke(nameof(DestroyOnNetwork), time);
        }
        else Destroy(this.gameObject, time);
    }

    private void DestroyOnNetwork()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}
