using UnityEngine;

public class Act1_Controller : MonoBehaviour
{
    [Header("Visual References")]
    public GameObject minionPanel;     // Drag 'Minion_Panel'
    public GameObject avaPanel;     // Drag 'Player_Panel'
    public GameObject dialoguePanel;   // Drag 'Dialogue_Panel'
    public GameObject puzzle1Container;  // Drag 'Puzzle1_Container'

    [Header("Data")]
    public DialogueData introDialogue; // Drag 'Dlg_Act1_Intro'

    // Dipanggil saat Intro selesai
    public void StartAct1()
    {
        // 1. Nyalakan Minion
        if (minionPanel != null) minionPanel.SetActive(true);

        // 2. Nyalakan Ava
        if (avaPanel != null) avaPanel.SetActive(true);

        // 3. Nyalakan Kotak Dialog
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // 4. Suruh Dialogue Manager mulai ngetik teks
        FindObjectOfType<DialogueManager>().StartDialogue(introDialogue, () => OnIntroDialogueFinished());
    }

    void OnIntroDialogueFinished()
    {
        Debug.Log("Dialog Selesai. Memunculkan Puzzle...");
        if (minionPanel != null) minionPanel.SetActive(false);
        // MUNCULKAN PUZZLE!
        if (puzzle1Container != null) puzzle1Container.SetActive(true);
    }
}