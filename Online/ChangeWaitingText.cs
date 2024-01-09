using UnityEngine;
using UnityEngine.UI;

public class ChangeWaitingText : MonoBehaviour
{
    public Text textComponent;  // テキストコンポーネントを格納する変数
    private float timer = 0;  // タイマー
    public float changeTime;

    private void Start()
    {
        textComponent.text = "けんさくちゅう";  // 初期状態のテキストを設定
    }

    private void Update()
    {
        // 1秒ごとにテキストを変更
        timer += Time.deltaTime;
        if (timer >= changeTime)
        {
            timer = 0f;
            if (textComponent.text == "けんさくちゅう")
            {
                textComponent.text = "けんさくちゅう.";
            }
            else if (textComponent.text == "けんさくちゅう.")
            {
                textComponent.text = "けんさくちゅう..";
            }
            else if (textComponent.text == "けんさくちゅう..")
            {
                textComponent.text = "けんさくちゅう...";
            }
            else
            {
                textComponent.text = "けんさくちゅう";
            }
        }
    }
}
