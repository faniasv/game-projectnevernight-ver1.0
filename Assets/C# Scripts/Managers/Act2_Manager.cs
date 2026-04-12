using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement; // WAJIB ADA buat pindah scene!

/// <summary>
/// Sutradara khusus untuk Act 2.
/// Mengatur transisi layar pemilihan vas, dialog pembuka, dan ending Act 2.
/// </summary>
public class Act2_Manager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject vaseSelectionPanel; 
    public GameObject puzzlePanel;        
    public GameObject staticMinion;       

    [Header("System References")]
    public DialogueManager dialogueManager;
    public Puzzle2_Manager puzzle2Manager; 
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Narration Data")]
    public DialogueData openingDialogue;  
    public DialogueData endingDialogue; // Dialog penutup Act 2 (setelah 5 vas)

    private int completedVases = 0;
    private int totalVases = 5; 

    private void Start()
    {
        // 1. KONDISI AWAL
        if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(false);
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        if (staticMinion != null) staticMinion.SetActive(true);

        // 2. MAINKAN DIALOG OPENING (Ava terbangun di toko vas)
        if (dialogueManager != null && openingDialogue != null)
        {
            dialogueManager.StartDialogue(openingDialogue, () => {
                // Callback: Setelah dialog beres, panel vas baru nongol
                if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(true);
            });
        }
        else
        {
            // Fallback kalau lupa pasang dialog
            if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(true);
        }
    }

    public void StartVasePuzzle(int vaseID)
    {
        // Dipanggil saat tombol vas di-klik
        if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(false);
        if (staticMinion != null) staticMinion.SetActive(false); 
        if (puzzlePanel != null) puzzlePanel.SetActive(true);
    }

    public void ReportVaseCompleted()
    {
        completedVases++;
        Debug.Log($"[Sutradara] Vas selesai: {completedVases}/{totalVases}");

        if (completedVases >= totalVases) // Kalau sudah 5
        {
            // Jalankan sequence Ending Act 2 (Dialog Final)
            StartCoroutine(EndAct2Sequence());
        }
        else 
        {
            // BALIK KE RAK VAS (Vase Selection Panel)
            if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(true);
            if (staticMinion != null) staticMinion.SetActive(true);
        
            // Tambahan: Di sini lo bisa matiin button vas yang barusan dikerjain 
            // biar pemain nggak nge-klik vas yang sama dua kali.
        }
    }

    private IEnumerator EndAct2Sequence()
    {
        yield return new WaitForSeconds(1f);

        if (dialogueManager != null && endingDialogue != null)
        {
            dialogueManager.StartDialogue(endingDialogue, () => {
                Debug.Log("SISTEM LIMBIK STABIL. Menuju Act 3...");
                
                // Ganti "Act3_SceneName" dengan nama scene Act 3 lo yang bener
                // SceneManager.LoadScene("Act3_SceneName"); 
                SceneManager.LoadScene(mainMenuSceneName);
            });
        }
    }
}