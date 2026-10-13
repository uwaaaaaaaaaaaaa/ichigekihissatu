
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("AudioMixer")]
    [SerializeField] private AudioMixer audioMixer;

    private const string BGMVolumeKey = "BGMVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Awake()
    {
        // シーンをまたいでも残す
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 保存されている音量を読み込む
        float bgm = PlayerPrefs.GetFloat(BGMVolumeKey, 1f);
        float sfx = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        Debug.Log($"保存された音量：BGM={bgm}, SFX={sfx}");

        SetBGMVolume(bgm);
        SetSFXVolume(sfx);
    }

    public void SetBGMVolume(float volume)
    {
        ApplyVolume("BGMVolume", volume);
        PlayerPrefs.SetFloat(BGMVolumeKey, volume);
    }

    public void SetSFXVolume(float volume)
    {
        ApplyVolume("SFXVolume", volume);
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }

    private void ApplyVolume(string parameterName, float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixerが設定されていません！");
            return;
        }

        float dB = volume <= 0.0001f
            ? -80f
            : Mathf.Log10(volume) * 20f;

        if (!audioMixer.SetFloat(parameterName, dB))
        {
            Debug.LogError(
                $"AudioMixerのパラメータ '{parameterName}' が見つかりません！"
            );
        }
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat(BGMVolumeKey, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
    }
}
