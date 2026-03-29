using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject obiectTooltip;
    public TextMeshProUGUI textTooltip;

    void Awake()
    {
        Instance = this;
        obiectTooltip.SetActive(false); // Il ascundem la inceput
    }

    // Am sters toata functia Update() cu Input.mousePosition! Nu mai urmareste mouse-ul nicaieri.

    // Acum ii cerem si "pozitia" butonului cand afisam textul
    public void ArataTooltip(string mesaj, Vector3 pozitieButon)
    {
        textTooltip.text = mesaj;
        obiectTooltip.SetActive(true);

        // Mutam eticheta fix deasupra butonului (pe axa Y, in sus cu 60 de unitati)
        obiectTooltip.transform.position = pozitieButon + new Vector3(0f, 60f, 0f); 
    }

    public void AscundeTooltip()
    {
        obiectTooltip.SetActive(false);
    }
}