using System;
using System.Collections;
using UnityEngine;

public class ScoreTarget : MonoBehaviour
{
    [SerializeField] Transform body;
    [SerializeField] Quaternion droppedAngle, standingAngle;
    [SerializeField] AudioSource[] metalHitSfxs;
    [SerializeField] AudioSource readyToShotSfx;
    [SerializeField] float fallingSpeed = 20, autoGetUpCoolDown;
    [SerializeField] bool falling, gettingUp, touchable = true, autoGetUp = true;
    [SerializeField] Transform centerPoint; // point center for tracking how far bullet hit from center point
    [SerializeField] Transform[] Type1ScorePoints;
    public void Fall(Vector3 hitpoint)
    {
        if (!touchable) { return; }
        falling = true;
        touchable = false;

        int randomHitSfx = UnityEngine.Random.Range(0, metalHitSfxs.Length);
        metalHitSfxs[randomHitSfx].Play();

        Debug.DrawLine(centerPoint.position, hitpoint, Color.green, Mathf.Infinity);
        float dist = Vector2.Distance(centerPoint.position, hitpoint);
        for (int i = 0; i < Type1ScorePoints.Length; i++)
        {
            float spDist = Vector2.Distance(centerPoint.position, Type1ScorePoints[i].position);
            if (dist < spDist)
            {
                Debug.Log("Dist :" + dist + " & index :" + i);
                break;
            }
        }
    }

    public void GetUp()
    {
        falling = false;
        gettingUp = true;
        //readyToShotSfx.Play();
    }

    void Update()
    {
        if (falling)
        {
            body.localRotation = Quaternion.Lerp(body.localRotation, droppedAngle, fallingSpeed * Time.deltaTime);
            if (body.localRotation == droppedAngle)
            {
                falling = false;
                if (autoGetUp)
                {
                    StartCoroutine(autoGetup());
                }
            }
        }
        if (gettingUp)
        {
            body.localRotation = Quaternion.Lerp(body.localRotation, standingAngle, fallingSpeed * Time.deltaTime);
            if (body.localRotation == standingAngle)
            {
                gettingUp = false;
                touchable = true;
            }
        }
    }

    IEnumerator autoGetup()
    {
        yield return new WaitForSeconds(autoGetUpCoolDown);
        GetUp();
    }
}
