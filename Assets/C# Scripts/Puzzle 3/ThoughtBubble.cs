using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Skrip untuk setiap gelembung pikiran. 
/// Mengatur pergerakan dan interaksi klik.
/// </summary>
public class ThoughtBubble : MonoBehaviour, IPointerClickHandler
{
    [Header("Internal UI")]
    public Image bubbleImage;           // Komponen Image gelembung
    public TextMeshProUGUI textComp;    // Komponen Text di dalam gelembung

    private bool isCorrectAnswer;       // Apakah ini jawaban benar?
    private Puzzle3_Manager manager;    // Referensi ke boss manager
    private float moveSpeed;            // Kecepatan lari
    private float destroyX;             // Batas koordinat untuk hancur
    private bool isReady = false;

    /// <summary>
    /// Fungsi inisialisasi yang dipanggil oleh Manager saat Instantiate.
    /// </summary>
    public void Setup(Color col, string txt, bool correct, Puzzle3_Manager mngr, float speed, float endX)
    {
        if (bubbleImage != null) bubbleImage.color = col;
        if (textComp != null) textComp.text = txt;
        
        isCorrectAnswer = correct;
        manager = mngr;
        moveSpeed = speed;
        destroyX = endX;
        
        isReady = true;
    }

    void Update()
    {
        // Jangan bergerak kalau data belum siap atau manager sedang 'Freeze'
        if (!isReady || manager.isFrozen) return;

        // Gerak lurus ke arah kanan
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        // Hancurkan jika sudah melewati batas titik EndPoint
        if (transform.localPosition.x > destroyX)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Mendeteksi klik pada gelembung (Membutuhkan Physics2D Raycaster pada Canvas)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isReady || manager.isFrozen) return;

        // Lapor ke manager hasil kliknya
        manager.OnBubbleClicked(isCorrectAnswer);
        
        // Langsung hancurkan setelah diklik
        Destroy(gameObject);
    }

    internal void Setup(Sprite chosenSprite, string chosenText, bool isGood, Puzzle3_Manager puzzle3_Manager, float scrollSpeed, float x)
    {
        throw new NotImplementedException();
    }
}