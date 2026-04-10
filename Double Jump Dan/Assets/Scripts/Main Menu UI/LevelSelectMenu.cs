using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField] ScrollRect levelsScrollRect;

    public event Action OnLevelButtonsRefresh;

    public void RefreshLevels()
    {
        OnLevelButtonsRefresh?.Invoke();
        StartCoroutine(DelayLevelsContentCentering());
    }

    IEnumerator DelayLevelsContentCentering()
    {
        yield return null;
        yield return null;

        levelsScrollRect.verticalNormalizedPosition = 1;
    }
}