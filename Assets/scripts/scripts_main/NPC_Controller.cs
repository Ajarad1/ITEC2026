using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; 

public class NPC_Controller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Date Personaj")]
    public string numeNPC = "Nume";
    public Image pozaPeHarta; 
    public Sprite fundalIntalnire;
    
    [Header("Cerințe și Bonusuri")]
    public int incredereNecesara = 0; 
    public int bonusIncredereLaRacolare = 5; 
    public float vitezaMinigame = 500f;
    
    [Header("Meniuri Pop-up")]
    public GameObject meniuRacolare;
    public GameObject meniuProductie;
    public TextMeshProUGUI textTrustNecesat;

    [Header("Setări Economie")]
    public int baniCasual = 5;
    public int baniPremium = 15;
    public int pierdereIncredere = 2;
    public float timpProductie = 3f;

    [Header("Stare Curentă")]
    public bool esteRacolat = false;
    public bool arePremium = false;
    public int modProductie = 0; 

    [Header("Sistem Cooldown")]
    public float cooldownDupaEsec = 30f;
    private float timpCooldownRamas = 0f;
    public bool esteInCooldown = false;

    private float timer = 0f;
    private HubManager banca; 

    [Header("Sistem Loialitate")]
    public int incredereIndividuala = 10; 
    public int incredereIndividualaMax = 10;

    [Header("Sistem Upgrade Manual")]
    public bool itemPremiumDeblocat = false; 
    public bool folosesteItemPremium = false;

    [Header("UI Tooltip")]
    public GameObject tooltipPanel; // Trage aici obiectul Tooltip_Detalii
    public TextMeshProUGUI textInfo; // Trage aici obiectul Text_Info

    void Start()
    {
        banca = FindFirstObjectByType<HubManager>();
        
        if (!esteRacolat && pozaPeHarta != null) 
        {
            pozaPeHarta.color = new Color(0.3f, 0.3f, 0.3f, 1f); 
        }
        
        if(meniuRacolare != null) meniuRacolare.SetActive(false);
        if(meniuProductie != null) meniuProductie.SetActive(false);

        ActualizeazaTextTrust();
    }

    void Update()
    {
        // Economia - generarea de bani
        if (esteRacolat && modProductie != 0)
        {
            timer += Time.deltaTime;
            if (timer >= timpProductie)
            {
                timer = 0f; 
                
                // Dacă e pe modul COMUN
                if (modProductie == 1) 
                { 
                    banca.AdaugaBani(baniCasual); 
                }
                // Dacă e pe modul PREMIUM
                else if (modProductie == 2) 
                { 
                    banca.AdaugaBani(baniPremium); 
                    banca.ScadeIncredere(pierdereIncredere); // Aici îți scade Trust-ul pe hartă!
                }
            }
        }

        // Sistemul de Cooldown
        if (esteInCooldown)
        {
            timpCooldownRamas -= Time.deltaTime;
            if (timpCooldownRamas <= 0)
            {
                esteInCooldown = false;
                Debug.Log(numeNPC + " este dispus să mai negocieze!");
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActualizeazaTextTrust(); 
        
        if (!esteRacolat) 
        { 
            if(meniuRacolare != null) meniuRacolare.SetActive(true); 
            if(meniuProductie != null) meniuProductie.SetActive(false); 
        }
        else 
        { 
            if(meniuProductie != null) meniuProductie.SetActive(true); 
            if(meniuRacolare != null) meniuRacolare.SetActive(false); 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(meniuRacolare != null) meniuRacolare.SetActive(false);
        if(meniuProductie != null) meniuProductie.SetActive(false);
    }

    private void ActualizeazaTextTrust()
    {
        if (textTrustNecesat != null && banca != null)
        {
            textTrustNecesat.text = "Necesită: " + incredereNecesara + " Încredere";
            Color maro;
            ColorUtility.TryParseHtmlString("#331704", out maro); // Maro în hex

            if (banca.incredere < incredereNecesara)
                textTrustNecesat.color = Color.red; 
            else
                textTrustNecesat.color = maro; 
                
        }
    }

    public void IncearcaRacolare() 
    { 
        if (esteInCooldown)
        {
            Debug.Log(numeNPC + " refuză să te vadă! Mai ai de așteptat " + Mathf.Ceil(timpCooldownRamas) + " secunde.");
            return; 
        }

        if (banca.incredere >= incredereNecesara)
        {
            if(meniuRacolare != null) meniuRacolare.SetActive(false);
            
            banca.npcCurentLaUsa = this; 
            
            if (fundalIntalnire != null)
            {
                banca.PregatesteFundalIntalnire(fundalIntalnire);
            }
            else
            {
                Debug.LogWarning("NPC_Controller: " + numeNPC + " nu are un fundal de întâlnire setat!");
            }
            
            banca.DeschideIntalnire(); 
        }
        else
        {
            Debug.Log("NU AI DESTULĂ ÎNCREDERE! Ai nevoie de: " + incredereNecesara);
        }
    }
    
    public void SuccesLaContract()
    {
        esteRacolat = true; 
        modProductie = 1; // Începe automat cu producția comună
        if (pozaPeHarta != null) pozaPeHarta.color = Color.white; 
        banca.AdaugaIncredere(bonusIncredereLaRacolare);
        Debug.Log("Ai racolat-o pe " + numeNPC + "!");
    }

    public void EsecLaContract()
    {
        esteInCooldown = true;
        timpCooldownRamas = cooldownDupaEsec;
        Debug.Log("Ai eșuat! " + numeNPC + " intră în cooldown " + cooldownDupaEsec + " sec.");
    }

    public void ModificaIncredereIndividuala(int valoare)
    {
        incredereIndividuala += valoare;
        
        if (incredereIndividuala > incredereIndividualaMax) 
            incredereIndividuala = incredereIndividualaMax;

        Debug.Log(numeNPC + " are acum " + incredereIndividuala + " încredere în tine.");

        if (incredereIndividuala <= 0)
        {
            incredereIndividuala = 0;
            PierdeAngajatul();
        }
    }

    private void PierdeAngajatul()
    {
        Debug.Log("DEZASTRU! " + numeNPC + " te-a părăsit!");
        
        esteRacolat = false;
        modProductie = 0;
        
        if (pozaPeHarta != null) pozaPeHarta.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        if (banca != null)
        {
            banca.ScadeIncredere(bonusIncredereLaRacolare); 
        }

        incredereIndividuala = incredereIndividualaMax; 
        itemPremiumDeblocat = false; // Îi luăm și itemul dacă pleacă
    }

    // --- FUNCȚIILE DE BUTON (legate de Crafting) ---

    public void DeblocheazaItemPremium()
    {
        itemPremiumDeblocat = true;
        Debug.Log("🔒 AI DEBLOCAT itemul premium pentru " + numeNPC + "!");
    }

    public void EchipeazaItemComun()
    {
        folosesteItemPremium = false;
        modProductie = 1; // 1 = Dă bani puțini, dar e sigur
        
        if(meniuProductie != null) meniuProductie.SetActive(false); // Ascundem meniul ca să arate bine
        
        Debug.Log(numeNPC + " s-a întors la itemul COMUN. Va produce " + baniCasual + "$");
    }

    public void EchipeazaItemPremium()
    {
        if (itemPremiumDeblocat == true)
        {
            folosesteItemPremium = true;
            modProductie = 2; // 2 = Dă bani mulți, dar SCADE încrederea
            
            if(meniuProductie != null) meniuProductie.SetActive(false); // Ascundem meniul

            Debug.Log("✨ " + numeNPC + " folosește acum itemul PREMIUM! Va produce " + baniPremium + "$, dar pierzi " + pierdereIncredere + " Încredere!");
        }
        else
        {
            Debug.Log("Nu poți echipa! Itemul nu este craftat încă!");
        }
    }

    // --- FUNCȚII PENTRU HOVER BUTOANE ---

    public void AfiseazaDetaliiCasual()
    {
        if (tooltipPanel == null || textInfo == null) return;

        tooltipPanel.SetActive(true);
        textInfo.text = $"<b>PRODUS CASUAL</b>\n" +
                        $"<color=yellow>{baniCasual}$ / {timpProductie}s</color>\n" +
                        $"Pierdere: 0 Încredere";
    }

    public void AfiseazaDetaliiPremium()
    {
        if (tooltipPanel == null || textInfo == null) return;

        tooltipPanel.SetActive(true);
        textInfo.text = $"<b>PRODUS PREMIUM</b>\n" +
                        $"<color=green>{baniPremium}$ / {timpProductie}s</color>\n" +
                        $"<color=red>Pierdere: -{pierdereIncredere} Încredere</color>";
    }

    public void AscundeDetalii()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}