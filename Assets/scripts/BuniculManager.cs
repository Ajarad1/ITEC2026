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
    
    // VARIABILE STATICE - Acestea NU se resetează la schimbarea scenei
    private static float momentFinalizareTimer = -1f; 
    private static List<DateRetetaBunic> reteteDisponibile = null;
    private static bool bunicPregatitStatic = false;

    private bool ecranEraDeschis = false;

    void Start()
    {
        // 1. Inițializăm lista de rețete dacă e prima dată când rulăm jocul
        if (reteteDisponibile == null) IncarcaRetetele();
        
        // 2. Dacă e prima pornire a jocului, setăm primul timer
        if (momentFinalizareTimer == -1f)
        {
            SeteazaTimerNou();
        }

        if (imagineButonBunic != null) imagineButonBunic.color = Color.white;
    }

    void Update()
    {
        // Calculăm timpul rămas față de ceasul global al Unity (Time.time)
        float timpRamas = momentFinalizareTimer - Time.time;

        // Verificăm dacă bunicul a terminat de gândit
        if (!bunicPregatitStatic && timpRamas <= 0)
        {
            bunicPregatitStatic = true;
        }

        // Vizual: Pulsare buton când e gata
        if (bunicPregatitStatic && imagineButonBunic != null)
        {
            float puls = Mathf.PingPong(Time.time * 3f, 1f);
            imagineButonBunic.color = Color.Lerp(Color.white, Color.green, puls); 
        }

        // Logică deschidere ecran
        bool ecranDeschisAcum = ecranBunic != null && ecranBunic.activeInHierarchy;
        
        if (ecranDeschisAcum)
        {
            if (!ecranEraDeschis) CandSeDeschideEcranul();

            // Gestionare afișare TEXT TIMER
            if (textTimer != null)
            {
                if (bunicPregatitStatic)
                {
                    textTimer.text = ""; // Nu afișăm nimic dacă e gata (sau "Gata!")
                }
                else
                {
                    textTimer.text = "Timp de gândire: " + Mathf.CeilToInt(timpRamas).ToString() + "s";
                }
            }
        }

        ecranEraDeschis = ecranDeschisAcum; 
    }

    void CandSeDeschideEcranul()
    {
        if (bunicPregatitStatic)
        {
            if (imagineButonBunic != null) imagineButonBunic.color = Color.white;
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

        CraftingManager.InvataRetetaDeLaBunic(retetaAleasa.numeExactProdusCrafting);

        if (efectMasinaDeScris != null)
        {
            efectMasinaDeScris.StartDialogue("Aha! Mi-am amintit una bună:\n" + retetaAleasa.mesajPoveste);
        }

        // După ce dă rețeta, resetăm bunicul și pornim un timer nou
        bunicPregatitStatic = false;
        SeteazaTimerNou();
    }

    void SeteazaTimerNou()
    {
        momentFinalizareTimer = Time.time + timpAsteptare;
    }

    void IncarcaRetetele()
    {
        reteteDisponibile = new List<DateRetetaBunic>()
        {
            new DateRetetaBunic("Ață + Staniol -> Mileu anti-radiații 5G.", "Mileu anti-radiații 5G"),
            new DateRetetaBunic("Magnet + Dop de plută -> Busola \"Păcii Interioare\".", "Busola \"Păcii Interioare\""),
            new DateRetetaBunic("Bec ars + Bandă adezivă -> Semnalizator pentru BMW.", "Semnalizator pentru BMW"),
            new DateRetetaBunic("Ochelari 3D + Marker negru -> Ochelari de soare.", "Ochelari de soare"),
            new DateRetetaBunic("Hartie + Marker verde -> Bancnota \"de colecție\".", "Bancnota \"de colectie\""),
            new DateRetetaBunic("2x Hârtie -> Ciornă norocoasă.", "Ciornă norocoasă"),
            new DateRetetaBunic("Dopuri de urechi + Sârmă -> Căști anti-manele.", "Căști anti-manele"),
            new DateRetetaBunic("Vată + Apă -> Norișor personal.", "Norișor personal"),
            new DateRetetaBunic("Măr + Balon cu heliu -> Măr anti-gravitațional.", "Măr anti-gravitațional")
        };
    }
}