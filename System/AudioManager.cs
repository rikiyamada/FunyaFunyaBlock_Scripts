using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public int soundLevel;
    [SerializeField] AudioSource BGM, SE, SE_Matching;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        soundLevel = 1;
    }

    public void SoundOnButtonClicked()
    {
        soundLevel = 0;
        BGM.volume = 0;
        SE.volume = 0;
    }

    public void SoundOffButtonClicked()
    {
        soundLevel = 1;
        BGM.volume = 1;
        SE.volume = 1;
        PlaySound();
    }

    public void PlaySound()
    {
        SE.PlayOneShot(SE.clip);
        VibrationMng.ShortVibration();
    }

    public void PlayMatchingSound()
    {
        SE.PlayOneShot(SE_Matching.clip);
        VibrationMng.ShortVibration();
    }
}
