using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveInitials : MonoBehaviour

{
    TMP_InputField _inputField;

    public static string name;

    void Start()
    {
        _inputField = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();
    }


    public void InputName()
    {
        name = _inputField.text;
        
        GlobalControl.Instance.userInitial = name;
        
        Tinylytics.AnalyticsManager.LogCustomMetric("NEWUSERJOINED", name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Opening_Scene");
        }
    }

}
