using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OutlineVibration : MonoBehaviour 
{
    [SerializeField]
    private float maxAlpha = 1.0f;
    [SerializeField]
    private float minAlpha = 0.0f;
    [SerializeField]
    private float frequency = 1.0f;
    Outline outline;

    void Awake()
    {
        this.outline = this.GetComponent<Outline>();
    }

    void Update()
    {
        Color color = this.outline.effectColor;
        color.a = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * frequency * Mathf.PI * 2) + 1) / 2f);
        this.outline.effectColor = color;
    }
}
