using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel; // Referensi ke panel dialog utama
    [SerializeField] private TextMeshProUGUI dialogueTextUI; // Referensi ke UI Teks
    [SerializeField] private Image characterImagePanel; // Asset Ekspresi 
    [SerializeField] private Color minionColor; // Warna teks untuk Minion
    [SerializeField] private Color avaColor; // Warna teks untuk Ava
    [SerializeField] private AudioSource dialogueAudioSource; // Sumber audio untuk efek suara dialog

    private Queue<DialogueLine> dialogueQueue; // Antrian untuk menyimpan baris dialog
    private bool isDialogueActive = false; // Variable untuk melacak status dialog
    private bool isTyping = false; // Variable untuk melacak status pengetikan
    private Coroutine typingCoroutine; // Untuk menyimpan referensi ke coroutine yang sedang berjalan
    private string currentFullLine; // Untuk menyimpan teks lengkap dari baris saat ini


    [SerializeField] public float charactersPerSecond = 60f; // Kecepatan pengetikan dialog bisa diatur di Inspector


    void Awake()
    {
        dialogueQueue = new Queue<DialogueLine>();

        // Pastikan panel gambar tersembunyi saat mulai
        if (characterImagePanel != null)
        {
            characterImagePanel.gameObject.SetActive(false);
        }

        // Coba cari AudioSource jika tidak di-set
        if (dialogueAudioSource == null)
        {
            dialogueAudioSource = GetComponent<AudioSource>();
            if (dialogueAudioSource == null)
            {
                // Jika masih tidak ada, buat satu agar tidak error
                dialogueAudioSource = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning("DialogueAudioSource tidak di-set, membuat satu secara otomatis.");
            }
        }
    }

    void Update()
    {
        // Hanya cek input jika dialog sedang aktif
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            // Jika kita sedang mengetik...
            if (isTyping)
            {
                // ...maka hentikan efek mengetik dan langsung tampilkan teks lengkapnya.
                CompleteLine();
            }
            else
            {
                // ...jika kita TIDAK sedang mengetik, baru tampilkan baris berikutnya.
                DisplayNextLine();
            }
        }
    }

    // Fungsi ini akan dipanggil oleh script lain (seperti di Puzzle1_Manager saat attempt ke-1 dan ke-2)
    public void StartDialogue(DialogueData dialogue)
    {
        isDialogueActive = true; // Aktifkan status dialog
        dialoguePanel.SetActive(true); // Tampilkan panel dialog
        dialogueQueue.Clear(); // Kosongkan antrian dialog lama

        // Masukkan semua baris dari "Kartu Resep" ke dalam antrian
        foreach (DialogueLine line in dialogue.lines)
        {
            dialogueQueue.Enqueue(line);
        }

        DisplayNextLine(); // Tampilkan baris pertama
    }

    // Fungsi untuk menampilkan baris dialog selanjutnya
    public void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = dialogueQueue.Dequeue();

        // Simpan teks lengkapnya untuk keperluan skip
        currentFullLine = currentLine.lineText;

        // Hentikan coroutine lama jika ada, lalu mulai yang baru dan simpan referensinya
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine(currentLine));


        // Jalankan semua perintah yang ada di baris ini
        ProcessCommands(currentLine.commands);
    }

    // Fungsi untuk menyelesaikan pengetikan teks secara instan
    private void CompleteLine()
    {
        // Hentikan coroutine pengetikan
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Tampilkan teks lengkapnya secara instan
        dialogueTextUI.text = currentFullLine;

        // Buka "kunci" agar klik berikutnya bisa lanjut ke baris selanjutnya
        isTyping = false;
    }

    // Buat fungsi Coroutine untuk efek mengetik
    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogueTextUI.text = "";

        // Logika warna dan alignment tetap di sini
        // Gunakan .ToUpper().Trim() untuk membuatnya tidak case-sensitive dan menghapus spasi
        string character = line.characterName.ToUpper().Trim();

        if (character == "MINION")
        {
            dialogueTextUI.color = minionColor;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineRight; 
        }
        else if (character == "AVA") // Kita periksa "AVA" secara eksplisit
        {
            dialogueTextUI.color = avaColor;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineLeft;
        }
        else
        {
            // Fallback jika nama karakter tidak dikenali
            dialogueTextUI.color = Color.white; // Warna putih sebagai default
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineJustified;
            UnityEngine.Debug.LogWarning("Nama karakter tidak dikenali: '" + line.characterName + "'"); // Pesan ini akan membantu debugging di masa depan
        }

        // Gunakan currentFullLine yang sudah kita simpan (yang sekarang tidak ada nama karakternya)
        foreach (char letter in currentFullLine.ToCharArray())
        {
            dialogueTextUI.text += letter;
            float typingSpeed = 1f / charactersPerSecond;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    // Fungsi untuk menjalankan perintah dari data dialog
    private void ProcessCommands(GameCommand[] commands)
    {
        foreach (GameCommand command in commands)
        {
            switch (command.type)
            {
                case CommandType.PlaySound:
                    if (command.audioAsset != null && dialogueAudioSource != null)
                    {
                        dialogueAudioSource.PlayOneShot(command.audioAsset);
                    }
                    else
                    {
                        Debug.LogWarning("PlaySound gagal: audioAsset atau dialogueAudioSource belum di-set!");
                    }
                    break;

                case CommandType.StartPuzzle:
                    // Anda masih menggunakan 'command.value' di sini, 
                    // pastikan di struct GameCommand Anda, field string-nya bernama 'value'
                    UnityEngine.Debug.Log("COMMAND DITERIMA: Memulai Puzzle -> " + command.value);
                    break;

                case CommandType.ChangeBackground:
                    if (command.spriteAsset != null && characterImagePanel != null)
                    {
                        characterImagePanel.sprite = command.spriteAsset;
                        characterImagePanel.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning("ChangeBackground gagal: spriteAsset atau characterImagePanel belum di-set!");
                    }
                    break;
            }
        }
    }

    // Fungsi yang berjalan saat dialog selesai
    private void EndDialogue()
    {
        // isDialogueActive = false; // Nonaktifkan status dialog
        // dialoguePanel.SetActive(false); // Sembunyikan panel dialog
        dialogueTextUI.text = ""; // Kosongkan teks
        UnityEngine.Debug.Log("Dialog Selesai.");
    }
}