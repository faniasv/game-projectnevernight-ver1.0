using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[System.Serializable]
public class TaskDefinition
{
    public string taskID;
    public float taskValue;
    public bool isNarrativelyImportant;
    public DialogueData forgottenDialogue;
}

public class Puzzle1_Manager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup puzzleCanvasGroup;
    [SerializeField] private Slider taskLoadSlider;
    [SerializeField] private GameObject errorPanel;

    [Header("Game Logic")]
    [SerializeField] private float maxLoad = 150f;
    private float currentLoad = 0f;
    private int attemptCounter = 0;
    private Dictionary<int, string> occupiedSlots = new Dictionary<int, string>();
    private bool isGameEnding = false;

    [Header("Data")]
    [SerializeField] private List<TaskDefinition> allTasksInLevel;

    [Header("Dialogue System Connection")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData dialogueAttempt1;
    [SerializeField] private DialogueData dialogueAttempt2;
    [SerializeField] private DialogueData dialogueAttempt3;

    // --- EVENT LISTENER ---
    private void OnEnable()
    {
        DialogueManager.OnDialogueEnded += HandleDialogueEnded;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEnded -= HandleDialogueEnded;
    }

    private void HandleDialogueEnded()
    {
        // Kalau game belum over, nyalakan puzzle lagi setelah dialog selesai
        if (!isGameEnding)
        {
            SetPuzzleState(true, true);
        }
    }
    // -----------------------

    void Start()
    {
        if (taskLoadSlider)
        {
            taskLoadSlider.maxValue = maxLoad;
            taskLoadSlider.value = 0;
        }

        if (puzzleCanvasGroup == null)
            Debug.LogError("Puzzle1_Manager: CanvasGroup KOSONG! Drag Puzzle1_Panel ke sini.");
    }

    public void OnTaskDroppedOnSlot(int slotPriority, string taskID)
    {
        if (occupiedSlots.ContainsKey(slotPriority))
            occupiedSlots[slotPriority] = taskID;
        else
            occupiedSlots.Add(slotPriority, taskID);

        RecalculateTotalLoad();
    }

    public void OnTaskRemovedFromSlot(int slotPriority)
    {
        if (occupiedSlots.ContainsKey(slotPriority))
        {
            occupiedSlots.Remove(slotPriority);
        }
        RecalculateTotalLoad();
    }

    private void RecalculateTotalLoad()
    {
        currentLoad = 0f;
        foreach (var slot in occupiedSlots)
        {
            TaskDefinition data = GetTaskData(slot.Value);
            if (data != null) currentLoad += data.taskValue;
        }

        if (taskLoadSlider != null) taskLoadSlider.value = currentLoad;
    }

    public void OnSaveButtonPressed()
    {
        if (isGameEnding) return;

        // 1. CEK MENANG
        if (occupiedSlots.ContainsKey(1) && occupiedSlots[1] == "Rest")
        {
            Debug.Log("WIN: Act 1 Selesai!");
            return;
        }

        // 2. JIKA GAGAL
        attemptCounter++;
        StartCoroutine(ShowErrorPanelRoutine());

        // Matikan puzzle sementara dialog jalan
        SetPuzzleState(true, false);

        // 3. CEK 3x GAGAL
        if (attemptCounter >= 3)
        {
            isGameEnding = true;
            dialogueManager.StartDialogue(dialogueAttempt3);
            Invoke("TransitionToAct2", 4f);
            return;
        }

        // 4. CEK LOGIKA NARATIF (Lupa tugas penting)
        HashSet<string> currentTableTasks = new HashSet<string>(occupiedSlots.Values);

        foreach (TaskDefinition task in allTasksInLevel)
        {
            if (task.isNarrativelyImportant && !currentTableTasks.Contains(task.taskID))
            {
                dialogueManager.StartDialogue(task.forgottenDialogue);
                return;
            }
        }

        // 5. DIALOG GENERIK
        if (attemptCounter == 1) dialogueManager.StartDialogue(dialogueAttempt1);
        else if (attemptCounter == 2) dialogueManager.StartDialogue(dialogueAttempt2);
    }

    private TaskDefinition GetTaskData(string id)
    {
        foreach (var task in allTasksInLevel)
        {
            if (task.taskID == id) return task;
        }
        return null;
    }

    private void TransitionToAct2()
    {
        Debug.Log(">> PINDAH SCENE KE ACT 2 <<");
        // SceneManager.LoadScene("SC_Act2");
    }

    private IEnumerator ShowErrorPanelRoutine()
    {
        if (errorPanel) errorPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (errorPanel) errorPanel.SetActive(false);
    }

    // FUNGSI PENGATUR VISUAL & INTERAKSI
    public void SetPuzzleState(bool isVisible, bool isInteractable)
    {
        if (puzzleCanvasGroup != null)
        {
            puzzleCanvasGroup.alpha = isVisible ? 1f : 0f;
            puzzleCanvasGroup.blocksRaycasts = isVisible && isInteractable;
            puzzleCanvasGroup.interactable = isVisible && isInteractable;
        }
    }
}