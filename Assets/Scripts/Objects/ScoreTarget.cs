using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreTarget : MonoBehaviour
{
    [SerializeField] Transform body;
    [SerializeField] Quaternion droppedAngle, standingAngle;
    [SerializeField] AudioSource[] metalHitSfxs;
    [SerializeField] AudioSource readyToShotSfx;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Animation scoreAnim;
    [SerializeField] int fallAfterHit = 1;
    private int defaultFallAfterHit = 0;
    [SerializeField] float fallingSpeed = 20, autoGetUpCoolDown;
    [SerializeField] bool falling, gettingUp, touchable = true, AutoGetUp = true;//Auto get up after delay
    [SerializeField] Transform centerPoint; // point center for tracking how far bullet hit from center point
    [SerializeField] Transform[] Type1ScorePoints;

    void Start()
    {
        defaultFallAfterHit = fallAfterHit;
    }

    public void Fall(Vector3 hitpoint)
    {
        bool isBehind = Vector3.Dot(transform.forward, (hitpoint - transform.position).normalized) < 0;
        if (!touchable || !isBehind) { return; }
        if (fallAfterHit-- <= 0)
        {
            falling = true;
            touchable = false;
        }

        int randomHitSfx = UnityEngine.Random.Range(0, metalHitSfxs.Length);
        metalHitSfxs[randomHitSfx].Play();

        Debug.DrawLine(centerPoint.position, hitpoint, Color.green, 5);
        float dist = Vector2.Distance(centerPoint.position, hitpoint);
        int score = 0;
        for (int i = 0; i < Type1ScorePoints.Length; i++)
        {
            float spDist = Vector3.Distance(centerPoint.position, Type1ScorePoints[i].position);
            if (dist < spDist)
            {
                score = Type1ScorePoints.Length * 3 + 1 - (i + 1) * 3 + 1;
                break;
            }
        }
        scoreText.text = score.ToString();
        scoreText.transform.parent.LookAt(Camera.main.transform);
        scoreText.transform.parent.Rotate(0, 180, 0);
        scoreAnim.Play();
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
                if (AutoGetUp)
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
                fallAfterHit = defaultFallAfterHit;
            }
        }
    }

    IEnumerator autoGetup()
    {
        yield return new WaitForSeconds(autoGetUpCoolDown);
        GetUp();
    }
}
