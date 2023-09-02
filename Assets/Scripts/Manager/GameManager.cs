using Unity.Netcode;
using UnityEngine;

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

    /*public void SpawnProjectile(GameObject projectile, Vector3 position, Quaternion rotation)
    {
        SpawnProjectileServerRpc(GetProjectileListSOIndex(projectile), position, rotation);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnProjectileServerRpc(int index, Vector3 position, Quaternion rotation)
    {
        GameObject projectile = GetProjectileObjectFromIndex(index);

        Instantiate(projectile, position, rotation);

        
    }*/

    private int GetProjectileListSOIndex(GameObject projectile)
    {
        return projectileListSO.projectileList.IndexOf(projectile);
    }

    private GameObject GetProjectileObjectFromIndex(int projectileListIndex)
    {
        return projectileListSO.projectileList[projectileListIndex];
    }
}
