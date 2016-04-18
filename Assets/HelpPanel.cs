using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HelpPanel : MonoBehaviour
{
    [SerializeField]
    private TextAsset commonBefore = null;
    [SerializeField]
    private TextAsset pcControls = null;
    [SerializeField]
    private TextAsset androidControls = null;
    [SerializeField]
    private TextAsset commonAfter = null;

    [SerializeField]
    private Text helpText = null;


    void Awake()
    {
        string separation = Environment.NewLine + Environment.NewLine + Environment.NewLine;

        this.helpText.text = commonBefore.text;
        this.helpText.text += separation;
#if UNITY_ANDROID && !UNITY_EDITOR
        this.helpText.text += androidControls.text;
#else
        this.helpText.text += pcControls.text;
#endif
        this.helpText.text += separation;
        this.helpText.text += commonAfter.text;
    }

}
