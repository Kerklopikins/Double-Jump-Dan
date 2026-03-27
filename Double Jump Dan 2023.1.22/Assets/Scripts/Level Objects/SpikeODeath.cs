using UnityEngine;
using System.Collections;

public class SpikeODeath : MonoBehaviour
{   
    [SerializeField] float movementTime;
    [Range(2, 30)]
    [SerializeField] int length;
    [SerializeField] float pauseDuration;
    [SerializeField] float startDelay;
	[SerializeField] SpriteRenderer bottomSegmant;
    [SerializeField] BoxCollider2D spikeCollider;

    Vector3 localStartPosition;
    Vector3 globalStartPosition;

    float speed;
    float percent;
    int direction = -1;
    float pauseTimer;
    bool pausing;
    float rawT;


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
    }

    void OnValidate()
    {
        length = (int)Mathf.Round(length / 2) * 2;
    }

    void Update()
    {
        if(startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }
        
        if(pausing)
        {
            pauseTimer += Time.deltaTime;

            if(pauseTimer > pauseDuration)
            {
                pausing = false;
                pauseTimer = 0;
            }

            return;
        }

        rawT += (Time.deltaTime / movementTime) * direction;

        if(rawT > 1)
        {
            rawT = 1;
            direction = -1;
            pausing = true;
        }
        else if(rawT < 0)
        {
            rawT = 0;
            direction = 1;
            pausing = true;
        }

        float t = rawT * rawT * (3 - 2 * rawT);
        float positionY = Mathf.Lerp(localStartPosition.y, length + localStartPosition.y, t);


        transform.localPosition = new Vector3(transform.localPosition.x, positionY, transform.localPosition.z);

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