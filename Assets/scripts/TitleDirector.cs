using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class TitleDirector : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject StagePanel;
    [SerializeField] private Slider VolumeSliderBGM;
    [SerializeField] private Slider VolumeSliderSE;
    [SerializeField] private Text StoryButtonText;

    //何回もisStoryを取得しなくてもいいようにするローカル変数
    int isStory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //表示の初期化
        SettingPanel.SetActive(false);
        StagePanel.SetActive(false);
        //変数の初期化
        isStory = PlayerPrefs.GetInt("isStory", 1);
        //Sliderのボリュームの初期化
        VolumeSliderBGM.value = PlayerPrefs.GetFloat("BGMvolume", 1.0f);
        VolumeSliderSE.value = PlayerPrefs.GetFloat("SEvolume", 1.0f);
        //Sliderのボリュームが変わったら設定値も変わるようリスナーを設定
        VolumeSliderBGM.onValueChanged.AddListener(OnSliderValueChangedBGM);
        VolumeSliderSE.onValueChanged.AddListener(OnSliderValueChangedSE);
    }

    // Update is called once per frame
    void Update()
    {
        if(isStory == 1)
        {
            StoryButtonText.text = "ON";
        }
        else
        {
            StoryButtonText.text = "OFF";
        }
    }

    //スライダーの値を読み込む
    private void OnSliderValueChangedBGM(float value)
    {
        PlayerPrefs.SetFloat("BGMvolume", value);
        PlayerPrefs.Save();
    }
    
    private void OnSliderValueChangedSE(float value)
    {
        PlayerPrefs.SetFloat("SEvolume", value);
        PlayerPrefs.Save();
    }

    //ゲームシーンに移る関数（ボタンから呼び出し）
    public void GameStart()
    {
        DontDestroyDirector.Instance.nowStage = 1;
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    //ステージ指定でゲームシーンに移る関数
    public void GameStartWithStageID(int StageNum)
    {
        DontDestroyDirector.Instance.nowStage = StageNum;
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    //設定パネルを出したりしまったりする関数
    public void Setting()
    {
            SettingPanel.SetActive(!SettingPanel.activeSelf);    
    }

    //ステージ選択パネルを出したりしまったりする関数
    public void Stages()
    {
        StagePanel.SetActive(!StagePanel.activeSelf);
    }

    //ストーリー表示をどうするか（ボタンから呼び出し）
    public void isStorySetting()
    {
        if (PlayerPrefs.GetInt("isStory", 1) == 1) {
            PlayerPrefs.SetInt("isStory", 0);
            PlayerPrefs.Save();
            isStory = 0;
        }
        else
        {
            PlayerPrefs.SetInt("isStory", 1);
            PlayerPrefs.Save();
            isStory = 1;
        }
    }
}
