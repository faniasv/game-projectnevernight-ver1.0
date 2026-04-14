using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

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
    [SerializeField] private Minion_Act2 minion;

    [Header("Narration Data")]
    public DialogueData openingDialogue;  
    public DialogueData endingDialogue; // Dialog penutup Act 2 (setelah 5 vas)

    [Header("Visual Settings")]
    public Image[] vaseImages; // Tarik 5 Image vas dari Hierarchy ke sini
    public Color completedColor = Color.white; // Warna terang (Menyala)
    public Color uncompletedColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    private int completedVases = 0;
    private int totalVases = 5; 

    private void Start()
    {
        // 1. KONDISI AWAL
        if(minion != null) minion.ChangeState(Minion_Act2.MinionState.Intro);
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

        foreach (Image img in vaseImages)
        {
            img.color = uncompletedColor;
        }
    }

    public void StartVasePuzzle(int vaseID)
    {
        // Dipanggil saat tombol vas di-klik
        if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(false);
        if (staticMinion != null) staticMinion.SetActive(false); 
        if (puzzlePanel != null) puzzlePanel.SetActive(true);
    }

    public void ReportVaseCompleted(int index)
    {
        completedVases++;
        Debug.Log($"[Sutradara] Vas selesai: {completedVases}/{totalVases}");

        if (index >= 0 && index < vaseImages.Length)
        {
            vaseImages[index].color = completedColor;
            // Matikan tombolnya biar gak bisa diklik lagi
            vaseImages[index].GetComponent<Button>().interactable = false;
        }

        Debug.Log($"[Sutradara] Vas {index} selesai. Total: {completedVases}/{totalVases}");

        if (completedVases >= totalVases) 
        {
            StartCoroutine(EndAct2Sequence());
        }
        else 
        {
            if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(true);
            if (minion != null) minion.ChangeState(Minion_Act2.MinionState.Selection);
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