using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupBugFix : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayToggleEnableCo());
    }
    
    IEnumerator DelayToggleEnableCo()
    {
        yield return new WaitForEndOfFrame();
        GetComponent<ToggleGroup>().enabled = true;
    }
}