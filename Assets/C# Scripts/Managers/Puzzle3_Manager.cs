using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Kelas bantuan untuk membuat "Paket" soal di Inspector
[System.Serializable]
public class QuestionPackage
{
    [TextArea(2, 3)]
    public string questionText;     // Kalimat di papan kuning
    public List<string> goodAnswers; // Jawaban benar (Lanjut/Skor)
    public List<string> badAnswers;  // Jawaban salah (Freeze/Merah)
}

public class Puzzle3_Manager : MonoBehaviour
{
    [Header("Konfigurasi Paket Soal")]
    public List<QuestionPackage> allPackages; // Daftar soal (Isi di Inspector)
    
    [Header("Aset Visual")]
    [Tooltip("Masukkan daftar gambar gelembung warna-warni dari Figma ke sini")]
    public List<Sprite> bubbleSprites;        // Pilihan gambar gelembung
    
    [Header("Referensi UI")]
    public TextMeshProUGUI boardText;        // Teks di Question Panel
    public Image trafficLight;               // Gambar lampu (Merah/Hijau)
    public GameObject bubblePrefab;          // Prefab gelembung
    public RectTransform spawnPoint;         // Titik muncul (Kiri luar)
    public RectTransform endPoint;           // Titik hancur (Kanan luar)

    [Header("Pengaturan Gameplay & Jalur")]
    public float laneDistance = 120f;        // Jarak antar 3 jalur (Atas/Tengah/Bawah)
    public float scrollSpeed = 250f;         // Kecepatan jalan gelembung
    public float spawnInterval = 1.2f;       // Jeda muncul gelembung
    public int targetScorePerLevel = 5;      // Butuh berapa jawaban benar?

    // Status Internal
    [HideInInspector] public bool isFrozen = false;
    private int currentPackageIndex = 0;
    private int currentScore = 0;
    private QuestionPackage currentPackage;

    void Start()
    {
        if (allPackages.Count > 0)
        {
            LoadPackage(0);
            StartCoroutine(SpawnLoop());
        }
        else
        {
            Debug.LogError("List All Packages masih kosong! Isi dulu di Inspector.");
        }
        
        SetTrafficLight(true); // Mulai dengan lampu Hijau
    }

    // Mengganti soal ke paket tertentu
    public void LoadPackage(int index)
    {
        if (index >= allPackages.Count)
        {
            WinGame();
            return;
        }

        currentPackageIndex = index;
        currentPackage = allPackages[index];
        boardText.text = currentPackage.questionText;
        currentScore = 0;
        Debug.Log($"[Puzzle 3] Memuat Paket {index + 1}: {currentPackage.questionText}");
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!isFrozen)
            {
                SpawnBubble();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBubble()
    {
        // Munculkan gelembung di posisi SpawnPoint
        if (bubblePrefab == null || spawnPoint == null) return;

        GameObject go = Instantiate(bubblePrefab, spawnPoint.parent);
        
        // --- LOGIKA 3 JALUR ---
        // Pilih jalur acak: -1 (Bawah), 0 (Tengah), atau 1 (Atas)
        int randomLane = Random.Range(-1, 2); 
        float yOffset = randomLane * laneDistance;

        // Atur posisi awal (Local Position agar relatif terhadap parent/panel)
        Vector3 startPos = spawnPoint.localPosition;
        startPos.y += yOffset;
        go.transform.localPosition = startPos;

        ThoughtBubble script = go.GetComponent<ThoughtBubble>();

        // --- LOGIKA IDENTITAS (BENAR/SALAH) ---
        bool isGood = Random.value > 0.5f;
        string chosenText = "";

        if (isGood && currentPackage.goodAnswers.Count > 0)
            chosenText = currentPackage.goodAnswers[Random.Range(0, currentPackage.goodAnswers.Count)];
        else if (currentPackage.badAnswers.Count > 0)
            chosenText = currentPackage.badAnswers[Random.Range(0, currentPackage.badAnswers.Count)];
        else
            chosenText = "...";

        // --- LOGIKA SPRITE (GAMBAR FIGMA) ---
        // Mengambil gambar acak dari list Sprite
        Sprite chosenSprite = (bubbleSprites != null && bubbleSprites.Count > 0) ? 
            bubbleSprites[Random.Range(0, bubbleSprites.Count)] : null;

        // Inisialisasi gelembung (Kirim Sprite ke script ThoughtBubble)
        if (script != null)
        {
            script.Setup(chosenSprite, chosenText, isGood, this, scrollSpeed, endPoint.localPosition.x);
        }
    }

    public void OnBubbleClicked(bool isCorrect)
    {
        if (isFrozen) return;

        if (isCorrect)
        {
            currentScore++;
            Debug.Log($"Skor: {currentScore}/{targetScorePerLevel}");
            
            if (currentScore >= targetScorePerLevel)
            {
                NextLevel();
            }
        }
        else
        {
            StartCoroutine(FreezeSystem());
        }
    }

    IEnumerator FreezeSystem()
    {
        isFrozen = true;
        SetTrafficLight(false); // Lampu Merah (Mental Block)
        Debug.Log("[Puzzle 3] Salah! Sistem membeku sejenak.");

        yield return new WaitForSeconds(2f); // Durasi membeku

        SetTrafficLight(true); // Kembali Hijau
        isFrozen = false;
    }

    void SetTrafficLight(bool isGreen)
    {
        if (trafficLight != null)
            trafficLight.color = isGreen ? Color.green : Color.red;
    }

    void NextLevel()
    {
        currentPackageIndex++;
        LoadPackage(currentPackageIndex);
    }

    void WinGame()
    {
        StopAllCoroutines();
        isFrozen = true;
        Debug.Log("[Puzzle 3] SELESAI! Ava merasa lebih baik.");
    }
}