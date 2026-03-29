using UnityEngine;
using UnityEngine.UI; // NOU: Ne trebuie asta pentru a schimba pozele (Image)
using TMPro;

public class HubManager : MonoBehaviour
{
    [Header("Statistici Totale")]
    public int bani = 0;
    public int incredere = 50;
    public int ziuaCurenta = 1;
    public int datorieZilnica = 50;

    [Header("Sistem Timp")]
    public float durataZi = 60f; 
    private float timpRamas;
    public bool jocActiv = true; 

    [Header("UI Elemente")]
    public TextMeshProUGUI textBani;
    public TextMeshProUGUI textIncredere;
    public TextMeshProUGUI textTimp;
    public TextMeshProUGUI textDatorie;

    [Header("Ecranele Jocului")]
    public GameObject ecranAcasa;
    public GameObject ecranHarta;
    public GameObject ecranBunic;
    public GameObject ecranCrafting;
    public GameObject ecranIntalnire; 
    
    [Header("Sistem Întâlnire")]
    public NPC_Controller npcCurentLaUsa;
    public Image imagineFundalIntalnire; // NOU: Locul unde vom pune poza trimisă de NPC

    void Start()
    {
        Time.timeScale = 1f; 
        timpRamas = durataZi;
        ActualizeazaInterfata();
        DeschideAcasa();
    }

    void Update()
    {
        if (jocActiv)
        {
            timpRamas -= Time.deltaTime;
            int secunde = Mathf.CeilToInt(timpRamas);
            textTimp.text = "Ziua " + ziuaCurenta + " (" + secunde + "s)";

            if (timpRamas <= 0f) TerminaZiuaAutomat();
        }
    }

    public void AdaugaBani(int suma) { bani += suma; ActualizeazaInterfata(); }
    public void ScadeIncredere(int suma) { incredere -= suma; ActualizeazaInterfata(); }
    public void AdaugaIncredere(int suma) { incredere += suma; ActualizeazaInterfata(); }

    public void ActualizeazaInterfata()
    {
        textBani.text = "Bani: " + bani + "$";
        textIncredere.text = "Încredere: " + incredere + "/100";
        textDatorie.text = "Datorie azi: " + datorieZilnica + "$";
    }

    public void TerminaZiuaAutomat()
    {
        if (bani >= datorieZilnica) 
        {
            bani -= datorieZilnica; 
            ziuaCurenta++;
            datorieZilnica += 25; 
            timpRamas = durataZi; 
            
            incredere += 10; 
            
            ActualizeazaInterfata();
        } 
        else 
        {
            jocActiv = false; 
            textTimp.text = "GAME OVER!";
            textTimp.color = Color.red; 
            Time.timeScale = 0f; 
        }
    }

    // --- FUNCȚIA NOUĂ PENTRU FUNDAL ---
    public void PregatesteFundalIntalnire(Sprite fundal)
    {
        if (imagineFundalIntalnire != null)
        {
            imagineFundalIntalnire.sprite = fundal;
            imagineFundalIntalnire.color = Color.white; // Ne asigurăm că imaginea se vede perfect
        }
    }

    // --- NAVIGARE MENIURI ---
    public void DeschideAcasa() { AscundeToateEcranele(); ecranAcasa.SetActive(true); }
    public void DeschideHarta() { AscundeToateEcranele(); ecranHarta.SetActive(true); }
    public void DeschideBunic() { AscundeToateEcranele(); ecranBunic.SetActive(true); }
    public void DeschideCrafting() { AscundeToateEcranele(); ecranCrafting.SetActive(true); }
    
    public void DeschideIntalnire() 
    { 
        AscundeToateEcranele(); 
        ecranIntalnire.SetActive(true);
    }

    public void InchideMinigameSiReiaTimpul()
    {
        DeschideHarta();
    }

    private void AscundeToateEcranele() 
    { 
        ecranAcasa.SetActive(false); 
        ecranHarta.SetActive(false); 
        ecranBunic.SetActive(false); 
        ecranCrafting.SetActive(false); 
        if (ecranIntalnire != null) ecranIntalnire.SetActive(false); 
    }
}