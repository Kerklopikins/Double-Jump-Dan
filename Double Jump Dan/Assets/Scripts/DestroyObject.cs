using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] float lifeTime;

    void Update()
    {
        Destroy(gameObject, lifeTime);
    }
}