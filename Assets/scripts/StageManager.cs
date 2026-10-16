using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    [Header("選択させないようにするパネル")]
    [SerializeField] private GameObject[] HidePanels;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        StageHide();
    }
    public void StageHide()
    {
        //一旦すべてオンにする
        foreach (GameObject panel in HidePanels)
        {
            panel.SetActive(true);
        }
        int clearStages = PlayerPrefs.GetInt("ClearStages", 0);
        for(int i = 0; i < clearStages; i++)
        {
            HidePanels[i].SetActive(false);
        }
    }
}
