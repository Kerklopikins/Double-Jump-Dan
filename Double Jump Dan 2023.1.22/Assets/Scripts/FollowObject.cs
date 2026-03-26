using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Vector3 Offset;
    [SerializeField] Transform Following;

    void Update()
    {
        transform.position = Following.transform.position + (Vector3)Offset;
    }
}