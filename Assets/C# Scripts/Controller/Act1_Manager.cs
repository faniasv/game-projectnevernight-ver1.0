using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // <--- INI WAJIB ADA UNTUK EVENT 'Action'

public class Act1_Manager : MonoBehaviour
{
    [Header("Manager References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Puzzle1_Manager puzzle1Manager;

    [Header("Narrative Data")]
    [SerializeField] private DialogueData introDialogue;

    void Start()
    {
        StartCoroutine(StartSequenceRoutine());
    }

    private IEnumerator StartSequenceRoutine()
    {
        // 1. SETUP AWAL: Puzzle Terlihat tapi Gak Bisa Diklik
        if (puzzle1Manager != null)
        {
            // true = visible (alpha 1), false = not interactable
            puzzle1Manager.SetPuzzleState(true, false);
        }

        yield return new WaitForSeconds(0.5f);

        // 2. MULAI DIALOG INTRO
        if (dialogueManager != null && introDialogue != null)
        {
            // Subscribe ke event selesai
            DialogueManager.OnDialogueEnded += OnIntroFinished;

            dialogueManager.StartDialogue(introDialogue);
        }
    }

    // Dipanggil otomatis saat dialog intro selesai
    private void OnIntroFinished()
    {
        // Cabut listener biar gak error nanti
        DialogueManager.OnDialogueEnded -= OnIntroFinished;

        Debug.Log("Intro Act 1 Selesai. Player boleh main puzzle.");

        // 3. AKTIFKAN PUZZLE: Visual ON, Interaksi ON
        if (puzzle1Manager != null)
        {
            puzzle1Manager.SetPuzzleState(true, true);
        }
    }

    private void OnDisable()
    {
        // Safety check saat ganti scene
        DialogueManager.OnDialogueEnded -= OnIntroFinished;
    }
}