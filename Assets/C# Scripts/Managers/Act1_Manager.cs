using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // Wajib untuk fungsi Event

public class Act1_Manager : MonoBehaviour
{
    [Header("Manager References")]
    [Tooltip("Drag Dialogue Manager di sini")]
    [SerializeField] private DialogueManager dialogueManager;

    [Tooltip("Drag object yang ada script Puzzle1_Manager")]
    [SerializeField] private Puzzle1_Manager puzzle1Manager;

    [Header("Narrative Data")]
    [Tooltip("Masukkan Scriptable Object percakapan Intro Act 1")]
    [SerializeField] private DialogueData introDialogue;

    // Penanda agar intro tidak berulang jika script di-reset
    private bool hasIntroPlayed = false;

    void Start()
    {
        // Safety Check: Pastikan semua slot terisi di Inspector
        if (dialogueManager == null || puzzle1Manager == null || introDialogue == null)
        {
            Debug.LogError("Act1_Manager: Ada referensi yang kosong! Cek Inspector.");
            return;
        }

        StartCoroutine(StartSequenceRoutine());
    }

    private IEnumerator StartSequenceRoutine()
    {
        // 1. FASE PERSIAPAN (Detik 0)
        // Perintah ke Puzzle Manager: "Nyalakan GameObject-nya, tapi bikin Transparan (Alpha 0) & Jangan bisa diklik"
        if (puzzle1Manager != null)
        {
            puzzle1Manager.SetPuzzleState(false, false);
        }

        // Tunggu sebentar (0.5 detik) biar transisi scene mulus
        yield return new WaitForSeconds(0.5f);

        // 2. FASE INTRO (Minion Bicara)
        if (!hasIntroPlayed)
        {
            // Dengarkan kabar dari Dialogue Manager: "Kabari saya kalau dialog selesai"
            DialogueManager.OnDialogueEnded += OnIntroFinished;

            // Mulai percakapan
            dialogueManager.StartDialogue(introDialogue);

            hasIntroPlayed = true;
        }
    }

    // Fungsi ini dipanggil otomatis saat dialog selesai
    private void OnIntroFinished()
    {
        // Cabut kabel telepon (Unsubscribe) supaya tidak terpanggil dua kali
        DialogueManager.OnDialogueEnded -= OnIntroFinished;

        Debug.Log("Intro Selesai. Memunculkan Puzzle.");

        // 3. FASE GAMEPLAY
        // Perintah ke Puzzle Manager: "Munculkan Visualnya (Alpha 1) & Izinkan Pemain Klik"
        if (puzzle1Manager != null)
        {
            puzzle1Manager.SetPuzzleState(true, true);
        }
    }

    // Safety: Pastikan event dicabut jika object mati/pindah scene
    private void OnDisable()
    {
        DialogueManager.OnDialogueEnded -= OnIntroFinished;
    }
}