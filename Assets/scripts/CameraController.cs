using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("カメラの移動速度")]
    [SerializeField] private float followSpeed = 5f;
    //プレイヤーの位置取得用
    private GameObject PlayerObj;

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
        if(minX == 0)
        {
            minX = minY = -10;
            maxX = maxY = 10;
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
