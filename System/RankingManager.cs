using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private Sprite orange, white;

    private void OnEnable()
    {
        PlayFabController.Instance.GetRanking(0);
        GameObject.Find("AllButton").GetComponent<Image>().sprite = orange;
        GameObject.Find("WeeklyButton").GetComponent<Image>().sprite = white;
    }

    public void AllButtonClicked()
    {
        PlayFabController.Instance.GetRanking(0);
        GameObject.Find("AllButton").GetComponent<Image>().sprite = orange;
        GameObject.Find("WeeklyButton").GetComponent<Image>().sprite = white;
        AudioManager.instance.PlaySound();
    }

    public void WeeklyButtonClicked()
    {
        PlayFabController.Instance.GetRanking(1);
        GameObject.Find("AllButton").GetComponent<Image>().sprite = white;
        GameObject.Find("WeeklyButton").GetComponent<Image>().sprite = orange;
        AudioManager.instance.PlaySound();
    }

    public void CloseButtonClicked()
    {
        this.gameObject.SetActive(false);
        AudioManager.instance.PlaySound();
    }
}
