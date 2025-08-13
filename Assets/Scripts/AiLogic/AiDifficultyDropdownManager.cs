using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AiDifficultyDropdownManager : MonoBehaviour
{
    [SerializeField] private AudioSource UiAudioSource;

    // Set opponent difficulty and handle ui sounds
    public void OnDropdownChanged()
    {
        GameManager.Instance.SetAiDifficulty(this.GetComponent<TMP_Dropdown>().value);
        UiAudioSource.Play();
    }
}
