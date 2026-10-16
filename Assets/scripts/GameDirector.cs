using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.Assertions.Must;

public class GameDirector : MonoBehaviour
{
    //どこからでもアクセスできるようにしておく
    public static GameDirector Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [Header("キャラクターのデータベース")]
    [SerializeField] private CharacterDatabase characterDatabase;

    [Header("参照")]
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject ClearPanel;
    [SerializeField] private GameObject FadeImage;
    [SerializeField] private GameObject TalkPanel;
    [SerializeField] private GameObject TalkBox;
    [SerializeField] private Text CountDownText;
    [SerializeField] private Text DeathCountText;
    [SerializeField] private Image RankImage;
    [SerializeField] private Image leftCharacterImage;
    [SerializeField] private Image rightCharactorImage;
    [SerializeField] private Text TalkText;
    [SerializeField] private Text PauseDeathText;
    [SerializeField] private Text PauseStageText;
    [SerializeField] private CameraController cameraController;

    [Header("クリア後の評価表示")]
    [SerializeField] private Sprite[] RankSprites;

    [Header("ステージのプレファブ")]
    [SerializeField] private GameObject[] stagePrefabs;

    [Header("ゲームの状態")]
    public bool isGaming = true;
    public bool isLiving = true;

    //死んだ数をカウントしておく変数
    private int deathCount;
    //制限時間用の変数
    private float countDownTimer = 0;
    //処理の複数回実行によるフリーズ防止
    private bool isRestarting;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //前のステージ数の取得と変わっていた時のデス数の初期化
        int lastStage = DontDestroyDirector.Instance.nowStage;
        if(lastStage != DontDestroyDirector.Instance.lastStage) {
            DontDestroyDirector.Instance.deathCount = 0;
            DontDestroyDirector.Instance.lastStage = lastStage;
        }
        //表示の初期化
        PausePanel.SetActive(false);
        ClearPanel.SetActive(false);
        TalkPanel.SetActive(false);
        FadeImage.SetActive(false);
        countDownTimer = 0;
        //変数の初期化
        isGaming = false;
        isLiving = true;
        StageGenerate(DontDestroyDirector.Instance.nowStage);
        deathCount = DontDestroyDirector.Instance.deathCount;
        isRestarting = false;
        if (PlayerPrefs.GetInt("isStory", 1) == 1) { 
            StartCoroutine(TalkingPhase());
        }else
         {
                isGaming = true;
         }
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && isGaming)
        {
            Pause();
        }
        if(isGaming && countDownTimer > 0)
        {
            countDownTimer -= Time.deltaTime;
            DontDestroyDirector.Instance.totalTime += Time.deltaTime;
            //分:秒に変換する部分
            int totalSeconds = Mathf.CeilToInt(Mathf.Max(0, countDownTimer));
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            CountDownText.text = $"{minutes:00}:{seconds:00}";
            if(countDownTimer < 0 && isLiving == true)
            {
                isLiving = false;
                Restart();
            }
        }
    }

    //ポーズメニューを表示（ボタンから呼び出し）
    public void Pause()
    {
        PauseDeathText.text = $"デス数：{DontDestroyDirector.Instance.deathCount}";
        PauseStageText.text = $"Stage{DontDestroyDirector.Instance.nowStage}";
        bool isPaused = !PausePanel.activeSelf;
        PausePanel.SetActive(!PausePanel.activeSelf);
        //ポーズ中は時間を止める
        Time.timeScale = isPaused ? 0f : 1f;
    }

    //タイトルへ戻る（ボタンから呼び出し）
    public void Quit(int end)
    {
        Time.timeScale = 1f;
        //最終ステージのポーズ画面からQuitするとクリア画面になるので対策
        if(DontDestroyDirector.Instance.nowStage == 5 && end == 1)
        {
            SceneManager.LoadScene("ResultScene");
        }
        else
        {
            DontDestroyDirector.Instance.deathCount = 0;
            SceneManager.LoadScene("TitleScene");
        }
    }

    //リスタート用のコルーチン開始用（Playerから呼び出し）
    public void Restart()
    {
        // すでにリスタート処理中なら何もしない
        if (isRestarting) return;

        isRestarting = true;
        isGaming = false;
        isLiving = false;

        DontDestroyDirector.Instance.deathCount++;
        DontDestroyDirector.Instance.totalDeathCount++;

        StartCoroutine(ReStartCoroutine());
    }

    //クリアを検知したらパネルを表示（Playerから呼び出し）
    public void Clear()
    {
        bool isPaused = false;
        
        //進行状況（クリアステージ）の保存
        if(DontDestroyDirector.Instance.nowStage > PlayerPrefs.GetInt("ClearStages", 0)) { 
            PlayerPrefs.SetInt("ClearStages", DontDestroyDirector.Instance.nowStage);
            PlayerPrefs.Save();
        }
        if (DontDestroyDirector.Instance.nowStage == 5 && PlayerPrefs.GetInt("isStory", 1) == 1)
        {
            StartCoroutine(LastTalk());
        }
        DeathCountText.text = $"{deathCount}回";
        if(deathCount >= 0 && deathCount <= 5)
        {
            RankImage.sprite = RankSprites[0];
        }else if(deathCount >= 6 && deathCount <= 10)
        {
            RankImage.sprite = RankSprites[1];
        }
        else if (deathCount >= 11 && deathCount <= 15)
        {
            RankImage.sprite = RankSprites[2];
        }
        else if (deathCount >= 16)
        {
            RankImage.sprite = RankSprites[3];
        }
        int stageNum = DontDestroyDirector.Instance.nowStage;
        if (deathCount == 0 && stageNum == 1 && PlayerPrefs.GetInt("isStory", 1) == 1)
        {
            StartCoroutine(FirstDeathTalk());
        }else if (stageNum == 5 && PlayerPrefs.GetInt("isStory", 1) == 1)
        {

        }

            ClearPanel.SetActive(true);
    }

    //ちょっと待ってからシーンを再ロードする
    IEnumerator ReStartCoroutine()
    {
        //初めて死んだときは会話を入れる
        int stageNum = DontDestroyDirector.Instance.nowStage;
        if (deathCount == 0 && stageNum == 1 && PlayerPrefs.GetInt("isStory", 1) == 1)
        {
            isGaming = false;
            TalkPanel.SetActive(true);
            yield return StartCoroutine(FirstDeathTalk());
            SceneManager.LoadScene("GameScene");
        }
        else {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("GameScene");
            yield break;
        }
    }

    //初めて死んだときの会話
    IEnumerator FirstDeathTalk()
    {
        TalkPanel.SetActive(true);
        yield return StartCoroutine(GenerateTalk("痛ったぁ！？俺死んだの！？", 1, "Player", "suprised"));
        yield return StartCoroutine(GenerateTalk("あ～...", 2, "Goddess", "confused"));
        yield return StartCoroutine(GenerateTalk("なんか自他ともに一撃必殺の爆発が起きとるな...", 2, "Goddess", "confused"));
        yield return StartCoroutine(GenerateTalk("多分じゃが...わしへの信仰が足りてないから加護が十分じゃないらしい...", 2, "Goddess", "confused"));
        yield return StartCoroutine(GenerateTalk("じゃあ俺はどうすれば！？", 1, "Player", "suprised"));
        yield return StartCoroutine(GenerateTalk("時間の加護もやるからそれで頑張って★", 2, "Goddess", "smile"));
        yield return StartCoroutine(GenerateTalk("まあお前の信仰度じゃし達成できなきゃ強制爆破とかついたりしてなガハハ！", 2, "Goddess", "smile"));
        yield return StartCoroutine(GenerateTalk("（村のやつらはこんな適当女神を信仰してるのか...）", 1, "Player", "confused"));
        yield return StartCoroutine(GenerateTalk("それじゃあ時間戻してやるからもいっちょいってこい！", 2, "Goddess", "smile"));
        TalkPanel.SetActive(false);
    }

    //最終ステージクリアの後の会話
    IEnumerator LastTalk()
    {
        TalkPanel.SetActive(true);
        yield return StartCoroutine(GenerateTalk("ボス討伐おめでとう", 2, "Goddess", "smile"));
        yield return StartCoroutine(GenerateTalk("ようやく全部終わったんだ！", 1, "Player", "smile"));
        yield return StartCoroutine(GenerateTalk("ま、今後もモンスター出てきたらよろしくな", 2, "Goddess", "smile"));
        yield return StartCoroutine(GenerateTalk("爆発するのはもうこりごりだー！", 1, "Player", "suprised"));
        TalkPanel.SetActive(false);
    }

    //クリア後の次のステージへ行く処理
    public void NextStage()
    {
        int nextStage = DontDestroyDirector.Instance.nowStage + 1;
        if(nextStage < 6) { 
            DontDestroyDirector.Instance.nowStage = nextStage;
            deathCount = 0;
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            SceneManager.LoadScene("ResultScene");
        }
    }

    //時間を止めずに一時停止したいとき（セリフ用）
    public void SetGamingState(bool state)
    {
        isGaming = state;
    }

    //ステージ生成と時間制限を設定する関数
    void StageGenerate(int stageNum)
    {
        //配列は0スタートなので-1する
        switch (stageNum)
        {
            case 1:
                cameraController.CameraSetting(0,0,0,0);
                countDownTimer = 20f;
                Instantiate(stagePrefabs[stageNum-1]);
                break;
            case 2:
                cameraController.CameraSetting(0,5,0,0);
                countDownTimer = 10f;
                Instantiate(stagePrefabs[stageNum-1]);
                break;
            case 3:
                cameraController.CameraSetting(0,5,0,-5);
                countDownTimer = 15f;
                Instantiate(stagePrefabs[stageNum - 1]);
                break;
            case 4:
                countDownTimer = 20f;
                Instantiate(stagePrefabs[stageNum - 1]);
                break;
            case 5:
                countDownTimer = 20f;
                Instantiate(stagePrefabs[stageNum - 1]);
                break;
            default:
                Debug.LogError("そんなステージ数うちにはないよ！");
                break;
        }
    }

    

    //セリフを生成する関数
    //message→表示する文、charactorPosition→1なら左/2なら右、charactor→表示したいキャラクターの名前、expression→キャラクターの表情（smile,confused,suprised,angry,mystery）
    IEnumerator GenerateTalk(string message, int charactorPosition, string character, string expression = "smile")
    {
        //セリフのテキストを更新
        TalkText.text = message;

        //データベースからキャラクターImageを取得
        CharacterData charaData = characterDatabase.GetCharacter(character);
        if (charaData == null) yield break;
        Sprite targetSprite = charaData.GetSprite(expression);

        //位置の指定
        switch (charactorPosition)
        {
            case 1:
                leftCharacterImage.sprite = targetSprite;
                leftCharacterImage.gameObject.SetActive(true);
                rightCharactorImage.gameObject.SetActive(false);
                TalkBox.SetActive(true);
                break;
            case 2:
                rightCharactorImage.sprite = targetSprite;
                rightCharactorImage.gameObject.SetActive(true);
                leftCharacterImage.gameObject.SetActive(false);
                TalkBox.SetActive(true);
                break;
            default:
                Debug.LogError($"'{charactorPosition}'は無効な位置やんね");
                break;
        }
        yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.spaceKey.wasReleasedThisFrame);
        TalkBox.SetActive(false);
        leftCharacterImage.gameObject.SetActive(false);
        rightCharactorImage.gameObject.SetActive(false);
    }

    //ステージ前にする会話
    IEnumerator TalkingPhase()
    {
        int stageNum = DontDestroyDirector.Instance.nowStage;
        isGaming = false;
        TalkPanel.SetActive(true);
        switch (stageNum)
        {
            
            case 1:
                if(deathCount == 0) { 
                yield return StartCoroutine(GenerateTalk("急に戦場に出されたから軽装なんだけど..." , 1, "Player", "confused"));
                yield return StartCoroutine(GenerateTalk("こんなんでどうやって戦えばいいんだ..." , 1, "Player", "confused"));
                yield return StartCoroutine(GenerateTalk("ふっふっふ...わしが加護を授けてやろう..." , 2, "Goddess", "mystery"));
                yield return StartCoroutine(GenerateTalk("この声は！？" , 1, "Player", "suprised"));
                yield return StartCoroutine(GenerateTalk("わしじゃ..." , 2, "Goddess", "mystery"));
                yield return StartCoroutine(GenerateTalk("女★神★降★臨" , 2, "Goddess", "smile"));
                yield return StartCoroutine(GenerateTalk("えと...誰...？" , 1, "Player", "confused"));
                yield return StartCoroutine(GenerateTalk("この村で信仰されてる女神ぞ！？" , 2, "Goddess", "suprised"));
                yield return StartCoroutine(GenerateTalk("あ～...聞いたことはありますね..." , 1, "Player", "confused"));
                yield return StartCoroutine(GenerateTalk("まさかこんな信仰心薄いヤツに話しかけるとは..." , 2, "Goddess", "confused"));
                yield return StartCoroutine(GenerateTalk("まあよい！とにかくあの強そうなのを倒してこい！" , 2, "Goddess", "angry"));
                yield return StartCoroutine(GenerateTalk("あいつが小さいのを生み出してるから倒せば全部解決じゃ！" , 2, "Goddess", "angry"));
                yield return StartCoroutine(GenerateTalk("...ところでなんの加護なんですか？" , 1, "Player", "confused"));
                yield return StartCoroutine(GenerateTalk("そりゃお前身体能力の強化と'イチゲキヒッサツ'のオートカウンターじゃ" , 2, "Goddess", "smile"));
                yield return StartCoroutine(GenerateTalk("空中でSpaceを押せば平行移動もできるぞ！" , 2, "Goddess", "smile"));
                yield return StartCoroutine(GenerateTalk("オートカウンターってことは体当たりでも一撃必殺！？" , 1, "Player", "smile"));
                yield return StartCoroutine(GenerateTalk("それじゃあ早速いってきます！" , 1, "Player", "smile"));
                yield return StartCoroutine(GenerateTalk("うむ！村民が減らんうちに早めにな！" , 2, "Goddess", "smile"));
                }
                break;
            case 2:
                if(deathCount == 0) { 
                yield return StartCoroutine(GenerateTalk("たおしたところで自分も爆発するから勝った気がしない..." , 1, "Player", "confused"));
                yield return StartCoroutine(GenerateTalk("まあ...なんじゃその..." , 2, "Goddess", "confused"));
                yield return StartCoroutine(GenerateTalk("あと４体がんばれ！" , 2, "Goddess", "smile"));
                yield return StartCoroutine(GenerateTalk("クソッ！さっさと済ませて家でゴロゴロしてやる！" , 1, "Player", "angry"));
                yield return StartCoroutine(GenerateTalk("ちなみに次の敵は追いかけてくるから気を付けるんじゃぞ" , 2, "Goddess", "smile"));
                }
                break;
                case 3:
                if (deathCount == 0)
                {
                    yield return StartCoroutine(GenerateTalk("よくやっておるのぉ", 2, "Goddess", "smile"));
                    yield return StartCoroutine(GenerateTalk("もうなんか爆発するの慣れてきたよ...", 1, "Player", "confused"));
                    yield return StartCoroutine(GenerateTalk("次はステージに針が接地されているようじゃぞ？", 2, "Goddess", "smile"));
                    yield return StartCoroutine(GenerateTalk("それにこの配置は...クククッ", 2, "Goddess", "smile"));
                    yield return StartCoroutine(GenerateTalk("練習は終わりのようじゃな...", 2, "Goddess", "smile"));
                }
                    break;
            case 4:
                if (deathCount == 0)
                {
                    yield return StartCoroutine(GenerateTalk("次は敵が攻撃もしてくるようじゃな", 2, "Goddess", "smile"));
                    yield return StartCoroutine(GenerateTalk("なんかどんどん複雑になってない？", 1, "Player", "confused"));
                    yield return StartCoroutine(GenerateTalk("まあより高度な能力を持った”ボス”に近づいとるんじゃろ", 2, "Goddess", "smile"));
                    yield return StartCoroutine(GenerateTalk("がんばれ！", 2, "Goddess", "smile"));
                }
                    break;
            case 5:
                if(deathCount == 0) { 
                yield return StartCoroutine(GenerateTalk("いや～長かった旅もここでおしまいじゃのう" , 2, "Goddess", "smile"));
                yield return StartCoroutine(GenerateTalk("お前もその力をなんだかんだ使いこなせるようになって..." , 2, "Goddess", "smile"));
                yield return StartCoroutine(GenerateTalk("おかげさまでね！" , 1, "Player", "angry"));
                yield return StartCoroutine(GenerateTalk("ちゃんと信仰するようになったらもっと使えるようになるかもな～？" , 2, "Goddess", "smile"));
                yield return StartCoroutine(GenerateTalk("うるせー！" , 1, "Player", "angry"));
                }
                break;
        }
       TalkPanel.SetActive(false);
        isGaming = true;
        yield break;
    }


}
