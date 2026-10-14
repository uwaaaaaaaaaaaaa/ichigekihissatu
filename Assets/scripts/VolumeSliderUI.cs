
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        bgmSlider.SetValueWithoutNotify(
            AudioManager.Instance.GetBGMVolume()
        );

        sfxSlider.SetValueWithoutNotify(
            AudioManager.Instance.GetSFXVolume()
        );

        // イベントをコードから登録
        bgmSlider.onValueChanged.RemoveListener(
            AudioManager.Instance.SetBGMVolume
        );
        bgmSlider.onValueChanged.AddListener(
            AudioManager.Instance.SetBGMVolume
        );

        sfxSlider.onValueChanged.RemoveListener(
            AudioManager.Instance.SetSFXVolume
        );
        sfxSlider.onValueChanged.AddListener(
            AudioManager.Instance.SetSFXVolume
        );
    }
}
