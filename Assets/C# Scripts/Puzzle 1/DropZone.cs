using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    // Variable baru untuk prioritas slot
    public int priorityLevel;
    // ------------------------------------------------
    // VARIABEL BARU UNTUK REFERENSI KE MANAGER
    [SerializeField] private Puzzle1_Manager puzzleManager;

    public void OnDrop(PointerEventData eventData)
    {
        UnityEngine.Debug.Log("Item di-drop di atas slot dengan prioritas: " + priorityLevel); // Di-update untuk testing

        GameObject droppedObject = eventData.pointerDrag;
        DraggableItem draggableItem = droppedObject.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            // Pindahkan posisi item yang di-drop agar sama persis dengan posisi slot ini
            droppedObject.transform.position = this.transform.position;
            // Cetak Task ID dan Value untuk memastikan semuanya masih bekerja
            puzzleManager.OnTaskDroppedOnSlot(priorityLevel, draggableItem.TaskID);
        }
    }
}