using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class Parachute : MonoBehaviour
{
    [SerializeField] GameObject pchuteUI;
    [SerializeField] TextMeshProUGUI altitudeText;
    [SerializeField] GameObject playerHolder;
    [SerializeField] bool Jumped, deployed;
    [SerializeField] CharacterController ctrl;
    [SerializeField] bool CanMove;
    [SerializeField] float maxRollAngle = 20f, maxFallPitchAngle = 60f, maxPchutePitchAngle = 20, tiltSpeed = 5f, movementSpeed = 10f;

    [SerializeField] Transform parachuteObject, pcht_Follows;
    [SerializeField] Animation anims;
    [SerializeField] KeyCode deployingKey;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Movement player;
    [SerializeField] float rollbackDistance = 10, minDeployAltitude = 100;

    bool keyup = true, MinAltitudeDeploy = false;
    Vector3 velocity;
    [SerializeField] float gravity, defaultGravity, pchuteGravity;

    [SerializeField] CinemachineCamera playerFpsCam;
    void Start()
    {
        pchuteUI.SetActive(false);
        player.controller.enabled = false;
        ctrl.enabled = false;
        defaultGravity = gravity;
        player.CanMove = false;
        parachuteObject.localScale = Vector3.zero;
        parachuteObject.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!Jumped) { return; }

        bool isDeployingPressed = Input.GetKey(KeyCode.Space);
        bool isDeployingReleased = Input.GetKeyUp(KeyCode.Space);

        if (isDeployingPressed && keyup && !deployed && !ctrl.isGrounded)
        {
            StartCoroutine(ieDeploying());
        }

        if (isDeployingReleased)
        {
            keyup = true;
        }

        if (CanMove)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            float targetYAngle = player.cam.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(z * (deployed ? maxPchutePitchAngle : maxFallPitchAngle), targetYAngle, -x * maxRollAngle);

            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * tiltSpeed
            );
            Vector3 move = transform.right * x + transform.forward * z;
            ctrl.Move(move * movementSpeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        ctrl.Move(velocity * Time.deltaTime);

        if (!MinAltitudeDeploy && isDeployingPressed && keyup && deployed)
        {
            StartCoroutine(ieRollingBack());
        }

        RaycastHit altitudeCalc;
        if (Physics.Raycast(ctrl.transform.position, Vector3.down, out altitudeCalc, Mathf.Infinity, groundLayer))
        {
            float altitCalc = Vector3.Distance(ctrl.transform.position, altitudeCalc.point);
            if (!MinAltitudeDeploy) { altitudeText.text = "Altitude " + (int)altitCalc; }
            if (!MinAltitudeDeploy && altitCalc <= minDeployAltitude && !deployed)
            {
                MinAltitudeDeploy = true;
                altitudeText.text = "Emergency Deployment";
                StopAllCoroutines();
                anims.Stop();
                StartCoroutine(ieDeploying());
            }

            if (altitCalc < rollbackDistance)
            {
                StartCoroutine(ieRollingBack());
                transform.localRotation = Quaternion.identity;
                this.enabled = false;
                playerHolder.transform.SetParent(null, true);
                pchuteUI.SetActive(false);
                player.CanMove = true;
                ctrl.enabled = false;
                player.controller.enabled = true;
                playerFpsCam.Priority += 1;
                Destroy(gameObject);
            }
        }
    }

    IEnumerator ieDeploying()
    {
        velocity.y = pchuteGravity * Time.deltaTime;
        gravity = pchuteGravity;
        keyup = false;
        parachuteObject.gameObject.SetActive(true);
        anims.Play("ParachuteDeploy");
        yield return new WaitForSeconds(anims.GetClip("ParachuteDeploy").length);
        deployed = true;
    }

    IEnumerator ieRollingBack()
    {
        gravity = defaultGravity;
        keyup = false;
        deployed = false;
        anims.Play("ParachuteRollback");
        yield return new WaitForSeconds(anims.GetClip("ParachuteRollback").length);
        parachuteObject.gameObject.SetActive(false);
    }

    public void Jump()
    {
        pchuteUI.SetActive(true);
        player.CanMove = false;
        ctrl.enabled = true;
        Jumped = true;
        player.cam.Priority += 1;
        CanMove = true;
    }
}