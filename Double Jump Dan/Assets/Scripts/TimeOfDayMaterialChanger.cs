using UnityEngine;

public class TimeOfDayMaterialChanger : MonoBehaviour 
{
    WorldManager worldManager;
    MeshRenderer meshRenderer;
    SpriteRenderer spriteRenderer;
    ParticleSystemRenderer particleSystemRenderer;
 
	void Start() 
	{
		worldManager = WorldManager.Instance;
		UpdateMaterialColor();
	}

	public void UpdateMaterialColor()
	{
		if(worldManager != null) 
		{
			if(GetComponent<MeshRenderer>() != null)
			{
				meshRenderer = GetComponent<MeshRenderer>();
				meshRenderer.material.color = new Color(worldManager.mainMaterial.color.r, worldManager.mainMaterial.color.g, worldManager.mainMaterial.color.b, meshRenderer.material.color.a);
				return;
			}

			if(GetComponent<SpriteRenderer>() != null)
			{
				spriteRenderer = GetComponent<SpriteRenderer>();
				spriteRenderer.color = new Color(worldManager.mainMaterial.color.r, worldManager.mainMaterial.color.g, worldManager.mainMaterial.color.b, spriteRenderer.color.a);
				return;
			}

			if(GetComponent<ParticleSystem>() != null)
			{
				particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
				particleSystemRenderer.material.color = new Color(worldManager.mainMaterial.color.r, worldManager.mainMaterial.color.g, worldManager.mainMaterial.color.b, particleSystemRenderer.material.color.a);
				return;
			}
		}
	}
}