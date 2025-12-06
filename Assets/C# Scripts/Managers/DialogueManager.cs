using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueTextUI;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    [SerializeField] private Image characterImagePanel;

    // --- [PERBAIKAN ERROR CS0103] ---
    [Header("Animation")]
    [SerializeField] private Animator animator; // Variabel ini sebelumnya hilang!
    // --------------------------------

    [Header("Character Styling")]
    [SerializeField] private Color minionColor = Color.yellow;
    [SerializeField] private Color avaColor = Color.cyan;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] typingSounds;
    [Range(1, 5)][SerializeField] private int frequencyLevel = 2;
    [SerializeField] private AudioSource dialogueAudioSource;
    [Range(0f, 1f)][SerializeField] private float typingVolume = 0.5f;

    [Header("Settings")]
    [SerializeField] public float charactersPerSecond = 25f;

    // Internal State
    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    // Variabel ini untuk menyimpan teks penuh yang akan diketik
    private string currentFullLine;

    private bool shouldKeepPanelOpen = false;

    public static event Action OnDialogueEnded;

    void Awake()
    {
        if (dialogueAudioSource == null)
        {
            dialogueAudioSource = GetComponent<AudioSource>();
            if (dialogueAudioSource == null) dialogueAudioSource = gameObject.AddComponent<AudioSource>();
        }
        if (characterImagePanel != null) characterImagePanel.gameObject.SetActive(false);
    }

    void Update()
    {
        // Input hanya jalan jika dialog aktif
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            if (isTyping)
            {
                CompleteLine(); // Skip ketikan
            }
            else
            {
                DisplayNextLine(); // Lanjut baris
            }
        }
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        if (isDialogueActive) return;

        isDialogueActive = true;
        shouldKeepPanelOpen = dialogueData.keepPanelOpenAtEnd;

        dialogueQueue.Clear();

        foreach (DialogueLine line in dialogueData.lines)
        {
            dialogueQueue.Enqueue(line);
        }

        // Buka Panel Animasi
        if (animator != null) animator.SetBool("IsOpen", true);
        else if (dialoguePanel != null) dialoguePanel.SetActive(true); // Fallback jika animator null

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = dialogueQueue.Dequeue();
        StopAllCoroutines();

        // --- [PERBAIKAN BUG TEKS HILANG] ---
        // Kita harus isi variabel string ini, kalau tidak Coroutine ngetik angin (null)
        currentFullLine = currentLine.lineText;
        // -----------------------------------

        if (nameTextUI != null) nameTextUI.text = currentLine.characterName;

        if (currentLine.commands != null) ProcessCommands(currentLine.commands);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogueTextUI.text = "";

        // Styling Warna
        string charName = line.characterName.ToUpper().Trim();
        if (charName == "MINION")
        {
            dialogueTextUI.color = minionColor;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineRight;
        }
        else if (charName == "AVA")
        {
            dialogueTextUI.color = avaColor;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineLeft;
        }
        else
        {
            dialogueTextUI.color = Color.white;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineLeft;
        }

        // Efek Mengetik
        char[] charArray = currentFullLine.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            dialogueTextUI.text += charArray[i];

            // Suara ketikan
            if (charArray[i] != ' ' && i % frequencyLevel == 0)
            {
                PlayTypingSound();
            }

            yield return new WaitForSeconds(1f / charactersPerSecond);
        }

        isTyping = false;
    }

    private void CompleteLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueTextUI.text = currentFullLine;
        isTyping = false;
    }

    private void PlayTypingSound()
    {
        if (dialogueAudioSource != null && typingSounds != null && typingSounds.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, typingSounds.Length);
            dialogueAudioSource.PlayOneShot(typingSounds[index], typingVolume);
        }
    }

    private void ProcessCommands(GameCommand[] commands)
    {
        foreach (GameCommand cmd in commands)
        {
            if (cmd.type == CommandType.PlaySound && cmd.audioAsset != null)
            {
                dialogueAudioSource.PlayOneShot(cmd.audioAsset);
            }
            else if (cmd.type == CommandType.ChangeBackground && cmd.spriteAsset != null)
            {
                characterImagePanel.sprite = cmd.spriteAsset;
                characterImagePanel.gameObject.SetActive(true);
            }
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        // Kalau TIDAK dicentang keep open, baru tutup panel
        if (!shouldKeepPanelOpen)
        {
            if (animator != null) animator.SetBool("IsOpen", false);
            else if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }

        OnDialogueEnded?.Invoke();
    }
}