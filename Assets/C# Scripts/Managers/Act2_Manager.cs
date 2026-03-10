using UnityEngine;
using System.Collections;

/// <summary>
/// Sutradara khusus untuk Act 2.
/// Mengatur transisi layar pemilihan vas, dialog pembuka, dan menyembunyikan Minion saat puzzle dimulai.
/// </summary>
public class Act2_Manager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel UI untuk memilih vas")]
    public GameObject vaseSelectionPanel; 
    
    [Tooltip("Panel UI tempat pemain menyusun pecahan vas")]
    public GameObject puzzlePanel;        
    
    [Tooltip("Masukkan objek gambar Minion (UI Image) yang diam di sini")]
    public GameObject staticMinion;       

    [Header("System References")]
    public DialogueManager dialogueManager;
    
    [Tooltip("Dialog yang otomatis diputar saat Act 2 baru mulai")]
    public DialogueData openingDialogue;  

    private void Start()
    {
        // 1. KONDISI AWAL SAAT MASUK ACT 2
        // Pastikan layar pilih vas terbuka, dan puzzle tertutup
        if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(true);
        if (puzzlePanel != null) puzzlePanel.SetActive(false);

        // Pastikan Minion muncul di awal untuk menemani dialog
        if (staticMinion != null) staticMinion.SetActive(true);

        // 2. MAINKAN DIALOG OPENING
        if (dialogueManager != null && openingDialogue != null)
        {
            dialogueManager.StartDialogue(openingDialogue);
        }
        else
        {
            Debug.LogWarning("[Act2_Manager] DialogueManager atau Opening Dialogue belum diisi!");
        }
    }

    /// <summary>
    /// Fungsi ini dipanggil saat pemain mengklik tombol salah satu vas.
    /// </summary>
    /// <param name="vaseID">Nomor/ID vas yang dipilih</param>
    public void StartVasePuzzle(int vaseID)
    {
        Debug.Log($"[Act2_Manager] Pemain memilih Vas ke-{vaseID}. Transisi ke mode Puzzle...");

        // 1. Matikan layar pemilihan vas
        if (vaseSelectionPanel != null) vaseSelectionPanel.SetActive(false);

        // 2. HILANGKAN MINION (Sesuai kodisi: pas di puzzle minion hilang)
        if (staticMinion != null) staticMinion.SetActive(false);

        // 3. Buka layar puzzle
        if (puzzlePanel != null) puzzlePanel.SetActive(true);

        // TODO: Di sinilah nanti kita akan memunculkan pecahan-pecahan vas (Fragment) sesuai vaseID
    }
}