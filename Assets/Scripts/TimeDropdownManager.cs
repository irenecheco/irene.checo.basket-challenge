using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeDropdownManager : MonoBehaviour
{
    public void OnDropdownChanged()
    {
        GameManager.Instance.SetGameTime(this.GetComponent<TMP_Dropdown>().value);
    }
}
