using UnityEngine;
public class ProjectileTrajectory : MonoBehaviour
{
    [SerializeField] float curveLength = 5f;
    [SerializeField] Transform endPointSprite;

    Vector2[] segmants;
    float lineScrollSpeed = 2;
    LineRenderer lineRenderer;
    const float timeCurveAddition = 0.5f;
    float lineTextureOffset;
    int segmantCount = 15;
    Material material;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        material = lineRenderer.material;
        segmants = new Vector2[segmantCount];
        lineRenderer.positionCount = segmantCount;
    }

    public void EnableTrajectoryLine(Vector2 startPoint, float projectileForce, float projectileForceMultiplier)
    {
        if(segmants == null || material == null || lineRenderer == null)
            return;

        lineRenderer.enabled = true;
        lineTextureOffset -= lineScrollSpeed * Time.deltaTime;

        if(lineTextureOffset >= 1)
                lineTextureOffset = 0;

        material.SetTextureOffset("_MainTex", Vector2.right * lineTextureOffset);

        segmants[0] = startPoint;
        lineRenderer.SetPosition(0, startPoint);

        Vector2 startVelocity = transform.right * projectileForce * projectileForceMultiplier * transform.lossyScale.x;

        for(int i = 0; i < segmantCount; i ++)
        {
            float timeOffset = (i * Time.fixedDeltaTime * curveLength);
            Vector2 gravityOffset = timeCurveAddition * Physics2D.gravity * Mathf.Pow(timeOffset, 2);
            segmants[i] = segmants[0] + startVelocity * timeOffset + gravityOffset;
            lineRenderer.SetPosition(i, segmants[i]);
        }
        
        endPointSprite.gameObject.SetActive(true);
        endPointSprite.transform.position = segmants[segmants.Length - 1];
        endPointSprite.rotation = Quaternion.identity;
    }

    public void DisableTrajectoryLine()
    {
        if(lineRenderer != null)
            lineRenderer.enabled = false;
        
        endPointSprite.gameObject.SetActive(false);
    }
}