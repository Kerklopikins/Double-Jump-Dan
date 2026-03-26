using UnityEngine;

public class UVScrolling : MonoBehaviour
{
    [SerializeField] Vector2 speed;
    [SerializeField] bool yPingPong;
    [SerializeField] Vector2 yMinMax;
    [SerializeField] float yPingPongLength;
    [SerializeField] bool useBumpMap;

    Vector2 offset;
    Material material;
    float xOffset;

	void Start() 
	{
        material = GetComponent<Renderer>().material;	
	}
	
	void Update() 
	{
        xOffset += speed.x * Time.deltaTime;

        if(yPingPong)
        {
            float t = Mathf.PingPong(Time.time * speed.y, yPingPongLength);
            float yOffset = Mathf.Lerp(yMinMax.x, yMinMax.y, t);
            offset = new Vector2(xOffset, yOffset);
        }
        else
        {
            offset = new Vector2(xOffset, 0);
        }

        if(xOffset >= 1)
            xOffset = 0;
        else if(xOffset <= -1)
            xOffset = 0;

        material.SetTextureOffset("_MainTex", offset);

        if(useBumpMap)
            material.SetTextureOffset("_BumpMap", offset);
	}
}