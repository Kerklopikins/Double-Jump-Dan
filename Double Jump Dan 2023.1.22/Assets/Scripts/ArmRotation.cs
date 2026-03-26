using UnityEngine;

public class ArmRotation : MonoBehaviour
{
    [SerializeField] float rotationOffset;
    [SerializeField] Camera orthoCamera;

    Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    void Update()
    {    
        if(!player.finishedLevel && Time.timeScale != 0)
            HandleInput();    
    }

    void HandleInput()
    {
        Vector3 difference = orthoCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
            
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;    
        transform.rotation = Quaternion.Euler(0, 0, rotZ + rotationOffset);
    }
}