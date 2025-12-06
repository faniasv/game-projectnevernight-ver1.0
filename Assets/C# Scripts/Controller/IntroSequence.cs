using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // <--- WAJIB ADA untuk pindah scene

public class IntroSequence : MonoBehaviour
{
    [Header("Panel References")]
    public List<CanvasGroup> panels;
    public float fadeDuration = 1.5f;
    public float waitDuration = 1f;
    public float finalWait = 2.0f;

    [Header("Scene Navigation")]
    [Tooltip("Nama Scene selanjutnya (misal: SC_Act1)")]
    public string nextSceneName = "SC_Act1"; 

    [Header("Events")]
    public UnityEvent OnIntroFinished;

    // Dipanggil otomatis saat Scene Intro mulai
    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Pastikan semua panel alpha 0 dulu
        foreach (var p in panels) p.alpha = 0;

        // Loop Animasi Muncul
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

        // Tunggu sebentar baca komik full
        yield return new WaitForSeconds(finalWait);

        // Panggil Event (jika ada custom logic lain)
        OnIntroFinished.Invoke();

        // PINDAH SCENE OTOMATIS
        Debug.Log("Intro Selesai. Pindah ke: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}