using UnityEngine;
using UnityEngine.UI;

public class ClipireTelefon : MonoBehaviour
{
    public Image imagineButon; 
    public Color culoareNormala = Color.white;
    public Color culoareAlerta = Color.red; // Am pus roșu pur ca să se vadă CLAR
    public float vitezaClipire = 6f;

    public Telefon scriptTelefon; 
    public bool areNotificari = false;

    void Start()
    {
        // Își ia singur imaginea de pe buton
        if (imagineButon == null) 
            imagineButon = GetComponent<Image>();

        // Caută scriptul de Telefon PESTE TOT, chiar dacă obiectul e stins/ascuns!
        if (scriptTelefon == null)
            scriptTelefon = FindAnyObjectByType<Telefon>(FindObjectsInactive.Include);

        // --- SISTEM DE DETECTARE ERORI ---
        if (imagineButon == null) 
            Debug.LogError("🚨 EROARE: Nu găsesc componenta Image pe acest buton!");
        if (scriptTelefon == null) 
            Debug.LogError("🚨 EROARE: Nu găsesc scriptul Telefon absolut nicăieri în joc!");
    }

    void Update()
    {
        if (scriptTelefon != null)
        {
            areNotificari = scriptTelefon.AreCrizeActive();
        }

        if (areNotificari && imagineButon != null)
        {
            // Clipire matematică
            float puls = (Mathf.Sin(Time.time * vitezaClipire) + 1f) / 2f;
            imagineButon.color = Color.Lerp(culoareNormala, culoareAlerta, puls);
        }
        else if (imagineButon != null)
        {
            imagineButon.color = culoareNormala;
        }
    }
}