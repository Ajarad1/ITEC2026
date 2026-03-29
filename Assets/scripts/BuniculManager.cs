using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DateRetetaBunic
{
    public string mesajPoveste;
    public string numeExactProdusCrafting;

    public DateRetetaBunic(string mesaj, string produs)
    {
        mesajPoveste = mesaj;
        numeExactProdusCrafting = produs;
    }
}

public class BuniculManager : MonoBehaviour
{
    [Header("Referinte UI - Bunic")]
    public GameObject ecranBunic; 
    public TextMeshProUGUI textTimer;
    public TypewriterEffect efectMasinaDeScris;

    [Header("Referinte Notificare")]
    public Image imagineButonBunic; 

    [Header("Setari")]
    public float timpAsteptare = 90f;
    
    private float timerCurent;
    private bool bunicPregatit = false;
    private bool ecranEraDeschis = false;

    private static List<DateRetetaBunic> reteteDisponibile = null;

    void Start()
    {
        if (reteteDisponibile == null) IncarcaRetetele();
        
        timerCurent = timpAsteptare;
        if (imagineButonBunic != null) imagineButonBunic.color = Color.white;
    }

    void Update()
    {
        if (!bunicPregatit)
        {
            timerCurent -= Time.deltaTime;
            if (timerCurent <= 0) bunicPregatit = true; 
        }

        if (bunicPregatit && imagineButonBunic != null)
        {
            float puls = Mathf.PingPong(Time.time * 3f, 1f);
            imagineButonBunic.color = Color.Lerp(Color.white, Color.green, puls); 
        }

        bool ecranDeschisAcum = ecranBunic != null && ecranBunic.activeInHierarchy;
        
        if (ecranDeschisAcum && !ecranEraDeschis)
        {
            CandSeDeschideEcranul();
        }

        if (ecranDeschisAcum && !bunicPregatit && textTimer != null)
        {
            textTimer.text = "Timp de gândire: " + Mathf.Ceil(timerCurent).ToString() + "s";
        }

        ecranEraDeschis = ecranDeschisAcum; 
    }

    void CandSeDeschideEcranul()
    {
        if (bunicPregatit)
        {
            if (imagineButonBunic != null) imagineButonBunic.color = Color.white;
            if (textTimer != null) textTimer.text = "Gata!";
            DaORețetă();
        }
        else
        {
            if (reteteDisponibile.Count == 0)
            {
                efectMasinaDeScris.StartDialogue("Gata, nepoate... te-am învățat tot ce știam. Du-te și fă bani!");
                if (textTimer != null) textTimer.text = "Fără idei!";
            }
            else
            {
                efectMasinaDeScris.StartDialogue("Mai lasă-mă să mă gândesc, nepoate... Mintea mea nu mai e ce a fost.");
            }
        }
    }

    void DaORețetă()
    {
        if (reteteDisponibile == null || reteteDisponibile.Count == 0)
        {
            efectMasinaDeScris.StartDialogue("Gata, nepoate... te-am învățat tot ce știam.");
            return;
        }

        int indexRandom = Random.Range(0, reteteDisponibile.Count);
        DateRetetaBunic retetaAleasa = reteteDisponibile[indexRandom];
        reteteDisponibile.RemoveAt(indexRandom);

        // AICI E MAGIA: Îi spunem Crafting-ului să deblocheze obiectul folosind numele EXACT!
        CraftingManager.InvataRetetaDeLaBunic(retetaAleasa.numeExactProdusCrafting);

        if (efectMasinaDeScris != null)
        {
            efectMasinaDeScris.StartDialogue("Aha! Mi-am amintit una bună:\n" + retetaAleasa.mesajPoveste);
        }

        bunicPregatit = false;
        timerCurent = timpAsteptare;
    }

    void IncarcaRetetele()
    {
        reteteDisponibile = new List<DateRetetaBunic>()
        {
            // TIER BRONZ
            new DateRetetaBunic("Ață + Staniol -> Mileu anti-radiații 5G.", "Mileu anti-radiații 5G"),
            new DateRetetaBunic("Magnet + Dop de plută -> Busola \"Păcii Interioare\".", "Busola \"Păcii Interioare\""),
            new DateRetetaBunic("Bec ars + Bandă adezivă -> Semnalizator pentru BMW.", "Semnalizator pentru BMW"),
            
            // TIER ARGINT
            new DateRetetaBunic("Ochelari 3D + Marker negru -> Ochelari de soare.", "Ochelari de soare"),
            new DateRetetaBunic("Hartie + Marker verde -> Bancnota \"de colecție\".", "Bancnota \"de colectie\""),
            new DateRetetaBunic("2x Hârtie -> Ciornă norocoasă.", "Ciornă norocoasă"),
            
            // TIER AUR
            new DateRetetaBunic("Dopuri de urechi + Sârmă -> Căști anti-manele.", "Căști anti-manele"),
            new DateRetetaBunic("Vată + Apă -> Norișor personal.", "Norișor personal"),
            new DateRetetaBunic("Măr + Balon cu heliu -> Măr anti-gravitațional.", "Măr anti-gravitațional")
        };
    }
}