using UnityEngine;

public struct ProjectileProperties
{
    public float speed;
    public int damage;
    public float lifeTime;
    public Vector2 position;
    public Quaternion rotation;
    public Vector2 scale;
    public bool useRaycast;
    public Vector2 targetPoint;
    public Vector2 direction;
    public string destroyedEffectPool;
}