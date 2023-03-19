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
        //Debug.Log(name);
        Tinylytics.AnalyticsManager.LogCustomMetric("User Initials", name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            start_Opening();
        }
    }

    void start_Opening()
    {
        SceneManager.LoadScene("Opening_Scene");
    }

}
