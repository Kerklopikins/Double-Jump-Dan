using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(OneWayPlatform))]
public class MovingPlatform : MonoBehaviour
{
	[Header("Main Stuff")]
	[Range(0, 3)]
	[SerializeField] int platformWidth;
	[SerializeField] List<Vector2> points = new List<Vector2>();
    [SerializeField] float speed;

	[Header("Platform Behaviour")]
	[SerializeField] bool startOnlyWhenSteppedOn;
	[SerializeField] bool stopWhenReachedEnd;
	[SerializeField] bool loop;

	[Header("Wobble")]
	[SerializeField] float wobbleAmount;
	[SerializeField] float wobbleSpeed;
	[SerializeField] Transform platformTransform;

	[Header("Offsets")]
	[SerializeField] float verticalPointOffset;
	[SerializeField] float roundingYOffset;

	[Header("Effects")]
	[SerializeField] ParticleSystem smoke;
    [SerializeField] float idleEnginePitch;
    [SerializeField] float enginePitch;

	[Header("Components")]
	[SerializeField] Sprite[] platformSprites;
	[SerializeField] Sprite[] engineSprites;
	[SerializeField] SpriteRenderer platformSprite;
	[SerializeField] SpriteRenderer engineSprite;
	[SerializeField] BoxCollider2D _collider2D;

    Vector2 currentPoint;
    int pointIndex;
    int direction = 1;
	AudioSource engine;
	Player player;
	bool canMove;
	float _wobbleAmount;
	bool isQuitting;

    void Start()
    {
		player = GameObject.FindWithTag("Player").GetComponent<Player>();
        engine = GetComponent<AudioSource>();
		engine.volume = GameManager.Instance.sfxVolume;
        currentPoint = points[0];

		player.OnPlayerRespawn += PlayerRespawn;

		SetSize();
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
				if(pointIndex == points.Count && stopWhenReachedEnd)
				{
					canMove = false;
					return;
				}

                if(loop)
                {
                    if(pointIndex == points.Count)
                        pointIndex = -1;
                }
                else
                {
                    if(pointIndex == points.Count)
                        direction = -1;
                    else if(pointIndex <= 0)
                        direction = 1;
                }

                pointIndex += direction;

				if(pointIndex > points.Count - 1)
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
					if(pointIndex == points.Count)
						pointIndex = -1;
                }
                else
                {
                    if(pointIndex == points.Count)
                        direction = -1;
                    else if(pointIndex <= 0)
                        direction = 1;
                }

                pointIndex += direction;

				if(pointIndex > points.Count - 1)
					return;

				currentPoint = points[pointIndex];
			}
		}
	}

    void OnCollisionEnter2D(Collision2D other)
    {
		if(stopWhenReachedEnd && (Vector2)transform.position == points[points.Count - 1])
			return;
            
        if(other.transform.CompareTag("Player"))
			canMove = true;

        if(other.transform.position.y > transform.position.y)
            SetParent(other.transform, transform);
    }

    void OnCollisionExit2D(Collision2D other)
    {
		if(other.transform.CompareTag("Player"))
        	SetParent(other.transform, null);
    }

	void SetParent(Transform other, Transform parent)
	{
		if(!isQuitting)
			other.transform.parent = parent;
	}

	void PlayerRespawn()
	{
		if((Vector2)transform.position != points[points.Count - 1])
		{
			smoke.Clear();
			smoke.Play();
			transform.position = points[0];
			canMove = false;
		}
	}
	
	void OnValidate()
	{
#if UNITY_EDITOR
		EditorApplication.delayCall += SetSize;

		if(points == null)
			return;
			
		for(int i = 0; i < points.Count; i++)
		{
			if(!Application.isPlaying && i == 0)
            	points[0] = transform.position;
		
			points[i] = SnapVector(points[i]);

			if(points[i] == new Vector2(0, roundingYOffset))
				points[i] = new Vector2(transform.position.x, transform.position.y);
        }
#endif
    }

	private Vector2 SnapVector(Vector2 v)
	{
		return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y - roundingYOffset) + roundingYOffset);
	}

	void SetSize()
	{
		if(platformSprites.Length <= 0)
			return;
		
		platformSprite.sprite = platformSprites[platformWidth];
		_collider2D.size = new Vector2(platformSprite.bounds.size.x, _collider2D.size.y);
		
		if(platformWidth == 0)
			engineSprite.sprite = engineSprites[1];
		else
			engineSprite.sprite = engineSprites[0];
	}

    void OnApplicationQuit()
    {
		isQuitting = true;
    }

    void OnDisable()
    {
        isQuitting = true;
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
		
		for(int i = 0; i < points.Count; i++)
		{
			Gizmos.color = new Color(0, 1, 1, 0.5f);
			Gizmos.DrawCube(new Vector3(points[i].x, points[i].y + verticalPointOffset, 0), _collider2D.size);
            Gizmos.DrawCube(new Vector3(points[i].x, points[i].y - 0.25f, 0), Vector3.one * 2);
			
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
            style.fontSize = 10;
			style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;

			if(i == 0)
                Handles.Label(points[i] + Vector2.up * 2, "Start Point", style);
            else if(i == points.Count - 1)
                Handles.Label(points[i] + Vector2.up * 2, "End Point", style);
            else
                Handles.Label(points[i] + Vector2.up * 2, "Point (" + i + ")", style);
        }

        for(int i = 0; i < points.Count - 1; i++)
			Gizmos.DrawLine(new Vector3(points[i].x, points[i].y + verticalPointOffset, 0), new Vector3(points[i + 1].x, points[i + 1].y + verticalPointOffset, 0));
		
		if(loop && points.Count > 0)
            Gizmos.DrawLine(new Vector3(points[points.Count - 1].x, points[points.Count - 1].y + verticalPointOffset, 0), new Vector3(points[0].x, points[0].y + verticalPointOffset, 0));
#endif

    }
}