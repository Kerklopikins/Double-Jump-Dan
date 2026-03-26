using UnityEngine;
using System.Collections;

public class ObjectOpitimizer : MonoBehaviour 
{
	[SerializeField] MonoBehaviour[] scriptsToCheck;
	[SerializeField] GameObject[] gameObjectsToCheck;
    [SerializeField] bool onlyCheckChild;

    bool debugMode = false;
	float checkFrequency = 0.5f;
    float distance = 60;
    float _distance;
	Player player;
	GameObject child;
	bool haveDelayed;
    float _checkFrequency;
    float safetyTimer = 1;

    void Start()
	{
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.OnPlayerRespawn += Refresh;
        player.OnPlayerTeleported += Refresh;
		child = transform.GetChild(0).gameObject;
        StartCoroutine(DelayStartCo());
    }

	IEnumerator DelayStartCo()
	{
        yield return new WaitForEndOfFrame();
        haveDelayed = true;
    }

    void FixedUpdate()
	{
        if(safetyTimer > 0)
            safetyTimer -= Time.deltaTime;

        if(safetyTimer <= 0)
            if(!haveDelayed)
                haveDelayed = true;

		if(!haveDelayed)
			return;

		_checkFrequency -= Time.deltaTime;

		if(_checkFrequency <= 0)
		{
            if(onlyCheckChild && child == null)
            {
                Destroy(gameObject);
                return;
            }

            if(onlyCheckChild)
                _distance = Vector3.Distance(new Vector3(player.transform.position.x, player.transform.position.y, 0), child.transform.position);
            else
                _distance = Vector3.Distance(new Vector3(player.transform.position.x, player.transform.position.y, 0), transform.position);

            if(_distance <= distance)
			{
				if(onlyCheckChild)
					child.SetActive(true);

                if(scriptsToCheck.Length > 0)
                {
                    for(int i = 0; i < scriptsToCheck.Length; i++)
                        scriptsToCheck[i].enabled = true;
                }

                if(gameObjectsToCheck.Length > 0)
                {
                    for(int i = 0; i < gameObjectsToCheck.Length; i++)
                        gameObjectsToCheck[i].SetActive(true);
                }
            }
            else
			{
                if(onlyCheckChild)
                    child.SetActive(false);

				if(scriptsToCheck.Length > 0)
				{
					for(int i = 0; i < scriptsToCheck.Length; i++)
						scriptsToCheck[i].enabled = false;
				}

                if(gameObjectsToCheck.Length > 0)
                {
                    for(int i = 0; i < gameObjectsToCheck.Length; i++)
                        gameObjectsToCheck[i].SetActive(false);
                }
            }

			_checkFrequency = checkFrequency;
        }
	}

    void Refresh()
    {
        if(onlyCheckChild)
        {
            if(child != null)
                _distance = Vector3.Distance(new Vector3(player.transform.position.x, player.transform.position.y, 0), child.transform.position);
        }
        else
            _distance = Vector3.Distance(new Vector3(player.transform.position.x, player.transform.position.y, 0), transform.position);

        if(_distance <= distance)
        {
            if(onlyCheckChild)
                if(child != null)
                    child.SetActive(true);

            if(scriptsToCheck.Length > 0)
            {
                for(int i = 0; i < scriptsToCheck.Length; i++)
                    scriptsToCheck[i].enabled = true;
            }

            if(gameObjectsToCheck.Length > 0)
            {
                for(int i = 0; i < gameObjectsToCheck.Length; i++)
                    gameObjectsToCheck[i].SetActive(true);
            }
        }

        _checkFrequency = checkFrequency;
    }
    void OnDrawGizmos()
    {
        if(debugMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, distance);
        }
    }
}