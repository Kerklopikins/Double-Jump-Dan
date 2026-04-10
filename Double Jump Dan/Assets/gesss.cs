
using UnityEngine;
using System.Collections.Generic;
public class gesss : MonoBehaviour
{
    public int rayCount;
    public float raySpacing;
    public float width;
    LineRenderer shadow;
    public List<Transform> shadowPieces = new List<Transform>();

    void Start()
    {
        shadow = GetComponent<LineRenderer>();
                raySpacing = width / rayCount;

    }

    void Update()
    {
        //shadow.positionCount = rayCount;

        for (int i = 0; i < rayCount; i++)
		{
			Vector2 rayVector = new Vector2(transform.position.x + (i * raySpacing), transform.position.y);
			Debug.DrawRay(rayVector, Vector2.down * 25, Color.red);
		
			RaycastHit2D hit = Physics2D.Raycast(rayVector, Vector2.down, 25, 1 << LayerMask.NameToLayer("Collisions"));

            shadowPieces[i].transform.position = hit.point;
        }
    }
}