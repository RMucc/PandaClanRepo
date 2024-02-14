using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;

    private void Awake()
    {
        entryContainer = transform.Find("scoreContainer");
        entryTemplate = entryContainer.Find("scoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        float templateheight = 20f;
        for (int i = 0; i < 10; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateheight * i);
            entryTransform.gameObject.SetActive(true);
        }
    }
}
