using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class ResultDirector : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Text TotalDeathText;
    [SerializeField] private Text TotalTimeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float timer = DontDestroyDirector.Instance.totalTime;
        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);
        TotalDeathText.text = $"総デス数：　{DontDestroyDirector.Instance.totalDeathCount}回";
        TotalTimeText.text = $"総プレイ時間：　{minutes:00}：{seconds:00}";
    }



    public void BacktoTitle()
    {
        DontDestroyDirector.Instance.totalDeathCount = 0;
        DontDestroyDirector.Instance.totalTime = 0;
        SceneManager.LoadScene("TitleScene");
    }
}
