using UnityEngine;

// Attribute ini akan membuat opsi "Nevernight/Dialogue" muncul di menu Create Unity
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Nevernight/Dialogue")]
public class DialogueData : ScriptableObject
{
    // Sebuah array yang berisi semua baris dialog dalam satu percakapan
    public DialogueLine[] lines;
}

// Struct ini mendefinisikan struktur dari SATU baris dialog
[System.Serializable]
public struct DialogueLine
{
    public string characterName; // Nama karakter (misal: "Minion", "Ava")

    [TextArea(3, 10)] // Membuat field ini menjadi kotak teks yang lebih besar di Inspector
    public string lineText; // Isi dialognya

    // Ini adalah implementasi dari ide command Anda
    public GameCommand[] commands;
}

// Struct baru untuk perintah (command)
[System.Serializable]
public struct GameCommand
{
    public CommandType type;
    public string value;
    public AudioClip audioAsset;
    public Sprite spriteAsset;
}

// Enum untuk jenis perintah yang tersedia (lebih aman dari string biasa)
public enum CommandType
{
    PlaySound,
    ChangeBackground,
    StartPuzzle,
    // (Bisa tambahkan jenis command lain di sini nanti)
}