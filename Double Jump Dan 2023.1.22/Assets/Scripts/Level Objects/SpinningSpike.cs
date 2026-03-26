using UnityEngine;

public class SpinningSpike : MonoBehaviour
{
    [SerializeField] float speed;

	void Update() 
	{
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
	}
}