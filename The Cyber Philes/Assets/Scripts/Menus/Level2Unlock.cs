using UnityEngine;

public class Level2Unlock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //MenuManager.Instance.unlockedLevel2 = true;
        PlayerPrefs.SetInt("UnlockedLevel2", 1);
        PlayerPrefs.Save();
    }

}
