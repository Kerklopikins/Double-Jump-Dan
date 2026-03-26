using UnityEngine;
using System.Collections;

public class SpikeODeath : MonoBehaviour
{   
    [SerializeField] float movementTime;
    [Range(2, 100)]
    [SerializeField] int length;
    [SerializeField] float delay;
    [SerializeField] float startDelay;
	[SerializeField] SpriteRenderer bottomSegmant;
    [SerializeField] BoxCollider2D spikeCollider;

    Vector3 localStartPosition;
    Vector3 globalStartPosition;

    float speed;
    float percent;
    int direction = -1;

    void Start()
    {
		localStartPosition = transform.localPosition;
        globalStartPosition = transform.position;

        if(length > 2)
        {
			bottomSegmant.gameObject.SetActive(true);
            bottomSegmant.size = new Vector2(bottomSegmant.size.x, length - 2);
		}
		else
		{
            bottomSegmant.gameObject.SetActive(false);

        }

		spikeCollider.size = new Vector2(spikeCollider.size.x, bottomSegmant.size.y);
		spikeCollider.offset = new Vector2(0, -bottomSegmant.size.y / 2);

        speed = 1 / movementTime;
        
		StartCoroutine(Move());
    }

    void OnValidate()
    {
        length = (int)Mathf.Round(length / 2) * 2;
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(startDelay);

		while(true)
		{
			percent += Time.deltaTime * speed * direction;
			transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(localStartPosition.y, length + localStartPosition.y, percent), transform.localPosition.z);

			if(percent >= 1)
			{
				direction = 0;
				yield return new WaitForSeconds(delay);
				direction = -1;
			}
			else if(percent <= 0)
			{
				direction = 0;
				yield return new WaitForSeconds(delay);
				direction = 1;
			}

			percent = Mathf.Clamp(percent, 0, 1);

			yield return null;
		}
	}

    void OnDrawGizmos()
    {
        int _length = length + 2;
        

        if(Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(globalStartPosition, Vector3.one * 2);
            Gizmos.matrix = Matrix4x4.TRS(globalStartPosition, transform.rotation, Vector3.one);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 2);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        }

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Vector3 offset = new Vector3(0, _length * 0.5f - 1, 0);
        Gizmos.DrawCube(offset, new Vector3(1, _length, 1));
    }
}