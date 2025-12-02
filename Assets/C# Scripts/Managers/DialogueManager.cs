using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System; // Penting untuk Action

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel; // Referensi ke panel dialog utama
    [SerializeField] private TextMeshProUGUI dialogueTextUI; // Referensi ke UI Teks Konten
    [SerializeField] private TextMeshProUGUI nameTextUI; // Referensi ke UI Teks Nama (Opsional)
    [SerializeField] private Image characterImagePanel; // Asset Ekspresi (Opsional)

    [Header("Character Styling")]
    [SerializeField] private Color minionColor = Color.yellow; // Warna teks untuk Minion
    [SerializeField] private Color avaColor = Color.cyan; // Warna teks untuk Ava

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] typingSounds; // Array variasi suara ketikan
    [Range(1, 5)]
    [SerializeField] private int frequencyLevel = 2; // Bunyi setiap 2 huruf
    [SerializeField] private AudioSource dialogueAudioSource; // Sumber audio
    [Range(0f, 1f)]
    [SerializeField] private float typingVolume = 0.5f;

    [Header("Settings")]
    [SerializeField] public float charactersPerSecond = 25f; // Kecepatan ngetik

    // Internal State
    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentFullLine;

    // Callback: Siapa yang harus ditelepon balik saat dialog selesai?
    private Action onDialogueFinished;

    void Awake()
    {
        // Setup awal Audio Source jika lupa dipasang
        if (dialogueAudioSource == null)
        {
            dialogueAudioSource = GetComponent<AudioSource>();
            if (dialogueAudioSource == null)
            {
                dialogueAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Matikan panel gambar di awal
        if (characterImagePanel != null) characterImagePanel.gameObject.SetActive(false);
    }

    void Update()
    {
        // Cek input hanya jika panel aktif
        if (dialoguePanel.activeInHierarchy && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            if (isTyping)
            {
                // Kalau sedang ngetik -> Langsung selesaikan (Skip)
                CompleteLine();
            }
            else
            {
                // Kalau sudah selesai ngetik -> Lanjut baris berikutnya
                DisplayNextLine();
            }
        }
    }

    // ==========================================================
    // FUNGSI UTAMA (Dipanggil oleh Controller/Manager lain)
    // ==========================================================

    public void StartDialogue(DialogueData data, Action onFinished = null)
    {
        // 1. Simpan callback (siapa yang manggil)
        onDialogueFinished = onFinished;

        // 2. Reset State
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        dialogueQueue.Clear();

        // 3. Masukkan semua baris ke antrian
        foreach (var line in data.lines)
        {
            dialogueQueue.Enqueue(line);
        }

        // 4. Mulai baris pertama
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        // Cek apakah antrian habis?
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Ambil data baris saat ini
        DialogueLine currentLine = dialogueQueue.Dequeue();
        currentFullLine = currentLine.lineText;

        // Set Nama di UI (jika ada)
        if (nameTextUI != null) nameTextUI.text = currentLine.characterName;

        // Jalankan Commands (Ganti BG, SFX, dll) sebelum teks muncul
        if (currentLine.commands != null)
        {
            ProcessCommands(currentLine.commands);
        }

        // Mulai Animasi Mengetik
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    // ==========================================================
    // LOGIKA PENGETIKAN & VISUAL
    // ==========================================================

    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogueTextUI.text = "";

        // --- Styling Karakter (Kiri/Kanan & Warna) ---
        string charName = line.characterName.ToUpper().Trim();

        if (charName == "MINION")
        {
            dialogueTextUI.color = minionColor;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineRight; // Minion di Kanan
        }
        else if (charName == "AVA")
        {
            dialogueTextUI.color = avaColor;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineLeft; // Ava di Kiri
        }
        else
        {
            dialogueTextUI.color = Color.white;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineLeft;
        }

        // --- Loop Huruf per Huruf ---
        char[] charArray = currentFullLine.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            dialogueTextUI.text += charArray[i];

            // Efek Audio
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

        dialogueTextUI.text = currentFullLine; // Tampilkan teks penuh
        isTyping = false;
    }

    private void PlayTypingSound()
    {
        if (dialogueAudioSource != null && typingSounds != null && typingSounds.Length > 0)
        {
            // Random variasi suara
            int index = UnityEngine.Random.Range(0, typingSounds.Length);

            // Random pitch biar organik (ala Animal Crossing/Celeste)
            dialogueAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            dialogueAudioSource.PlayOneShot(typingSounds[index], typingVolume);
        }
    }

    // ==========================================================
    // LOGIKA COMMAND & ENDING
    // ==========================================================

    private void ProcessCommands(GameCommand[] commands)
    {
        foreach (GameCommand cmd in commands)
        {
            switch (cmd.type)
            {
                case CommandType.PlaySound:
                    if (cmd.audioAsset != null) dialogueAudioSource.PlayOneShot(cmd.audioAsset);
                    break;

                case CommandType.ChangeBackground:
                    if (cmd.spriteAsset != null && characterImagePanel != null)
                    {
                        characterImagePanel.sprite = cmd.spriteAsset;
                        characterImagePanel.gameObject.SetActive(true);
                    }
                    break;

                case CommandType.StartPuzzle:
                    Debug.Log("Command Start Puzzle diterima: " + cmd.value);
                    // Logika trigger puzzle bisa ditambahkan di sini jika perlu direct access
                    break;
            }
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false); // Tutup panel
        dialogueTextUI.text = "";

        Debug.Log("Dialog Selesai. Memanggil Callback...");

        // PENTING: Kabari script pemanggil (Act1_Controller) bahwa dialog sudah beres
        onDialogueFinished?.Invoke();
    }
}