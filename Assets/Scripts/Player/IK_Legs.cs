using UnityEngine;

public class IK_Legs : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    [SerializeField] IK_Legs otherFoot = default;
    [SerializeField] float speed = 5;
    [SerializeField] float stepDistance = 1;
    [SerializeField] float stepLength = 1;
    [SerializeField] float stepHeight = 0.5f;
    [SerializeField] Vector3 footOffset = default;
    
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Quaternion oldRotation, currentRotation, newRotation;
    float lerp;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentRotation = newRotation = oldRotation = transform.rotation;
        lerp = 1;
    }

    void Update()
    {
        transform.position = currentPosition;
        transform.rotation = currentRotation;

        Vector3 localSpaceOffset = new Vector3(footSpacing, 0f, 0f);
        Vector3 rayOrigin = body.position + body.TransformDirection(localSpaceOffset);
        
        Ray ray = new Ray(rayOrigin + Vector3.up * 2f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer))
        {
            if ((Vector3.Distance(newPosition, info.point) > stepDistance || Vector3.Distance(transform.position, info.point) > stepDistance * 2f) && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                oldPosition = currentPosition;
                oldRotation = currentRotation;
                
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                Vector3 rotatedOffset = body.TransformDirection(footOffset);
                newPosition = info.point + (body.forward * stepLength * direction) + rotatedOffset;

                Quaternion targetRot = Quaternion.LookRotation(body.forward, info.normal);
                newRotation = targetRot;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentRotation = Quaternion.Slerp(oldRotation, newRotation, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldRotation = newRotation;
        }
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}