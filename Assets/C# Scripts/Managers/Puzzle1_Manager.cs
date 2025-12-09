using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class TaskData
{
    public string taskID;
    [Range(0, 100)] public float taskLoad;
    public DialogueData forgottenDialogue;
}

public class Puzzle1_Manager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject puzzlePanel;
    public CanvasGroup puzzleCanvasGroup;
    public GameObject errorPanel;

    public Slider mentalLoadSlider;
    public float maxLoad = 100f;

    [Header("System References")]
    public DialogueManager dialogueManager;

    [Header("Puzzle Data")]
    public List<TaskData> allTasksInLevel;
    public DialogueData overloadDialogue; // Dialog saat Give Up (Percobaan ke-3)

    private int attemptCounter = 0;
    private Dictionary<int, string> occupiedSlots = new Dictionary<int, string>();

    // Subscribe Event agar puzzle aktif lagi setelah dialog selesai
    private void OnEnable() => DialogueManager.OnDialogueEnded += ReboundPuzzle;
    private void OnDisable() => DialogueManager.OnDialogueEnded -= ReboundPuzzle;

    void Start()
    {
        if (puzzlePanel != null) puzzlePanel.SetActive(true);

        if (mentalLoadSlider != null)
        {
            mentalLoadSlider.maxValue = maxLoad;
            mentalLoadSlider.value = 0;
        }

        // Sembunyikan visual (Alpha 0) menunggu Act1_Manager memunculkan
        SetPuzzleState(false, false);
    }

    // --- LOGIKA SLOT & SLIDER (TIDAK DIUBAH) ---

    public void OnTaskDroppedOnSlot(int slotIndex, string taskID)
    {
        if (occupiedSlots.ContainsKey(slotIndex)) occupiedSlots[slotIndex] = taskID;
        else occupiedSlots.Add(slotIndex, taskID);

        UpdateSliderVisual();
    }

    public void OnTaskRemovedFromSlot(int slotIndex)
    {
        if (occupiedSlots.ContainsKey(slotIndex)) occupiedSlots.Remove(slotIndex);

        UpdateSliderVisual();
    }

    private void UpdateSliderVisual()
    {
        if (mentalLoadSlider == null) return;

        float currentTotalLoad = 0f;
        foreach (var item in occupiedSlots)
        {
            string id = item.Value;
            TaskData data = allTasksInLevel.Find(x => x.taskID == id);
            if (data != null) currentTotalLoad += data.taskLoad;
        }
        mentalLoadSlider.value = currentTotalLoad;
    }

    // --- LOGIKA UTAMA YANG DIUBAH (BUTTON & DIALOGUE) ---

    public void OnSaveButtonPressed()
    {
        Debug.Log("TEST: Tombol Kerjakan Berhasil Ditekan!");

        // Mencegah spam klik tombol saat animasi error sedang jalan
       if (puzzleCanvasGroup != null && !puzzleCanvasGroup.interactable) return;

        Debug.Log("Tombol Kerjakan Ditekan. Memulai Sequence Error...");

        // Jalankan urutan: Error Panel -> Tunggu -> Logic Dialog
        StartCoroutine(HandleSubmissionSequence());
    }

    private IEnumerator HandleSubmissionSequence()
    {
        // 1. Matikan Interaksi agar player tidak bisa drag item saat error
        SetPuzzleInteractive(false);

        // 2. Munculkan Error Panel (Sesuai request agar tersambung)
        if (errorPanel != null) errorPanel.SetActive(true);

        // 3. Tunggu durasi glitch/error (1.5 detik)
        yield return new WaitForSeconds(1.5f);

        // 4. Matikan Error Panel
        if (errorPanel != null) errorPanel.SetActive(false);

        // 5. Update Counter Percobaan
        attemptCounter++;
        Debug.Log("Percobaan Submit ke-" + attemptCounter);

        // 6. TENTUKAN DIALOG MANA YANG MUNCUL
        DialogueData dialogueToPlay = null;

        // KONDISI A: Sudah 3x Gagal (Overload / Give Up)
        if (attemptCounter >= 3)
        {
            dialogueToPlay = overloadDialogue;
            LevelLoader.instance.LoadNextScene("SC_Act2");
        }
        // KONDISI B: Belum 3x (Cari Random Forgotten Task)
        else
        {
            // Buat list tugas yang TIDAK ada di meja (occupiedSlots)
            List<TaskData> forgottenList = new List<TaskData>();

            // Ambil semua ID yang sedang terpasang di meja
            List<string> placedIDs = occupiedSlots.Values.ToList();

            foreach (var task in allTasksInLevel)
            {
                // Jika ID tugas ini TIDAK ada di meja -> Masukkan ke list forgotten
                if (!placedIDs.Contains(task.taskID))
                {
                    forgottenList.Add(task);
                }
            }

            // Jika ada tugas yang dilupakan, pilih satu secara random
            if (forgottenList.Count > 0)
            {
                int randomIndex = Random.Range(0, forgottenList.Count);
                dialogueToPlay = forgottenList[randomIndex].forgottenDialogue;
            }
        }

        // 7. Eksekusi Dialog
        if (dialogueToPlay != null)
        {
            Debug.Log("Memainkan Dialog: " + dialogueToPlay.name);
            dialogueManager.StartDialogue(dialogueToPlay);
        }
        else
        {
            // Fallback jika lupa assign dialog di inspector, kembalikan kontrol ke player
            Debug.LogWarning("Tidak ada dialog yang ditemukan (Null). Mengembalikan kontrol.");
            SetPuzzleInteractive(true);
        }
    }

    // --- FUNGSI BANTUAN (TIDAK DIUBAH BANYAK) ---

    private void ReboundPuzzle()
    {
        // Jika belum menyerah (3x), aktifkan lagi puzzle setelah dialog selesai
        if (attemptCounter < 3)
        {
            SetPuzzleInteractive(true);
        }
        // Jika sudah 3x, biasanya akan pindah scene atau logic lain (diatur Dialogue Data)
    }

    private void SetPuzzleInteractive(bool status)
    {
        if (puzzleCanvasGroup != null)
        {
            puzzleCanvasGroup.interactable = status;
            puzzleCanvasGroup.blocksRaycasts = status;
        }
    }

    public void SetPuzzleState(bool isVisible, bool isInteractable)
    {
        if (puzzleCanvasGroup != null)
        {
            puzzleCanvasGroup.alpha = isVisible ? 1f : 0f;
            puzzleCanvasGroup.interactable = isInteractable;
            puzzleCanvasGroup.blocksRaycasts = isInteractable;
        }
        if (puzzlePanel != null && !puzzlePanel.activeSelf)
            puzzlePanel.SetActive(true);
    }
}