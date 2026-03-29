using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance; 

    public List<RecipeData> listaRetete;
    public List<ItemData> ingredientePeMasa = new List<ItemData>();

    [Header("Imaginile de pe ecran")]
    public Image slotStanga;
    public Image slotDreapta;
    public Image slotRezultat;

    [Header("Banner Feedback")]
    public GameObject bannerFeedback; 
    public TextMeshProUGUI textBanner; 

    // Aici stocăm ce ne-a învățat Bunicul
    public static List<string> reteteDeblocate = new List<string>(); 

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        GolesteMasa();
    }

    // --- MAGIA DE CURĂȚENIE PENTRU ECRANE BLOCATE ---
    void OnEnable()
    {
        // Când se deschide ecranul, ne asigurăm că banner-ul e ascuns
        if (bannerFeedback != null)
        {
            bannerFeedback.SetActive(false);
        }
    }

    void OnDisable()
    {
        // Când ieși de pe ecran, oprim forțat orice timer (corutină) ca să nu se încurce în fundal
        StopAllCoroutines();
        
        if (bannerFeedback != null)
        {
            bannerFeedback.SetActive(false);
        }
    }
    // ------------------------------------------------

    public void AdaugaIngredient(ItemData itemNou)
    {
        if (bannerFeedback.activeSelf) return;

        if (ingredientePeMasa.Count >= 2 || slotRezultat.sprite != null)
        {
            GolesteMasa();
        }

        ingredientePeMasa.Add(itemNou);
        ActualizeazaVizualMasa();
    }

    void ActualizeazaVizualMasa()
    {
        if (ingredientePeMasa.Count > 0)
        {
            slotStanga.sprite = ingredientePeMasa[0].iconita;
            slotStanga.color = Color.white;
        }
        if (ingredientePeMasa.Count > 1)
        {
            slotDreapta.sprite = ingredientePeMasa[1].iconita;
            slotDreapta.color = Color.white;
        }
    }

    public void IncearcaAsamblarea()
    {
        if (bannerFeedback.activeSelf) return;

        if (ingredientePeMasa.Count != 2)
        {
            StartCoroutine(AfiseazaBanner("Ai nevoie de 2 ingrediente!"));
            return;
        }

        bool combinatieGasita = false;

        foreach (RecipeData reteta in listaRetete)
        {
            if (VerificaReteta(reteta))
            {
                combinatieGasita = true;
                Debug.Log("Masa caută: [" + reteta.produsRezultat.numeItem + "]. Bunicul te-a învățat: [" + string.Join(", ", reteteDeblocate) + "]");
                
                // VERIFICAREA FINALĂ: BUNICUL A ZIS SAU NU?
                if (!reteteDeblocate.Contains(reteta.produsRezultat.numeItem))
                {
                    StartCoroutine(AfiseazaBanner("Încă nu știi cum să faci asta! Vorbește cu Bunicul."));
                    return; 
                }

                // Dacă ai rețeta de la bunic:
                slotRezultat.sprite = reteta.produsRezultat.iconita;
                slotRezultat.color = Color.white;
                
                StartCoroutine(AfiseazaBanner("SUCCES! Ai creat: " + reteta.produsRezultat.numeItem));
                
                // Trimite semnalul de deblocare către NPC
                DeblocheazaLaNPC(reteta.produsRezultat.numeItem);
                break;
            }
        }

        if (!combinatieGasita)
        {
            StartCoroutine(AfiseazaBanner("Eșec! Ingredientele nu se potrivesc."));
        }
    }

    IEnumerator AfiseazaBanner(string mesaj)
    {
        textBanner.text = mesaj;
        
        Color culoareMaro;
        if(ColorUtility.TryParseHtmlString("#331604", out culoareMaro))
        {
             textBanner.color = culoareMaro;
        }

        bannerFeedback.SetActive(true);
        yield return new WaitForSeconds(2f);
        bannerFeedback.SetActive(false);

        GolesteMasa();
    }

    bool VerificaReteta(RecipeData reteta)
    {
        if (reteta.ingredienteNecesare.Count != 2) return false;
        List<ItemData> copieMasa = new List<ItemData>(ingredientePeMasa);
        foreach (ItemData ingr in reteta.ingredienteNecesare)
        {
            if (copieMasa.Contains(ingr)) copieMasa.Remove(ingr);
            else return false;
        }
        return true;
    }

    public void DeblocheazaLaNPC(string numeItemCraftat)
    {
        NPC_Controller[] totiNPC = FindObjectsByType<NPC_Controller>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (NPC_Controller npc in totiNPC)
        {
            if (npc.numeNPC == "Bunica" && numeItemCraftat == "Mileu anti-radiații 5G") npc.DeblocheazaItemPremium();
            else if (npc.numeNPC == "Cercetasul" && numeItemCraftat == "Busola \"Păcii Interioare\"") npc.DeblocheazaItemPremium();
            else if (npc.numeNPC == "Mecanicul" && numeItemCraftat == "Semnalizator pentru BMW") npc.DeblocheazaItemPremium();
            else if (npc.numeNPC == "FashionModel" && numeItemCraftat == "Ochelari de soare") npc.DeblocheazaItemPremium();
            else if (npc.numeNPC == "Primarul" && numeItemCraftat == "Bancnota \"de colectie\"") npc.DeblocheazaItemPremium();
            else if (npc.numeNPC == "Profesorul" && numeItemCraftat == "Ciornă norocoasă") npc.DeblocheazaItemPremium();
            else if (npc.numeNPC == "Mozart" && numeItemCraftat == "Căști anti-manele") npc.DeblocheazaItemPremium();
            else if (npc.numeNPC == "Zeus" && numeItemCraftat == "Norișor personal") npc.DeblocheazaItemPremium();
            else if (npc.numeNPC == "Newton" && numeItemCraftat == "Măr anti-gravitațional") npc.DeblocheazaItemPremium();
        }
    }

    public void GolesteMasa()
    {
        ingredientePeMasa.Clear();
        if(slotStanga != null) { slotStanga.sprite = null; slotStanga.color = new Color(1, 1, 1, 0); }
        if(slotDreapta != null) { slotDreapta.sprite = null; slotDreapta.color = new Color(1, 1, 1, 0); }
        if(slotRezultat != null) { slotRezultat.sprite = null; slotRezultat.color = new Color(1, 1, 1, 0); }
    }

    // Funcția apelată de BuniculManager
    public static void InvataRetetaDeLaBunic(string numeProdus)
    {
        if (!reteteDeblocate.Contains(numeProdus))
        {
            reteteDeblocate.Add(numeProdus);
            Debug.Log("Sistem Crafting: Am deblocat rețeta pentru " + numeProdus);
        }
    }
}