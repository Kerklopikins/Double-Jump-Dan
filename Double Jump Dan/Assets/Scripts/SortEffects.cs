using UnityEngine;

[ExecuteInEditMode]
public class SortEffects : MonoBehaviour
{
    [SerializeField] string LayerName;
    [SerializeField] int orderInLayer;

    TrailRenderer _trailRenderer;
    LineRenderer _lineRenderer;

    void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();

        if(_trailRenderer != null)
        {
            _trailRenderer.sortingLayerName = LayerName;
            _trailRenderer.sortingOrder = orderInLayer;
        }

        if(_lineRenderer != null)
        {
            _lineRenderer.sortingLayerName = LayerName;
            _lineRenderer.sortingOrder = orderInLayer;
        }
    }
}