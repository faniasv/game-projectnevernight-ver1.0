using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Untuk mengakses komponen Slider
using TMPro; // Untuk mengakses komponen TextMeshPro
using System.Linq; // Untuk logika pencarian

// [Perubahan] Langkah 1: Buat "Database" untuk setiap tugas
[System.Serializable]
public class TaskDefinition
{
    [Tooltip("ID unik, harus sama dengan TaskID di DraggableItem")]
    public string taskID;

    [Tooltip("Nilai 'Load' untuk gameplay. (Set 50f agar 3x50 = 150)")]
    public float taskValue;

    [Tooltip("Tandai 'true' jika ini penting secara naratif")]
    public bool isNarrativelyImportant;

    [Tooltip("Dialog yang akan diputar JIKA tugas penting ini DILUPAKAN")]
    public DialogueData forgottenDialogue;
}
public class Puzzle1_Manager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider taskLoadSlider;
    [SerializeField] private GameObject errorPanel;

    [Header("Game Logic Variables")]
    private float currentLoad = 0f; // [cite: 21]
    [SerializeField] private float maxLoad = 150f; // 
    private int attemptCounter = 0; // [cite: 25]

    // [Perubahan] Langkah 2: "Database" baru
    [Header("Task Database")]
    [SerializeField] private List<TaskDefinition> allTasksInLevel;

    [Header("Dialogue System")]
    [SerializeField] private DialogueManager dialogueManager;

    // Dialog 'fallback' jika tidak ada tugas penting yang dilupakan
    [SerializeField] private DialogueData dialogueAttempt1;
    [SerializeField] private DialogueData dialogueAttempt2;
    [SerializeField] private DialogueData dialogueAttempt3;

    // Dictionary tetap sama, tapi sekarang hanya menyimpan ID
    private Dictionary<int, string> occupiedSlots = new Dictionary<int, string>();

    // Fungsi yang bisa dipanggil oleh script lain
    // [Perubahan] Langkah 3: Sederhanakan fungsi ini
    // Dia hanya perlu tahu 'slotPriority' dan 'taskID'
    public void OnTaskDroppedOnSlot(int slotPriority, string taskID)
    {
        // 1. Cari data lengkap untuk taskID ini dari database
        TaskDefinition taskData = GetTaskData(taskID);
        if (taskData == null)
        {
            Debug.LogError($"TaskID '{taskID}' tidak ditemukan di 'allTasksInLevel'!");
            return;
        }

        // 2. Catat di slot
        occupiedSlots[slotPriority] = taskID;
        UnityEngine.Debug.Log("GameManager mencatat: Slot " + slotPriority + " diisi oleh Task '" + taskID + "'");

        // 3. Update load berdasarkan data dari database
        taskLoadSlider.maxValue = maxLoad;
        currentLoad += taskData.taskValue;
        taskLoadSlider.value = currentLoad;
    }

    // [Perubahan] Langkah 4: Perbarui Logika Tombol Save
    public void OnSaveButtonPressed()
    {
        // 1. Cek Menang (Tidak berubah)
        if (occupiedSlots.ContainsKey(1) && occupiedSlots[1] == "Rest")
        {
            UnityEngine.Debug.Log("PUZZLE 1 SELESAI!");
            return;
        }

        // 2. Logika Gagal
        attemptCounter++;
        UnityEngine.Debug.Log("Tombol Save diklik! Percobaan ke-" + attemptCounter);
        StartCoroutine(ShowErrorPanelForSeconds(2f));

        // 3. Cek Gagal Total (Pindah Act)
        if (attemptCounter >= 3)
        {
            dialogueManager.StartDialogue(dialogueAttempt3);
            UnityEngine.Debug.Log("Sudah 5x gagal. Memicu transisi...");
            Invoke("TransitionToAct2", 4f);
            return;
        }

        // 4. LOGIKA NARATIF BARU (Percobaan 1 atau 2)

        // Buat daftar tugas yang DIPILIH pemain
        HashSet<string> placedTaskIDs = new HashSet<string>(occupiedSlots.Values);

        // Ulangi 'database' untuk mencari tugas PENTING yang DILUPAKAN
        foreach (TaskDefinition task in allTasksInLevel)
        {
            // Cek: Apakah tugas ini TIDAK ada di daftar 'placed' DAN 'isImportant' == true?
            if (!placedTaskIDs.Contains(task.taskID) && task.isNarrativelyImportant)
            {
                // DITEMUKAN!
                UnityEngine.Debug.Log($"Pemain melupakan tugas penting: {task.taskID}. Memutar dialog spesifik.");
                dialogueManager.StartDialogue(task.forgottenDialogue);

                // PENTING: Hentikan fungsi di sini agar kita hanya memutar SATU dialog spesifik
                return;
            }
        }

        // 5. FALLBACK DIALOGUE
        // Jika loop di atas selesai tanpa menemukan dialog (artinya pemain tidak melupakan 
        // tugas penting), putar dialog 'attempt' generik.
        UnityEngine.Debug.Log("Tidak ada tugas penting yang dilupakan. Memutar dialog fallback.");
        if (attemptCounter == 1)
        {
            dialogueManager.StartDialogue(dialogueAttempt1);
        }
        else if (attemptCounter == 2)
        {
            dialogueManager.StartDialogue(dialogueAttempt2);
        }
    }

    // [Perubahan] Langkah 5: Fungsi helper baru untuk mencari data di database
    private TaskDefinition GetTaskData(string taskID)
    {
        foreach (TaskDefinition task in allTasksInLevel)
        {
            if (task.taskID == taskID)
            {
                return task;
            }
        }
        return null; // Tidak ditemukan
    }

    // Fungsi transisi ke Act 2
    private void TransitionToAct2()
    {
        // Logika pindah scene
        Debug.Log("Pindah ke Act 2...");
    }

    // Coroutine (Tidak berubah)
    private IEnumerator ShowErrorPanelForSeconds(float seconds)
    {
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(seconds);
        errorPanel.SetActive(false);
    }
}