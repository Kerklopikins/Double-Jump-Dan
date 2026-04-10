using UnityEngine;

public class GameOver : MonoBehaviour 
{
	public void RestartLevel()
	{
		GameHUD.Instance.Restart();
	}
}