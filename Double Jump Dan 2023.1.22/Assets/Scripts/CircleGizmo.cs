using UnityEngine;

public class CircleGizmo : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] Color circleColor = new Color(0, 0, 1, 0.25f);

    void OnDrawGizmos()
    {
        Gizmos.color = circleColor;
        Gizmos.DrawSphere(transform.position, radius);
    }
}