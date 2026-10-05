using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("カメラの移動速度")]
    [SerializeField] private float followSpeed = 5f;
    //プレイヤーの位置取得用
    private GameObject PlayerObj;

    //最大or最小のカメラの移動範囲を設定するためのオブジェクト
    private GameObject minXObj;
    private GameObject maxXObj;
    private GameObject minYObj;
    private GameObject maxYObj;

    //カメラの移動範囲変数
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    //カメラの移動範囲を外部から設定
    public void CameraSetting(float minx, float maxx, float miny, float maxy)
    {
        minX = minx; maxX = maxx;
        minY = miny; maxY = maxy;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        minXObj = GameObject.FindGameObjectWithTag("minX");
        maxXObj = GameObject.FindGameObjectWithTag("maxX");
        minYObj = GameObject.FindGameObjectWithTag("minY");
        maxYObj = GameObject.FindGameObjectWithTag("maxY");
        if(minX == 0)
        {
            minX = minXObj.transform.position.x;
            minY = minYObj.transform.position.y;
            maxX = maxXObj.transform.position.x;
            maxY = maxYObj.transform.position.y;
        }
    }

    // プレイヤーをふわっと追従するためにLateUpdate
    void LateUpdate()
    {
        if(PlayerObj != null) {
            float x = Mathf.Clamp(PlayerObj.transform.position.x, minX, maxX);
            float y = Mathf.Clamp(PlayerObj.transform.position.y, minY, maxY);
            Vector3 targetPos = new Vector3(x, y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position,targetPos,followSpeed * Time.deltaTime);
        }
        else{
        }
    }
}
