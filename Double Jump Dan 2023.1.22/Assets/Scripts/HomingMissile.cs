using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float rotationOffset;
    [SerializeField] bool invincible;
    [SerializeField] int gemsToGivePlayer;
    [SerializeField] GameObject destroyedEffect;
    [SerializeField] AudioClip destroySound;

    Player player;

    void Start()
    {
        print("Need player detector! Don't forget Riley.");
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        Vector3 difference = player.transform.position - transform.position;
        difference.Normalize();

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ + rotationOffset);
    }

    void DestroyProjectile()
    {
        if(invincible == false)
        {
            if(destroyedEffect != null)
                Instantiate(destroyedEffect, transform.position, transform.rotation);

            AudioManager.Instance.PlaySound2D(destroySound);

            Destroy(gameObject);
        }
    }
}