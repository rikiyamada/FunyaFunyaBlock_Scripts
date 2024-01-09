using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class OnlineMenuManager : MonoBehaviourPunCallbacks
{
    bool inRoom;
    bool isMatching;
    [SerializeField] GameObject matchingPanel;
    [SerializeField]  Text nameText1,nameText2, rateText1, rateText2, rensyouText1, rensyouText2;
    public static int opponentRate, opponentRensyou;
    public static string opponentName;
    public static bool isSoloPlay;



    private void Start()
    {
        inRoom = false;
        isMatching = false;

        Hashtable customProperties = new Hashtable();
        customProperties.Add("rate", PlayerPrefs.GetInt("rate"));
        customProperties.Add("rensyou", PlayerPrefs.GetInt("rensyou"));
        customProperties.Add("name", PlayerPrefs.GetString("playerName"));
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);


        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        //PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        // ランダムな座標に自身のアバター（ネットワークオブジェクト）を生成する
        //var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        //PhotonNetwork.Instantiate("BlockI", position, Quaternion.identity);
        inRoom = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, TypedLobby.Default);
    }

    // 二人揃ったらシーン移動
    public void Update()
    {
        if (isMatching)
        {
            return;
        }

        if (inRoom)
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                isMatching = true;
                isSoloPlay = false;
                Invoke("UpdateDeta", 0.3f);
            }
        }
    }

    public void HomeButtonClicked()
    {
        PhotonNetwork.Disconnect();
        AudioManager.instance.PlaySound();
        SceneManager.LoadScene("TitleScene");
    }

    private void UpdateDeta()
    {
        Photon.Realtime.Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
        Photon.Realtime.Player otherPlayer = otherPlayers[0];
        opponentName = (string)otherPlayer.CustomProperties["name"];
        opponentRate= (int)otherPlayer.CustomProperties["rate"];
        opponentRensyou = (int)otherPlayer.CustomProperties["rensyou"];

        nameText1.text = PlayerPrefs.GetString("playerName");
        nameText2.text = opponentName;
        rateText1.text = "レート：" + PlayerPrefs.GetInt("rate");
        rateText2.text = "レート：" +  opponentRate;
        rensyouText1.text = PlayerPrefs.GetInt("rensyou") + "連勝中";
        rensyouText2.text = opponentRensyou + "連勝中";

        matchingPanel.SetActive(true);

        AudioManager.instance.PlayMatchingSound();


        Invoke(nameof(MoveToGame_OnlineScene), 3.0f);
    }


    private void MoveToGame_OnlineScene()
    {
        SceneManager.LoadScene("Game_OnlineScene");
    }
}