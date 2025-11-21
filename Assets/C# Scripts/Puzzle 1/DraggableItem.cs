using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    // Variabel untuk data tugas
    [Header("Task Data")]
    public string TaskID;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Membuat object sedikit transparan saat di-drag
        canvasGroup.alpha = 0.6f;
        // Mematikan blocksRaycasts agar DropZone bisa terdeteksi
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Menggunakan logika delta yang sudah benar
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Mengembalikan object ke kondisi normal
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}