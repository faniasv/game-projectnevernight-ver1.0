using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Butuh ini untuk pakai "List"
using UnityEngine.Events; // Butuh ini untuk pakai "UnityEvent"

public class IntroSequence : MonoBehaviour
{
    [Header("Bahan-Bahan")]
    // List: Wadah untuk menaruh banyak Panel sekaligus (WakeUp, Work, Eat)
    // CanvasGroup: Komponen untuk mengatur transparansi (Alpha) satu grup UI
    public List<CanvasGroup> panels;

    // Durasi: Berapa detik waktu yang dibutuhkan dari transparan -> jelas
    public float fadeDuration = 1.5f;

    // Jeda: Berapa detik diam dulu sebelum gambar berikutnya muncul
    public float waitDuration = 1f;

    [Header("Pemicu Akhir")]
    // Event: Sinyal yang ditembakkan kalau semua sudah selesai
    public UnityEvent OnIntroFinished;

    // Fungsi ini ibarat tombol "ON" yang akan ditekan oleh Start Button
    public void StartIntro()
    {
        StartCoroutine(PlaySequence());
    }

    // Ini adalah "Resep" utamanya
    IEnumerator PlaySequence()
    {
        // "foreach" artinya: Lakukan hal ini untuk SETIAP panel yang ada di dalam list
        foreach (var panel in panels)
        {
            // === Tahap 1: FADE IN (Muncul Perlahan) ===
            float timer = 0f;

            // Selama timer masih kurang dari durasi yang ditentukan...
            while (timer < fadeDuration)
            {
                // Tambahkan waktu berjalan (frame demi frame)
                timer += Time.deltaTime;

                // Mathf.Lerp adalah Matematika untuk "Pergeseran Bertahap"
                // Mengubah Alpha dari 0 (hilang) ke 1 (muncul) berdasarkan % waktu berjalan
                panel.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);

                // "yield return null" artinya: "Tunggu frame berikutnya, jangan langsung selesai"
                yield return null;
            }

            // Pastikan alpha benar-benar 1 (penuh) saat loop selesai
            panel.alpha = 1;

            // === Tahap 2: TUNGGU SEBENTAR ===
            // Biarkan pemain melihat gambarnya dulu sebelum lanjut ke gambar sebelah
            yield return new WaitForSeconds(waitDuration);
        }

        // === Tahap 3: SELESAI ===
        // Kalau semua panel di list sudah muncul, panggil event ini
        Debug.Log("Intro Selesai! Pindah ke Act 1.");
        OnIntroFinished.Invoke();
    }
}