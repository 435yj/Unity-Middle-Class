using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeText : MonoBehaviour
{
    private Text text;
    [Range(0.1f, 1f)]
    [SerializeField] private float fadeTime;
    void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(Fade());
    }

    // Update is called once per frame
    private IEnumerator Fade()
    {
        while(true)
        {
            text.DOFade(0f, fadeTime);
            new WaitForSeconds(fadeTime);
            text.DOFade(1f, fadeTime);
            new WaitForSeconds(fadeTime);
        }
    }
}