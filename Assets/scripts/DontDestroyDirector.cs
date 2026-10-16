using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyDirector : MonoBehaviour
{

    //どこからでもアクセスできるようにしておく
    public static DontDestroyDirector Instance { get; private set; }


    //このオブジェクトがシーンをまたげるようにする
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //デス数の持ち越しカウント
    public int deathCount = 0;

    //ステージ数の変数
    public int nowStage = 0;
    public int lastStage = 0;

    //最後の画面に表示する変数
    public int totalDeathCount = 0;
    public float totalTime = 0;
}
