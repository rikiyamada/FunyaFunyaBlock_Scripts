using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject RenamePanel, RankingPanel, SoundOnButton, SoundOffButton;
    [SerializeField] private Text placeHolder;
    [SerializeField] private InputField inputField;
    private string playerName;
    // Start is called before the first frame update
    void Start()
    {
        makePlayerPrefes();
        SoundButtonManage();
        placeHolder.text = PlayerPrefs.GetString("playerName");
        OnlineMenuManager.isSoloPlay = false;

        if (!AdManager.instance.isBanner) AdManager.instance.RequestBanner();
    }

    private void SoundButtonManage()
    {
        if (AudioManager.instance.soundLevel == 1)
        {
            SoundOffButton.SetActive(false);
        }
        else
        {
            SoundOnButton.SetActive(false);
        }
    }
    private void makePlayerPrefes()
    {
        if (!PlayerPrefs.HasKey("rate"))
        {
            PlayerPrefs.SetInt("rate", 1500);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("playerName"))
        {
            PlayerPrefs.SetString("playerName", "ななしさん");
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("maxRate"))
        {
            PlayerPrefs.SetInt("maxRate", 1500);
            PlayerPrefs.Save();
        }
    }

    public void ReloadButton()
    {
        AudioManager.instance.PlaySound();
        SceneManager.LoadScene("TitleScene");
    }

    public void SoloButtonClicked()
    {
        AudioManager.instance.PlaySound();
        SceneManager.LoadScene("GameScene");
        OnlineMenuManager.isSoloPlay = true;
    }

    public void MultiButtonClicked()
    {
        AudioManager.instance.PlaySound();
        SceneManager.LoadScene("MatchingScene"); 
    }

    public void NameButtonClicked()
    {
        RenamePanel.SetActive(true);
        AudioManager.instance.PlaySound();
    }

    public void CloseButtonClicked()
    {
        RenamePanel.SetActive(false);
        AudioManager.instance.PlaySound();
    }

    public void LoadNameText()
    {
        playerName = inputField.text;
    }

    public void SaveButtonClicked()
    {
        PlayFabController.Instance.SetUserName(playerName);
        AudioManager.instance.PlaySound();
    }

    public void RankingButtonClicked()
    {
        RankingPanel.SetActive(true);
        AudioManager.instance.PlaySound();
    }

    public void SoundOnButtonClicked()
    {
        SoundOnButton.SetActive(false);
        SoundOffButton.SetActive(true);
        AudioManager.instance.SoundOnButtonClicked();
    }

    public void SoundOffButtonClicked()
    {
        SoundOnButton.SetActive(true);
        SoundOffButton.SetActive(false);
        AudioManager.instance.SoundOffButtonClicked();
    }
}

