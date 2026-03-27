using UnityEngine;
using UnityEditor;

public class Pendulum : MonoBehaviour
{   
    [SerializeField] float speed;
    [Range(2, 30)]
    [SerializeField] int length;
    [SerializeField] float phaseOffset;
    [SerializeField] float startRotation;
    [SerializeField] float endRotation;
	[SerializeField] SpriteRenderer middleSegmant;
    [SerializeField] Transform endSpike;
    [SerializeField] BoxCollider2D spikeCollider;

    void Start()
    {
        middleSegmant.size = new Vector2(middleSegmant.size.x, length);
        endSpike.transform.localPosition = new Vector2(0, -length);

		spikeCollider.size = new Vector2(spikeCollider.size.x, middleSegmant.size.y + 2);
		spikeCollider.offset = new Vector2(0, -middleSegmant.size.y / 2 - 1);
    }

    void OnValidate()
    {
        length = (int)Mathf.Round(length / 2) * 2;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * speed + phaseOffset) * 0.5f + 0.5f;
        float angle = Mathf.Lerp(startRotation, endRotation, t);
        middleSegmant.transform.localEulerAngles = new Vector3(0, 0, angle);
	}

    void OnDrawGizmos()
    {
        int _length = length + 4;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - 1), new Vector3(6, 2, 1));
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Vector3 offset = new Vector3(0, -_length * 0.5f, 0);
        Gizmos.DrawCube(offset, new Vector3(0.75f, _length, 1));

        Gizmos.DrawCube(new Vector3(0, -_length + 1, 0), new Vector3(6, 2, 1));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(0, 0, startRotation), Vector3.one);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Vector3 offset1 = new Vector3(0, -_length * 0.5f, 0);
        Gizmos.DrawCube(offset1, new Vector3(0.75f, _length, 1));

        Gizmos.DrawCube(new Vector3(0, -_length + 1, 0), new Vector3(6, 2, 1));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(0, 0, endRotation), Vector3.one);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Vector3 offset2 = new Vector3(0, -_length * 0.5f, 0);
        Gizmos.DrawCube(offset2, new Vector3(0.75f, _length, 1));

        Gizmos.DrawCube(new Vector3(0, -_length + 1, 0), new Vector3(6, 2, 1));
    }
}