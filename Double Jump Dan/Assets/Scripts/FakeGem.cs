using UnityEngine;

public class FakeGem: MonoBehaviour, IPoolable
{
    public void OnObjectReuse(object data)
    {
        TransformProperties properties = (TransformProperties)data;
        transform.position = properties.position;
        transform.localScale = properties.scale;
        transform.rotation = properties.rotation;
    }
}