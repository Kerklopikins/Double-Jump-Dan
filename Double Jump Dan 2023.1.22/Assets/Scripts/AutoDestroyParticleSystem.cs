using UnityEngine;

public class AutoDestroyParticleSystem : MonoBehaviour
{
    ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(!_particleSystem.isPlaying)
        	Destroy(gameObject);
    }
}