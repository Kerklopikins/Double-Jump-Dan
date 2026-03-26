using UnityEngine;
using UnityEditor;

public class BuzzSaw : MonoBehaviour
{
    [SerializeField] Vector2[] points;
    [SerializeField] float speed = 1;
    [SerializeField] bool loop;

    Vector2 currentPoint;
    int pointIndex;
    int direction = 1;

    void Start()
    {
        currentPoint = points[0];
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentPoint, speed * Time.deltaTime);
        transform.Rotate(Vector3.forward, -1000 * Time.deltaTime);

        if((Vector2)transform.position == currentPoint)
        {
            if(loop)
            {
                if(pointIndex == points.Length)
                    pointIndex = -1;
            }
            else
            {
                if(pointIndex == points.Length)
                    direction = -1;
                else if(pointIndex <= 0)
                    direction = 1;
            }

            pointIndex += direction;

            if(pointIndex > points.Length - 1)
                return;

            currentPoint = points[pointIndex];
        }
    }
   
    void OnValidate()
    {
        if(points == null)
            return;

        for(int i = 0; i < points.Length; i++)
        {
            points[i] = SnapVector(points[i]);

            if(points[i] == Vector2.zero)
                points[i] = new Vector2(transform.position.x, transform.position.y);
        }
    }

    private Vector2 SnapVector(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        for(int i = 0; i < points.Length; i++)
        {
            Gizmos.color = new Color(0, 1, 1, 0.5f);
            Gizmos.DrawCube(new Vector3(points[i].x, points[i].y, 0), Vector3.one * 2);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 10;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;

            int pointString = i + 1;
            Handles.Label(points[i], "Point (" + pointString + ")", style);
        }

        for(int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(new Vector3(points[i].x, points[i].y, 0), new Vector3(points[i + 1].x, points[i + 1].y, 0));
        }

        if(loop && points.Length > 0)
            Gizmos.DrawLine(new Vector3(points[points.Length - 1].x, points[points.Length - 1].y, 0), new Vector3(points[0].x, points[0].y, 0));
#endif

    }
}