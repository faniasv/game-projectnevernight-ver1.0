using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mengatur perilaku Minion khusus di Act 1.
/// Menangani patroli otomatis dan transisi ke mode dialog (tampak depan).
/// </summary>
public class MinionC_Act1 : MonoBehaviour
{
    [Header("Gerakan")]
    public float moveSpeed = 70f;      // Kecepatan jalan
    public float patrolRange = 100f;   // Jarak jalan ke kiri & kanan dari titik awal

    [Header("Referensi Visual")]
    public Image minionImage;          // Komponen Image Minion di UI
    public Sprite sideSprite;          // Sprite tampak samping (untuk patroli)
    public Sprite frontSprite;         // Sprite tampak depan (base untuk dialog)

    private RectTransform rectTrans;
    private float startX;
    private bool movingRight = true;
    private bool isPatrolling = true;  // Kontrol apakah Minion boleh jalan

    // Singleton agar mudah dipanggil dari DialogueManager atau Manager lainnya
    public static MinionC_Act1 instance;

    void Awake()
    {
        instance = this;
        rectTrans = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Catat posisi awal sebagai titik tengah (pusat patroli)
        startX = rectTrans.anchoredPosition.x;
        
        // Inisialisasi awal dengan sprite samping jika mulai dengan patroli
        if (sideSprite != null) minionImage.sprite = sideSprite;
    }

    void Update()
    {
        // Hanya jalankan fungsi Patrol jika status isPatrolling aktif
        if (isPatrolling)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (movingRight)
        {
            rectTrans.anchoredPosition += Vector2.right * moveSpeed * Time.deltaTime;

            // Jika sudah mencapai batas kanan -> Balik ke Kiri
            if (rectTrans.anchoredPosition.x >= startX + patrolRange)
            {
                movingRight = false;
                Flip(true);
            }
        }
        else
        {
            rectTrans.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;

            // Jika sudah mencapai batas kiri -> Balik ke Kanan
            if (rectTrans.anchoredPosition.x <= startX - patrolRange)
            {
                movingRight = true;
                Flip(false);
            }
        }
    }

    // --- FUNGSI KONTROL MODE ---

    /// <summary>
    /// Memanggil Minion ke tengah dan mengubah posisinya menjadi tampak depan.
    /// Panggil fungsi ini tepat saat pemain mengklik tombol 'Kerjakan' atau dialog mulai.
    /// </summary>
    public void GoToCenterAndFaceFront()
    {
        isPatrolling = false;
        
        // Teleport atau geser ke posisi tengah
        rectTrans.anchoredPosition = new Vector2(startX, rectTrans.anchoredPosition.y);
        
        // Ubah gambar ke tampak depan (default)
        if (frontSprite != null) minionImage.sprite = frontSprite;
        
        // Reset scale agar tidak ter-mirror (menghadap depan normal)
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    /// <summary>
    /// Mengembalikan Minion ke mode jalan/patroli.
    /// Panggil fungsi ini saat DialogueManager memberikan sinyal OnDialogueEnded.
    /// </summary>
    public void ResumePatrolling()
    {
        isPatrolling = true;
        
        // Kembalikan ke gambar tampak samping agar siap jalan lagi
        if (sideSprite != null) minionImage.sprite = sideSprite;
    }

    /// <summary>
    /// Fungsi untuk mengubah ekspresi secara dinamis.
    /// Dipanggil oleh DialogueManager menggunakan Sprite yang ada di DialogueData.
    /// </summary>
    public void SetExpression(Sprite newSprite)
    {
        if (minionImage != null && newSprite != null)
        {
            minionImage.sprite = newSprite;
        }
    }

    // Fungsi membalik gambar (Mirroring) menggunakan Scale
    void Flip(bool faceLeft)
    {
        Vector3 currentScale = transform.localScale;
        float absoluteX = Mathf.Abs(currentScale.x);

        // Ke kiri dikali -1, ke kanan dikali 1
        currentScale.x = absoluteX * (faceLeft ? -1 : 1);
        transform.localScale = currentScale;
    }
}