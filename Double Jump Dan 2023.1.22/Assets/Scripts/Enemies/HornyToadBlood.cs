using UnityEngine;

public class HornyToadBlood : MonoBehaviour
{
	Animator bloodSplatAnimator;

	void Start()
	{
		bloodSplatAnimator = GameObject.Find("Blood Splatter").GetComponent<Animator>();
	}

	void OnParticleCollision(GameObject other)
	{
		if(other.GetComponent<Player>() != null)
		{
			if(!bloodSplatAnimator.IsInTransition(0))
				bloodSplatAnimator.SetTrigger("Splat");
		}
	}
}