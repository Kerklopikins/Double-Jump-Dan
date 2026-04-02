using UnityEngine;
using UnityEditor;

public class MovingPlatform : MonoBehaviour
{
	[SerializeField] Vector2 topPointOffset;
	[SerializeField] Vector2[] points;
	[SerializeField] float verticalPointOffset;
	[SerializeField] float roundingYOffset;
    [SerializeField] float speed;
	[SerializeField] bool startOnlyWhenSteppedOn;
	[SerializeField] bool stopWhenReachedEnd;
	[SerializeField] bool loop;
	[SerializeField] float wobbleAmount;
	[SerializeField] float wobbleSpeed;
	[SerializeField] Transform platformTransform;

    //public Transform[] gears;
	[SerializeField] ParticleSystem smoke;
    [SerializeField] float idleEnginePitch;
    [SerializeField] float enginePitch;

    Vector2 currentPoint;
    int pointIndex;
    int direction = 1;
	AudioSource engine;
	Player player;
	bool canMove;
	BoxCollider2D _collider2D;
	float _wobbleAmount;
	SpriteRenderer platformSprite;

    void Start()
    {
		player = GameObject.FindWithTag("Player").GetComponent<Player>();
        engine = GetComponent<AudioSource>();
		engine.volume = GameManager.Instance.sfxVolume;
		platformSprite = platformTransform.GetComponent<SpriteRenderer>();
        currentPoint = points[0];
    }

	void Update()
	{
		if(Time.timeScale == 0)
			engine.volume = 0;
		else
			engine.volume = AudioManager.Instance.sfxVolumePercent;
	}

	void FixedUpdate()
    {
		if(canMove)
		{
			engine.pitch = enginePitch * Time.timeScale;
			transform.position = Vector2.MoveTowards(transform.position, currentPoint, speed * Time.deltaTime);

			_wobbleAmount = wobbleAmount;
        }
		else
		{
			engine.pitch = idleEnginePitch * Time.timeScale;
            _wobbleAmount = 0;
        }

        float angle = Mathf.Sin(Time.time * wobbleSpeed) * _wobbleAmount;
        platformTransform.rotation = Quaternion.Euler(0, 0, angle);

        if(!startOnlyWhenSteppedOn && !stopWhenReachedEnd)
		{
			canMove = true;
		}
		else
		{
			if(player.dead && startOnlyWhenSteppedOn || player.dead && stopWhenReachedEnd)
				canMove = false;
		}

		if(platformSprite.isVisible && Time.timeScale > 0)
			engine.enabled = true;
		else if (!platformSprite.isVisible || Time.timeScale == 0)
			engine.enabled = false;

		if(player.respawned && (Vector2)transform.position != points[points.Length - 1])
		{
			smoke.Clear();
			smoke.Play();
			transform.position = points[0];
			canMove = false;
			player.respawned = false;
		}

		if(startOnlyWhenSteppedOn)
		{
			if(!canMove)
				return;

            if((Vector2)transform.position == points[0] && !stopWhenReachedEnd && canMove && !transform.Find("Player"))
			{
				canMove = false;
				return;
			}

            if((Vector2)transform.position == currentPoint)
			{
				if(pointIndex == points.Length && stopWhenReachedEnd)
				{
					canMove = false;
					return;
				}

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
		else
		{
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
	}

    void OnCollisionEnter2D(Collision2D other)
    {
		if(stopWhenReachedEnd && (Vector2)transform.position == points[points.Length - 1])
			return;
            
        if(other.transform.CompareTag("Player"))
			canMove = true;

        if(other.transform.position.y > transform.position.y)
            other.transform.parent = transform;
    }

    void OnCollisionExit2D(Collision2D other)
    {
		if(other.transform.CompareTag("Player"))
        	other.transform.parent = null;
    }

	void OnValidate()
	{
		if(points == null)
			return;
		
		for(int i = 0; i < points.Length; i++)
		{
			points[i] = SnapVector(points[i]);

			if(points[i] == new Vector2(0, roundingYOffset))
				points[i] = new Vector2(transform.position.x, transform.position.y);
        }
    }

	private Vector2 SnapVector(Vector2 v)
	{
		return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y - roundingYOffset) + roundingYOffset);
	}

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if(_collider2D == null)
			_collider2D = GetComponent<BoxCollider2D>();
		
		for(int i = 0; i < points.Length; i++)
		{
			Gizmos.color = new Color(0, 1, 1, 0.5f);
			Gizmos.DrawCube(new Vector3(points[i].x, points[i].y + verticalPointOffset, 0), _collider2D.size);
			Gizmos.DrawCube(new Vector3(points[i].x + topPointOffset.x, points[i].y + verticalPointOffset + topPointOffset.y, 0), new Vector3(0.5f, 0.5f, 0.5f));
            
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
			Gizmos.DrawLine(new Vector3(points[i].x, points[i].y + verticalPointOffset, 0), new Vector3(points[i + 1].x, points[i + 1].y + verticalPointOffset, 0));
        }
		
		if(loop && points.Length > 0)
            Gizmos.DrawLine(new Vector3(points[points.Length - 1].x, points[points.Length - 1].y + verticalPointOffset, 0), new Vector3(points[0].x, points[0].y + verticalPointOffset, 0));
#endif

    }
}