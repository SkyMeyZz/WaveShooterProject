using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [Header("Movements")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashSpeedMultiplier;
    [SerializeField] private float dashTime;
    [SerializeField] private GameObject dashParticle;
    private Vector2 movement;
    private enum MovementState { normal, dodgeRolling };
    private MovementState currentMovementState;
    private bool canDodgeRoll;
    private Vector2 dodgeDir;
    private float dodgingTimer;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    private Vector3 mousePos;

    [Header("Weapon Handling")]
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootInterval;
    private bool canShoot;

    private Rigidbody2D rb;
    public PlayerScript localInstance { get; private set; }

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        currentMovementState = MovementState.normal;
        canDodgeRoll = true;
        canShoot = false;

    }
    public override void OnNetworkSpawn()
    {
        cam = CameraManager.Instance.GetMainCamera();
        if (!IsOwner) return;
        localInstance = this;
        canShoot = true;
    }

    private void Update()
    {
        if (!IsOwner) return;
        HandleInputs();
        HandlePlayerRotation();
    }


    private void FixedUpdate()
    {
        if (currentMovementState == MovementState.normal)
        {
            rb.position += movement.normalized * moveSpeed * Time.fixedDeltaTime;
        }
        else if (currentMovementState == MovementState.dodgeRolling)
        {
            DodgeRoll();
        }
    }

    private void HandleInputs()
    {
        if (currentMovementState != MovementState.dodgeRolling)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Space) && canDodgeRoll)
        {
            HandleDodgeRolling();
        }

        if (Input.GetKey(KeyCode.Mouse0) && canShoot)
        {
            Shoot();
        }

        /*if (Input.GetKeyDown(KeyCode.E))
        {
            IInteractable interactable = GetPlayerClosestInterraction();
            if (interactable == null) return;

            interactable.Interact(gameObject);
        }*/
    }

    private void HandlePlayerRotation()
    {
        Vector2 lookDir = mousePos - transform.position;
        lookDir = lookDir.normalized;
        float minOrientationRange = 1f;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        if (Vector2.Distance(transform.position, mousePos) > minOrientationRange)
        {
            rb.rotation = angle;
        }
        Debug.DrawRay(shootPoint.transform.position, shootPoint.transform.right * 10);
    }

    /*private IInteractable GetPlayerClosestInterraction()
    {
        List<IInteractable> interactablesList = new List<IInteractable>();
        float interactRange = 2f;
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (Collider2D collider in colliderArray)
        {
            if (collider.TryGetComponent(out IInteractable interactables))
            {
                interactablesList.Add(interactables);
            }
        }

        IInteractable closestInteractable = null;
        foreach (IInteractable interactables in interactablesList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactables;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactables.GetTransform().position) < Vector3.Distance(transform.position, closestInteractable.GetTransform().position))
                {
                    closestInteractable = interactables;
                }
            }
        }

        return closestInteractable;
    }*/

    private void HandleDodgeRolling()
    {
        GameObject smokeparticles = Instantiate(dashParticle, transform.position, transform.rotation);
        smokeparticles.GetComponent<NetworkObject>().Spawn(true);

        dodgeDir = movement;
        dodgingTimer = dashTime;
        currentMovementState = MovementState.dodgeRolling;
    }

    private void DodgeRoll()
    {
        rb.position += dodgeDir.normalized * moveSpeed * dashSpeedMultiplier * Time.deltaTime;
        dodgingTimer -= Time.deltaTime;
        if (dodgingTimer <= 0)
        {
            currentMovementState = MovementState.normal;
        }
    }

    private void Shoot()
    {
        canShoot = false;
        GameManager.Instance.SpawnProjectile(bulletPrefab);
        Invoke(nameof(ResetCanShoot), shootInterval);
    }

    public Vector3 GetMousePos()
    {
        return mousePos;
    }

    private void ResetCanShoot()
    {
        canShoot = true;
    }

}

