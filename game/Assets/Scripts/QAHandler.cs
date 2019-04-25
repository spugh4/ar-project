﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QAHandler : MonoBehaviour
{
    public GameObject panel;
    public Button button;
    public Text questionText;
    public Text[] possible_solutions;

    void Start()
    {
        panel.gameObject.SetActive(false);
        button.onClick.AddListener(OnClick);        
    }

    public void OnClick()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
