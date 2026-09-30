using Unity.Cinemachine;
using UnityEngine;

public class PlaneFlightManager : MonoBehaviour
{
    [SerializeField] float spawnRadius, altitude, disablingDistance;
    [SerializeField] Transform mapCenter;
    [SerializeField] GameObject planeTransform;
    [SerializeField] float flightSpeed = 10f;

    [SerializeField] float zoomSpeed, maxZoom, minZoom, zoomSmoothness;
    float targetFov;

    [SerializeField] CinemachineCamera planeCam;

    [SerializeField] Rigidbody rb;
    [SerializeField] bool isFlying = false;

    [SerializeField] GameObject blinkerLight,doorSound;
    [SerializeField] Animation anims;
    [SerializeField] bool isDoorOpen;

    [SerializeField] Parachute player;
    [SerializeField] bool playerJumped;

    void Start()
    {
        targetFov = planeCam.Lens.FieldOfView;
        blinkerLight.SetActive(false);
        RelocatePlane();
    }

    void RelocatePlane()
    {
        if (mapCenter == null || planeTransform == null || rb == null) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * spawnRadius;

        Vector3 randomPosition = new Vector3(
            mapCenter.position.x + randomCirclePoint.x,
            altitude,
            mapCenter.position.z + randomCirclePoint.y
        );

        planeTransform.transform.position = randomPosition;

        Vector3 lookTarget = new Vector3(mapCenter.position.x, randomPosition.y, mapCenter.position.z);
        planeTransform.transform.LookAt(lookTarget);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDoorOpen)
        {
            isDoorOpen = true;
            anims.Play("OpenDoor");
            blinkerLight.SetActive(true);
            doorSound.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.E) && isDoorOpen && !playerJumped)
        {
            planeCam.gameObject.SetActive(false);
            player.transform.SetParent(null, true);
            player.Jump();
            playerJumped = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isFlying = false;
            RelocatePlane();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlying = !isFlying;
            if (!isFlying) { rb.linearVelocity = Vector3.zero; }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetFov = Mathf.Clamp(targetFov - (scroll * zoomSpeed * 10f), minZoom, maxZoom);
        }

        planeCam.Lens.FieldOfView = Mathf.Lerp(planeCam.Lens.FieldOfView, targetFov, zoomSmoothness * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (isFlying && rb != null)
        {
            rb.linearVelocity = planeTransform.transform.forward * flightSpeed;
            if (Vector3.Distance(mapCenter.position, transform.position) >= disablingDistance)
            {
                gameObject.SetActive(false);
            }
        }
    }
}