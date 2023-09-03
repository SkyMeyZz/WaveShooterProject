using Unity.Netcode;
using UnityEngine;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ProjectileListSO projectileListSO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There is more than one GameManager, {gameObject.name} was Destroyed");
            Destroy(this.gameObject);
        }
        else Instance = this;
    }

    public void SpawnProjectile(GameObject projectile)
    {
        SpawnProjectileServerRpc(GetProjectileListSOIndex(projectile));
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnProjectileServerRpc(int projectileIndex)
    {
        GameObject projectile = Instantiate(GetProjectileObjectFromIndex(projectileIndex));
        NetworkObject projectileNetworkObject = projectile.GetComponent<NetworkObject>();
        projectileNetworkObject.Spawn(true);
    }

    public void DestroyProjectile(GameObject projectile)
    {
        DestroyProjectileServerRpc(projectile.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc(NetworkBehaviourReference projectileNetworkBehaviourReference)
    {
        projectileNetworkBehaviourReference.TryGet(out NetworkObject projectileNetworkObject);
        
    }

    private int GetProjectileListSOIndex(GameObject projectile)
    {
        return projectileListSO.projectileList.IndexOf(projectile);
    }

    private GameObject GetProjectileObjectFromIndex(int projectileListIndex)
    {
        return projectileListSO.projectileList[projectileListIndex];
    }
}
