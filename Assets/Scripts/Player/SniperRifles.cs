using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class SniperRifles : Guns
{
    [Header("Sniper Attributes")]
    [SerializeField] CinemachineCamera scopeCam;
    [SerializeField] int priorityIncreaser = 5;
    [SerializeField] TextMeshProUGUI distance, lastHit;
    new void Start()
    {
        lastHit.text = "";
        base.Start();
    }

    new void Update()
    {
        base.Update();
        if (CanUse)
        {
            RaycastHit hit;
            if (Physics.Raycast(NuzzlePoint.position, NuzzlePoint.forward, out hit, maxRange))
            {
                distance.text = hit.distance.ToString("F1") + "m";
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    lastHit.text = hit.distance.ToString("F1") + "m";
                }
            }
            else
            {
                distance.text = "";
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                scopeCam.Priority += priorityIncreaser;
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                scopeCam.Priority = 0;
                lastHit.text = "";
            }
        }
        else
        {
            if (distance != null)
            {
                distance.text = "";
            }
        }
    }
}
