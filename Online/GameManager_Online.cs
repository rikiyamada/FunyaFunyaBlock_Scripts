using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//bool変数二個の確認と、リクエストオーナー周りがおかしい

public class GameManager_Online : MonoBehaviourPunCallbacks
{
    [SerializeField] private Spawner_Online spawner;
    [SerializeField] Text turnText, countTimerText;

    Vector2 tapPos1, tapPos2, beforePos;
    Block activeBlock;
    private Rigidbody2D rbActive;
    public bool acceptPlayerInput = false, isMyTurn = true;
    float spawny;
    public int score;
    private float spawnCheckTimer, countTimer;
    public float spawnPos_y = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isMyTurn = false;
            spawnBlock();
            acceptPlayerInput = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            PhotonNetwork.Disconnect();
            AudioManager.instance.PlaySound();
            SceneManager.LoadScene("TitleScene");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(acceptPlayerInput + "ac" + isMyTurn);
        }

        CountDownTimer();

        moveBlock();

        if (!activeBlock) return;


        if (!PhotonNetwork.IsMasterClient) return;

        if (CheckActiveBlock())
        {
            spawnBlock();
        }

    }

    void spawnBlock()   //部屋主がブロックをスポーンさせる
    {
        photonView.RPC(nameof(spawnerMove), RpcTarget.All);
        spawner.spawnBlock(new Vector2(0, spawnPos_y));
        photonView.RPC(nameof(LoadActiveBlock), RpcTarget.All);
    }

    [PunRPC]
    private void LoadActiveBlock()
    {
        activeBlock = GameObject.FindWithTag("Spawned").GetComponent<Block>();
        activeBlock.transform.tag = "Active";
        rbActive = activeBlock.GetComponent<Rigidbody2D>();

        ChangeTurn();
        if (isMyTurn) acceptPlayerInput = true;

        if (!PhotonNetwork.IsMasterClient && isMyTurn)
        {
            RequestOwner();
        }

        countTimer = 10;
    }

    [PunRPC]
    private void spawnerMove()
    {
        if (!activeBlock) return;
        if (spawnPos_y < activeBlock.transform.position.y + 1.5f)
        {
            spawnPos_y += 1;
        }
    }

    private bool CheckActiveBlock()
    {
        if (activeBlock.transform.position.y > spawnPos_y - 0.1f) return false; //落とされているかの判定
        if (rbActive.velocity.magnitude > 0.01f)
        {
            spawnCheckTimer += Time.deltaTime;
            if(spawnCheckTimer > 10)
            {
                spawnCheckTimer = 0;
                return true;
            }
            return false;
        }

        spawnCheckTimer = 0;
        return true;

    }

    private void CountDownTimer()
    {
        if(countTimer <= 0)
        {
            if(isMyTurn && acceptPlayerInput)
            {
                dropBlock();
                countTimerText.text = 0 + "";
            }

            return;
        }

        countTimer -= Time.deltaTime;
        countTimerText.text = Mathf.CeilToInt(countTimer).ToString();

    }

    public void dropBlock()
    {
        AudioManager.instance.PlaySound();

        if (!activeBlock) return;
        if (!acceptPlayerInput) return;
        acceptPlayerInput = false;
        photonView.RPC(nameof(PhysicsOn), RpcTarget.MasterClient);

    }


    void moveBlock()
    {
        if (!acceptPlayerInput) return;
        if (Input.GetMouseButtonDown(0))
        {
            tapPos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            beforePos = activeBlock.transform.position;
        }


        if (Input.GetMouseButton(0))
        {
            tapPos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (activeBlock.transform.position.x > 2.1 || activeBlock.transform.position.x < -2.1)
            {
                if (activeBlock.transform.position.x * (tapPos2 - tapPos1).x > 0)
                {
                    return;
                }
            }
            activeBlock.transform.position = new Vector2((beforePos + (tapPos2 - tapPos1)).x, activeBlock.transform.position.y);

            if (tapPos2.y - tapPos1.y > 1)
            {
                //hold();
                tapPos1 = new Vector2(tapPos1.x, tapPos2.y + 100);
            }

        }
    }

    public void RotateR()
    {
        if (acceptPlayerInput)
        {
            activeBlock.transform.Rotate(0, 0, -30);
        }
    }

    public void RotateL()
    {
        if (acceptPlayerInput)
        {
            activeBlock.transform.Rotate(0, 0, 30);
        }
    }

    [PunRPC]
    public void RpcspawnerMove()
    {
        if (spawny < activeBlock.transform.position.y + 1.5f)
        {
            spawny = activeBlock.transform.position.y + 1.5f;
        }

        spawner.transform.position = new Vector2(0, spawny);
    }

    [PunRPC]
    private void PhysicsOn()
    {
        RequestOwner();
        activeBlock.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        foreach (Collider2D col in activeBlock.transform.GetComponents<Collider2D>())
        {
            col.enabled = true;
        }

        foreach (Transform child in activeBlock.transform)
        {
            child.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            child.GetComponent<Collider2D>().enabled = true;
        }
    }

    [PunRPC]
    private void RequestOwner()
    {
        activeBlock.GetComponent<PhotonView>().RequestOwnership();
        for (int i = 0; i < activeBlock.transform.childCount; i++)
        {
            activeBlock.transform.GetChild(i).GetComponent<PhotonView>().RequestOwnership();
        }

    }

    private void ChangeTurn()
    {
        isMyTurn = !isMyTurn;  //ターン入れ替え

        if (isMyTurn)
        {
            turnText.text = "あなた";
        }
        else
        {
            turnText.text = "だれか";
        }
    }

    public void HomeButotnClicked()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void RankingButtonClicked()
    {
        //ランキング送信
    }

    public void RetryButtonClicked()
    {
        SceneManager.LoadScene("MatchingScene");
    }

    public void RequestBanner()
    {
        //adsBanner.GetComponent<AdsBanner>().RequestBanner();
    }

    public void BannerDestroy()
    {
        //adsBanner.GetComponent<AdsBanner>().BannerDestroy();
    }


}
