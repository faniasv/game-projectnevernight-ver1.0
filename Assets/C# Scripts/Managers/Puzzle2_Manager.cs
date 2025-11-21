using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Kita butuh ini untuk Image

// Ini adalah struktur data "Array di dalam Array" (Database Puzzle)
[System.Serializable]
public class PuzzleData
{
    [Header("Data Inti Puzzle")]
    public string puzzleName; // Misal: "Vas Keluarga"
    public GameObject vasePuzzleSetPrefab; // Prefab 'VaseX_PuzzleSet'

    // --- PERUBAHAN ANDA ADA DI SINI ---
    [Tooltip("Gambar (Sprite) yang akan ditampilkan di photocard")]
    public Sprite photocardSprite; // Kita simpan gambarnya, bukan panelnya
    // --------------------------------

    [Header("Data Narasi")]
    public DialogueData openingDialogue; // Dialog SEBELUM puzzle ini
    public DialogueData afterPuzzleDialogue; // Dialog SETELAH puzzle ini
}

public class Puzzle2_Manager : MonoBehaviour
{
    [Header("Panel Utama")]
    [SerializeField] private GameObject vaseSelectionPanel;
    [SerializeField] private GameObject reassemblePanel;
    [SerializeField] private GameObject cutscenePanel;

    // --- PERUBAHAN ANDA ADA DI SINI ---
    [Header("Panel Photocard (Hanya 1)")]
    [SerializeField] private GameObject photocardPanel; // Panel Parent Photocard
    [SerializeField] private Image photocardImageDisplay; // Komponen Image di dalam panel itu
    // --------------------------------

    [SerializeField] private Transform puzzleSetParent; // Tempat memunculkan VaseX_PuzzleSet

    [Header("Database Puzzle")]
    public PuzzleData[] allPuzzles;

    [Header("Koneksi Sistem")]
    [SerializeField] private DialogueManager dialogueManager;

    private GameObject currentPuzzleSetInstance;

    public void OnVaseSelected(int puzzleIndex)
    {
        if (puzzleIndex < 0 || puzzleIndex >= allPuzzles.Length)
        {
            Debug.LogError($"Index puzzle tidak valid: {puzzleIndex}");
            return;
        }
        vaseSelectionPanel.SetActive(false);
        StartCoroutine(PlayCutsceneAndStartPuzzle(allPuzzles[puzzleIndex]));
    }

    private IEnumerator PlayCutsceneAndStartPuzzle(PuzzleData puzzleData)
    {
        Debug.Log("Memainkan Cutscene Minion...");
        cutscenePanel.SetActive(true);
        yield return new WaitForSeconds(3f); // Durasi cutscene
        cutscenePanel.SetActive(false);

        Debug.Log($"Memunculkan puzzle set: {puzzleData.puzzleName}");
        reassemblePanel.SetActive(true);

        if (currentPuzzleSetInstance != null) 
        { 
            Destroy(currentPuzzleSetInstance); 
        }

        currentPuzzleSetInstance = Instantiate(puzzleData.vasePuzzleSetPrefab, puzzleSetParent);

        VasePuzzleSet vaseScript = currentPuzzleSetInstance.GetComponent<VasePuzzleSet>();
        if (vaseScript != null)
        {
            // Kirim data puzzlenya.
            vaseScript.SetupSet(this, puzzleData);
        }
    }


    public void HandlePuzzleCompletion(PuzzleData completedPuzzleData)
    {
        Debug.Log("Puzzle2_Manager: Menerima sinyal puzzle selesai!");
        StartCoroutine(ShowPhotocardSequence(completedPuzzleData));
    }

    private IEnumerator ShowPhotocardSequence(PuzzleData puzzleData)
    {
        // Sembunyikan panel puzzle
        reassemblePanel.SetActive(false);
        if (currentPuzzleSetInstance != null)
        {
            Destroy(currentPuzzleSetInstance);
        }

        // 1. Ganti gambar (Sprite) di dalam komponen Image
        photocardImageDisplay.sprite = puzzleData.photocardSprite;

        // 2. Tampilkan panel photocard-nya
        photocardPanel.SetActive(true);
        // ------------------------------------------

        yield return new WaitForSeconds(3f); // Tunggu pemain mengklik (atau jeda 3 detik)

        // Sembunyikan photocard
        photocardPanel.SetActive(false);

        // Tampilkan dialog setelah puzzle selesai
        Debug.Log("Memainkan dialog penutup puzzle...");
        dialogueManager.StartDialogue(puzzleData.afterPuzzleDialogue);

        yield return new WaitForSeconds(5f); // Asumsi durasi dialog

        // Kembali ke panel pemilihan vas
        ShowVaseSelection();
    }

    // (Tambahkan fungsi StartPuzzle2Sequence dan ShowVaseSelection dari script sebelumnya)
    public void StartPuzzle2Sequence()
    {
        Debug.Log("Puzzle 2 Dimulai. Menampilkan panel pemilihan...");
        vaseSelectionPanel.SetActive(true);
        reassemblePanel.SetActive(false);
        cutscenePanel.SetActive(false);
        photocardPanel.SetActive(false); // Pastikan panel photocard mati
    }

    public void ShowVaseSelection()
    {
        Debug.Log("Kembali ke Panel Pemilihan Vas...");
        vaseSelectionPanel.SetActive(true);
        reassemblePanel.SetActive(false);

        if (currentPuzzleSetInstance != null)
        {
            Destroy(currentPuzzleSetInstance);
        }
    }
}