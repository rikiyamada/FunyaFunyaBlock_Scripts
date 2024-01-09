using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController Instance;  // シングルトンのインスタンス


    private void Awake()
    {
        // 既存のインスタンスが存在する場合
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // この新しいインスタンスを破棄する
            return;
        }

        Instance = this; // このオブジェクトをシングルトンのインスタンスとして設定
        DontDestroyOnLoad(gameObject);  // このオブジェクトを破棄しないようにする
    }

    void Start()
    {
        string customId;
        if (PlayerPrefs.HasKey("CustomId"))
        {
            // 既存のIDを取得
            customId = PlayerPrefs.GetString("CustomId");
        }
        else
        {
            // 新しいIDを生成
            customId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("CustomId", customId);
        }
        PlayerPrefs.Save();
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true },
            result => Debug.Log("ログイン成功！"),
            error => Debug.Log("ログイン失敗"));
    }

    public void SetUserName(string userName)
    {
        Text AvailableText = GameObject.Find("AvailableText").GetComponent<Text>();
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSuccess, OnError);

        void OnSuccess(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log("success!");
            AvailableText.text = "変更完了";
            PlayerPrefs.SetString("UserName", userName);
            PlayerPrefs.Save();
        }

        void OnError(PlayFabError error)
        {
            Debug.Log($"{error.Error}");
            AvailableText.text = "使用できません";
        }
    }

    public void SendPlayScore(int score)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(
            new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>()
                {
                new StatisticUpdate
                {
                    StatisticName = "TotalHighScore",
                    Value = score
                },
                new StatisticUpdate
                {
                    StatisticName = "WeeklyHighScore",
                    Value = score
                }
                }
            },
            result =>
            {
                Debug.Log("スコア送信成功");
            },
            error =>
            {
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }


    public void SendButton()
    {
        SendPlayScore(10);
    }

    public void GetRanking(int num)
    {
        GetTop10Ranking(num);
        GetAroundRanking(num);
    }

    public void GetTop10Ranking(int num)
    {
        Text RankingText = GameObject.Find("Top10RankingText").GetComponent<Text>();
        string name;
        if (num == 0) name = "TotalHighScore";
        else　name = "WeeklyHighScore";
        
        var request = new GetLeaderboardRequest
        {
            StatisticName = name, // 統計情報名を指定します。
            StartPosition = 0, // 何位以降のランキングを取得するか指定します。
            MaxResultsCount = 10 // ランキングデータを何件取得するか指定します。最大が100です。
        };

        PlayFabClientAPI.GetLeaderboard(request, OnSuccess, OnError);

        void OnSuccess(GetLeaderboardResult leaderboardResult)
        {
            int i = 0;
            RankingText.text = " ";
            foreach (var item in leaderboardResult.Leaderboard)
            {
                if (item.DisplayName == null)
                {
                    RankingText.text += $"{item.Position + 1}位: ななしさん  {item.StatValue}個\n";

                }
                else
                {
                    RankingText.text += $"{item.Position + 1}位: {item.DisplayName}  {item.StatValue}個\n";
                }
                i++;
                if (i >= 10) return;
            }
        }

        void OnError(PlayFabError error)
        {
            Debug.Log($"{error.Error}");
        }

    }

    public void GetAroundRanking(int num)
    {
        Text RankingText = GameObject.Find("AroundRankingText").GetComponent<Text>();
        string name;
        if (num == 0) name = "TotalHighScore";
        else name = "WeeklyHighScore";

        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = name,
            MaxResultsCount = 10 // 周辺ランキングとして10件を取得
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnSuccess, OnError);

        void OnSuccess(GetLeaderboardAroundPlayerResult result)
        {
            RankingText.text = "";
            // 実際は良い感じのランキングを表示するコードにします。
            foreach (var item in result.Leaderboard)
            {
                if (item.DisplayName == null)
                {
                    RankingText.text += $"{item.Position + 1}位: ななしさん  {item.StatValue}個\n";

                }
                else
                {
                    RankingText.text += $"{item.Position + 1}位: {item.DisplayName}  {item.StatValue}個\n";
                }
            }
        }

        void OnError(PlayFabError error)
        {
            Debug.Log($"{error.Error}");
        }

    }
}
