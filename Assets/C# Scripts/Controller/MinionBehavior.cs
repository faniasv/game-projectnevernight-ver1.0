using UnityEngine;
using UnityEngine.UI;

public class MinionController : MonoBehaviour
{
    [Header("Gerakan")]
    public float moveSpeed = 70f;      // Kecepatan jalan
    public float patrolRange = 100f;   // Jarak jalan ke kiri & kanan dari titik awal

    [Header("Referensi Visual")]
    public Image minionImage;          // Masukkan Image Minion di sini

    private RectTransform rectTrans;
    private float startX;
    private bool movingRight = true;

    // Singleton biar gampang dipanggil dari Dialog buat ganti ekspresi
    public static MinionController instance;

    void Awake()
    {
        instance = this;
        rectTrans = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Catat posisi awal sebagai titik tengah
        startX = rectTrans.anchoredPosition.x;
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        // 1. Gerakkan Minion
        if (movingRight)
        {
            rectTrans.anchoredPosition += Vector2.right * moveSpeed * Time.deltaTime;

            // Kalau sudah mentok kanan -> Balik Kiri
            if (rectTrans.anchoredPosition.x >= startX + patrolRange)
            {
                movingRight = false;
                Flip(true);
            }
        }
        else
        {
            rectTrans.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;

            // Kalau sudah mentok kiri -> Balik Kanan
            if (rectTrans.anchoredPosition.x <= startX - patrolRange)
            {
                movingRight = true;
                Flip(false);
            }
        }
    }

    // Fungsi membalik gambar (Mirroring)
    void Flip(bool faceLeft)
    {
        // 1. Ambil scale saat ini
        Vector3 currentScale = transform.localScale;

        // 2. Pastikan kita ambil nilai POSITIF-nya dulu (ukuran asli tanpa arah)
        float absoluteX = Mathf.Abs(currentScale.x);

        // 3. Tentukan arah: Kalau ke kiri dikali -1, kalau kanan dikali 1
        currentScale.x = absoluteX * (faceLeft ? -1 : 1);

        // 4. Terapkan kembali
        transform.localScale = currentScale;
    }

    // Panggil fungsi ini kalau mau ganti ekspresi minion lewat script lain
    public void SetExpression(Sprite newSprite)
    {
        if (minionImage != null && newSprite != null)
        {
            minionImage.sprite = newSprite;
        }
    }
}