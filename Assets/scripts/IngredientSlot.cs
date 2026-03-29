using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IngredientSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ce ingredient tine acest buton?")]
    public ItemData dateIngredient; 
    
    [Header("Referinta vizuala")]
    public Image imagineIngredient; 

    void Start()
    {
        if (dateIngredient != null && imagineIngredient != null)
        {
            imagineIngredient.sprite = dateIngredient.iconita;
        }
    }

    public void LaClick()
    {
        if (dateIngredient != null)
        {
            CraftingManager.Instance.AdaugaIngredient(dateIngredient);
            TooltipManager.Instance.AscundeTooltip();
        }
    }

    // ==========================================
    // FUNCȚIILE PENTRU EVENT TRIGGER (Planul B)
    // ==========================================
    public void ArataNumele()
    {
        if (dateIngredient != null)
        {
            TooltipManager.Instance.ArataTooltip(dateIngredient.numeItem, transform.position);
        }
    }

    public void AscundeNumele()
    {
        TooltipManager.Instance.AscundeTooltip();
    }

    // ==========================================
    // DETECȚIA AUTOMATĂ (Planul A)
    // ==========================================
    public void OnPointerEnter(PointerEventData eventData)
    {
        ArataNumele(); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AscundeNumele();
    }
}