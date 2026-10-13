
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
    }
}
