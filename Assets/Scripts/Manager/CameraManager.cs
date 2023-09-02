using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogError($"There is more than one CameraManager, {gameObject.name} was Destroyed");
            Destroy(this.gameObject);
        }
        else Instance = this;
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }
}
