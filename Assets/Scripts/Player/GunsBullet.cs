using UnityEngine;

public class GunsBullet : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] float hitDistance;
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform bulletHole;
    [Header("Bullet Core")]
    [SerializeField] Transform BulletCore;

    [Header("Bullet Body")]
    [SerializeField] Transform BulletBody;
    [SerializeField] Rigidbody bBodyRb;
    [SerializeField] float ejectBulletDistance = 2, destroyAfter = 3;


    Vector3 target;
    bool fired, reached;
    RaycastHit? hit;
    RaycastHit hitx;

    public void Fire(Vector3 target, Vector3 bulletEjectPoint, RaycastHit? hit)
    {
        if (hit != null) { hitx = (RaycastHit)hit; this.hit = (RaycastHit)hit; }
        this.target = target;
        fired = true;
        BulletBody.parent = null;
        BulletBody.position = bulletEjectPoint;
        bBodyRb.isKinematic = false;
        bBodyRb.AddForce((Vector3.up + transform.right).normalized * ejectBulletDistance * Time.deltaTime, ForceMode.Impulse);
        Destroy(BulletBody.gameObject, destroyAfter);
    }

    void ReachedToTarget()
    {
        if (hit != null)
        {
            RaycastHit hitx = (RaycastHit)hit;
            Transform hole = Instantiate(bulletHole, hitx.point + hitx.normal * 0.1f, Quaternion.LookRotation(hitx.normal));
            hole.parent = hitx.transform;
            BulletCore.localScale = Vector3.zero;
            Destroy(BulletCore.gameObject, 1);
            HitAction();
            Destroy(hole.gameObject, destroyAfter);
        }
        Destroy(gameObject, destroyAfter);
    }

    void HitAction()
    {
        switch (hitx.transform.gameObject.tag)
        {
            case "ScoreTarget":
                ScoreTarget st = hitx.transform.GetComponent<ScoreTarget>();
                st.Fall(hitx.point);
                break;
            case "ExplosiveTarget":
                ExplosiveTarget expT = hitx.transform.GetComponent<ExplosiveTarget>();
                expT.Explode();
                break;
        }
    }
    void Update()
    {
        if (!fired || reached) { return; }

        if (Vector3.Distance(BulletCore.position, target) < hitDistance)
        {
            reached = true;
            ReachedToTarget();
        }
        transform.position = Vector3.MoveTowards(transform.position, target, bulletSpeed * Time.deltaTime);
    }
}
