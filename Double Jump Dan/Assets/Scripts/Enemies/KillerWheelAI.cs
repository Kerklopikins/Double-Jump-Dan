using UnityEngine;

public class KillerWheelAI : MonoBehaviour
{
    [SerializeField] float speed;

    Rigidbody2D rb2D;
	WatchOutTrigger watchOutTrigger;

	void Start() 
	{
        rb2D = GetComponent<Rigidbody2D>();
		watchOutTrigger = GetComponentInChildren<WatchOutTrigger>();
		watchOutTrigger.Activate();
    }
	
	void Update() 
	{
        rb2D.angularVelocity = speed;
	}
}