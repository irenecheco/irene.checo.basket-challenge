using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeDropdownManager : MonoBehaviour
{
    [SerializeField] private AudioSource UiAudioSource;

    public void OnDropdownChanged()
    {
        GameManager.Instance.SetGameTime(this.GetComponent<TMP_Dropdown>().value);
        UiAudioSource.Play();
    }
}
