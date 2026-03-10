using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class IntroSequence : MonoBehaviour
{
    [Header("Panel References")]
    public List<CanvasGroup> panels;
    public float fadeDuration = 1.5f;
    public float waitDuration = 1f;
    public float finalWait = 2.0f;

    [Header("Scene Navigation")]
    [Tooltip("Sesuai urutan Build Settings: 2 untuk pindah ke SC_Act1")]
    public int nextActNumber = 2; // Add actNumber

    [Header("Events")]
    public UnityEvent OnIntroFinished;

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        foreach (var p in panels) p.alpha = 0;

        foreach (var panel in panels)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                panel.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }
            panel.alpha = 1;
            yield return new WaitForSeconds(waitDuration);
        }

        yield return new WaitForSeconds(finalWait);

        OnIntroFinished.Invoke();

        // --- Add Level Manager and Act Number ---
        Debug.Log("Intro Selesai. Lapor ke LevelManager untuk pindah ke Act: " + nextActNumber);
        GameEvents.OnActChanged?.Invoke(nextActNumber);
    }
}