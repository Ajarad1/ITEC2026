using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificareCriza
{
    public NPC_Controller npcImplicat;
    public string mesajCriza;
    public float timpRamas;
    public GameObject butonUI; 
    public TextMeshProUGUI textTimerUI; 
    
    // NOU: Acum fiecare notificare își ține minte propriile răspunsuri
    public string[] texteRaspuns = new string[4];
    public int[] scoruriRaspuns = new int[4];
}

public class Telefon : MonoBehaviour
{
    [Header("Ecrane Telefon")]
    public GameObject ecranInbox;
    public GameObject ecranMesaj; 
    public TextMeshProUGUI textNotificariBula; 

    [Header("Matrița și Lista")]
    public GameObject prefabButonNotificare; 
    public Transform containerListaInbox; 

    [Header("UI Ecran Mesaj (Conversația)")]
    public TextMeshProUGUI textNumeNPC;
    public TextMeshProUGUI textContinutMesaj;
    public Button[] butoaneRaspuns = new Button[4]; 
    public TextMeshProUGUI[] texteButoaneRaspuns = new TextMeshProUGUI[4];

    [Header("Setări Sistem")]
    public float timpPanaLaUrmatorulMesaj = 20f; // L-am pus la 20 ca să vină mai des!
    public float timpPentruRaspuns = 30f; 

    private float timerTrimitereMesaj;
    private List<NotificareCriza> crizeActive = new List<NotificareCriza>();
    private NotificareCriza crizaCurentaDeschisa; 

    void Start()
    {
        timerTrimitereMesaj = timpPanaLaUrmatorulMesaj;
        ActualizeazaBulaNotificari();
    }

    void Update()
    {
        timerTrimitereMesaj -= Time.deltaTime;
        if (timerTrimitereMesaj <= 0)
        {
            GenereazaOCrizaNoua();
            timerTrimitereMesaj = timpPanaLaUrmatorulMesaj;
        }

        for (int i = crizeActive.Count - 1; i >= 0; i--)
        {
            NotificareCriza criza = crizeActive[i];
            criza.timpRamas -= Time.deltaTime;

            if (criza.textTimerUI != null)
                criza.textTimerUI.text = Mathf.Ceil(criza.timpRamas).ToString() + "s";

            if (criza.timpRamas <= 0)
            {
                criza.npcImplicat.ModificaIncredereIndividuala(-2); 
                StergeCriza(criza);
            }
        }
    }

    void GenereazaOCrizaNoua()
    {
        NPC_Controller[] totiNPC = FindObjectsByType<NPC_Controller>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        List<NPC_Controller> npcRacolati = new List<NPC_Controller>();

        foreach (NPC_Controller npc in totiNPC)
        {
            if (npc.esteRacolat) npcRacolati.Add(npc);
        }

        if (npcRacolati.Count == 0) return;

        NPC_Controller npcAles = npcRacolati[Random.Range(0, npcRacolati.Count)];
        
        CreazaNotificareUI(npcAles);
    }

    void CreazaNotificareUI(NPC_Controller npc)
    {
        NotificareCriza crizaNoua = new NotificareCriza();
        crizaNoua.npcImplicat = npc;
        crizaNoua.timpRamas = timpPentruRaspuns;

        // BAZA DE DATE CU SCENARII CUSTOM (5 pentru fiecare)
        IncarcaScenariuSpecific(crizaNoua, npc.numeNPC);

        GameObject butonClonat = Instantiate(prefabButonNotificare, containerListaInbox);
        crizaNoua.butonUI = butonClonat;

        TextMeshProUGUI[] texteDePeButon = butonClonat.GetComponentsInChildren<TextMeshProUGUI>();
        if (texteDePeButon.Length >= 2)
        {
            texteDePeButon[0].text = npc.numeNPC; 
            crizaNoua.textTimerUI = texteDePeButon[1]; 
        }

        Button btn = butonClonat.GetComponent<Button>();
        btn.onClick.AddListener(() => DeschideConversatia(crizaNoua));

        crizeActive.Add(crizaNoua);
        ActualizeazaBulaNotificari();
    }

    // --- BAZA DE DATE CU ÎNTREBĂRI ---
    void IncarcaScenariuSpecific(NotificareCriza c, string nume)
    {
        int rand = Random.Range(1, 6); // Alege un număr de la 1 la 5

        if (nume == "Bunica")
        {
            if (rand==1) Seteaza(c, "Nepoate, m-a prins poliția cu plăcintele 5G! Ce fac?", "Descurcă-te singură!", -2, "Prefă-te senilă.", -1, "Ascunde-le repede!", 1, "Trimit avocat, stai calmă.", 2);
            else if (rand==2) Seteaza(c, "Mi-au furat mileul de pe televizor! Nu mai lucrez azi.", "Treci la muncă!", -2, "Cumpără altul.", -1, "Te ajut să-l cauți diseară.", 1, "Îți dau bani de unul din mătase.", 2);
            else if (rand==3) Seteaza(c, "Pensia e prea mică, șefule. Mai punem ceva la contract?", "Nu e problema mea.", -2, "Vedem la anu'.", -1, "Îți dau un mic spor azi.", 1, "Măresc procentul, meriți!", 2);
            else if (rand==4) Seteaza(c, "Am uitat rețeta secretă de cozonac! E un dezastru!", "Cum să uiți?!", -2, "Caută pe Google.", -1, "Respiră, o să-ți amintești.", 1, "Găsim caietul vechi, te ajut.", 2);
            else Seteaza(c, "Pisica a vărsat aluatul pe jos...", "Fă altul imediat!", -2, "Dă-o afară.", -1, "Curăță și ia-o de la capăt.", 1, "Lasă, ia o pauză de o oră.", 2);
        }
        else if (nume == "Cercetasul")
        {
            if (rand==1) Seteaza(c, "M-am pierdut în pădure și n-am semnal la busolă!", "Ești cercetaș, descurcă-te!", -2, "Uită-te după mușchi pe copaci.", -1, "Stai pe loc, te găsim noi.", 1, "Trimit un elicopter acum!", 2);
            else if (rand==2) Seteaza(c, "Șefu, mi-a fost foame și am mâncat toți biscuiții...", "Ești concediat!", -2, "Îți opresc din bani.", -1, "Data viitoare fii atent.", 1, "Să-ți fie de bine, aduc alții.", 2);
            else if (rand==3) Seteaza(c, "Un urs îmi dă târcoale la stand. Ce fac?", "Vinde-i ceva!", -2, "Prefă-te mort.", -1, "Fă gălăgie și fugi.", 1, "Trimitem paza imediat!", 2);
            else if (rand==4) Seteaza(c, "Ceilalți copii râd de uniforma mea...", "Nu mă interesează.", -2, "Ignoră-i.", -1, "E o uniformă de elită!", 1, "Îți iau una nouă, de lux.", 2);
            else Seteaza(c, "Vreau să plec acasă, mă dor picioarele.", "Nici să nu te gândești!", -2, "Mai stai o oră.", -1, "Ia un scaun.", 1, "Du-te acasă, ai muncit destul.", 2);
        }
        else if (nume == "Mecanicul")
        {
            if (rand==1) Seteaza(c, "Am zgâriat mașina primarului din greșeală...", "O plătești din buzunar!", -2, "Ascunde-o.", -1, "Dă cu niște ojă pe ea.", 1, "Rezolv eu cu primarul, repar-o.", 2);
            else if (rand==2) Seteaza(c, "Am rămas fără ulei de motor și atelierul e plin.", "Spală-te pe mâini.", -2, "Cere de la vecini.", -1, "Comand acum un butoi.", 1, "Închide azi, facem cinste.", 2);
            else if (rand==3) Seteaza(c, "Clientul urlă la mine că i-am pus piese ieftine.", "Dă vina pe el.", -2, "Asta i-am dat, asta are.", -1, "Calmează-l frumos.", 1, "Dă-i banii înapoi, suport eu.", 2);
            else if (rand==4) Seteaza(c, "Mi-am rupt cheia franceză norocoasă!", "Ia alta din cutie.", -2, "Ghinion.", -1, "Cumpărăm alta la fel.", 1, "Îți comand o trusă premium.", 2);
            else Seteaza(c, "Am băut antigel din greșeală, credeam că e suc albastru.", "Treci la muncă, nu mori.", -2, "Bea apă.", -1, "Sună la ambulanță!", 1, "Te duc eu la spital acum!", 2);
        }
        else if (nume == "Profesorul")
        {
            if (rand==1) Seteaza(c, "Elevii au adormit toți la ora mea de fizică cuantică.", "Slab profesor ești.", -2, "Țipă la ei.", -1, "Fă o pauză.", 1, "Fă un experiment exploziv!", 2);
            else if (rand==2) Seteaza(c, "A explodat laboratorul. Din nou.", "Plătești daunele!", -2, "Curăță rapid.", -1, "Ești bine? Asta contează.", 1, "Perfect pentru cercetare! Finanțez.", 2);
            else if (rand==3) Seteaza(c, "Am pierdut catalogul cu notele de producție.", "Ești iresponsabil.", -2, "Caută-l mai bine.", -1, "Pune tuturor 10 și gata.", 1, "Trecem la sistem digital, te ajut.", 2);
            else if (rand==4) Seteaza(c, "Colegii zic că teoria mea e o nebunie.", "Probabil e.", -2, "Nu-i băga în seamă.", -1, "Vom dovedi contrariul.", 1, "Sunt niște incompetenți, tu ești geniu!", 2);
            else Seteaza(c, "Am rămas fără cretă și nu pot scrie formulele.", "Folosește un marker.", -2, "Scrie cu degetul.", -1, "Cumpărăm azi.", 1, "Îți aduc un smartboard ultimul răcnet.", 2);
        }
        else if (nume == "Primarul")
        {
            if (rand==1) Seteaza(c, "E DNA-ul la ușa primăriei! Ce le zic?!", "Ești pe cont propriu.", -2, "Nu deschide ușa.", -1, "Zâmbește și cheamă avocatul.", 1, "Trimit elicopterul, plecăm în Cuba!", 2);
            else if (rand==2) Seteaza(c, "Avem cetățeni furioși cu furci în fața instituției.", "Du-te și confruntă-i.", -2, "Cheamă jandarmii.", -1, "Dă-le niște mici și bere.", 1, "Ies eu și vorbesc cu ei.", 2);
            else if (rand==3) Seteaza(c, "Am rămas fără buget de panseluțe...", "Asta e ultima problemă.", -2, "Plantează buruieni.", -1, "Vorbim la rectificare.", 1, "Donez eu pentru floricele.", 2);
            else if (rand==4) Seteaza(c, "Vine campania și sondajele arată rău.", "Ești istorie.", -2, "Fă niște promisiuni.", -1, "Pornim mașinăria de PR.", 1, "Infuzez capital în campanie, stai calm.", 2);
            else Seteaza(c, "Presa a aflat de afacerile noastre...", "Dă vina pe consilieri.", -2, "Dă o dezmințire.", -1, "Spune că e fake news.", 1, "Cumpărăm ziarul și oprim știrea.", 2);
        }
        else if (nume == "FashionModel")
        {
            if (rand==1) Seteaza(c, "Mi s-a rupt tocul chiar înainte de podium!", "Mergi desculță.", -2, "Lipește-l cu scoci.", -1, "Ia altă pereche.", 1, "Oprește show-ul până primim pantofi noi!", 2);
            else if (rand==2) Seteaza(c, "Rivala mea poartă EXACT aceeași rochie!", "Asta e.", -2, "Du-te acasă.", -1, "Tu o porți mai bine.", 1, "Îi tăiem rochia ei, te rezolv.", 2);
            else if (rand==3) Seteaza(c, "Mi-a ieșit un coș enorm pe nas!", "Ești concediată.", -2, "Dă cu fond de ten.", -1, "Lumina te va avantaja.", 1, "Edităm pozele, ești superbă oricum.", 2);
            else if (rand==4) Seteaza(c, "Paparazzi sunt la ușa mea, nu pot ieși.", "Bate-te cu ei.", -2, "Ignoră-i.", -1, "Ieși pe ușa din spate.", 1, "Trimit bodyguarzii să-i alunge.", 2);
            else Seteaza(c, "Vreau o ședință foto la Paris, m-am săturat de studioul ăsta.", "N-avem bani, taci.", -2, "Poate la anul.", -1, "E o idee bună pe viitor.", 1, "Fă bagajele, am luat biletele!", 2);
        }
        else if (nume == "Newton")
        {
            if (rand==1) Seteaza(c, "Mărul ăla mi-a spart capul. Nu mai gândesc clar.", "Ia un paracetamol.", -2, "Pune gheață.", -1, "Concentrează-te la știință.", 1, "Îți aduc un medic celebru.", 2);
            else if (rand==2) Seteaza(c, "Gravitația pare să fluctueze azi în laborator.", "Ești nebun.", -2, "Verifică iar calculele.", -1, "E o descoperire uriașă!", 1, "Finanțez studiul anomaliei imediat.", 2);
            else if (rand==3) Seteaza(c, "Leibniz susține că a inventat el calculul infinitezimal!", "Cine e Leibniz?", -2, "Dă-l în judecată.", -1, "Toți știu că tu ai fost primul.", 1, "Îi distrugem reputația, stai liniștit.", 2);
            else if (rand==4) Seteaza(c, "Mi-am pierdut peruca norocoasă.", "Arăți bine și chel.", -2, "Caut-o.", -1, "Îți aduc eu alta.", 1, "Îți comand o perucă din aur pur.", 2);
            else Seteaza(c, "Oamenii zic că alchimia mea e vrăjitorie.", "Chiar este.", -2, "Ignoră plebea.", -1, "E doar știință neînțeleasă.", 1, "Construim un laborator secret doar pentru tine.", 2);
        }
        else if (nume == "Mozart")
        {
            if (rand==1) Seteaza(c, "Pianul e complet dezacordat. O insultă la adresa artei mele!", "Cântă așa.", -2, "Acordează-l singur.", -1, "Chem pe cineva să-l repare.", 1, "Îți cumpăr un pian nou din Viena.", 2);
            else if (rand==2) Seteaza(c, "Salieri mă sabotează din nou...", "Te plângi prea mult.", -2, "Ignoră-l, e invidios.", -1, "Muzica ta e superioară.", 1, "Mă ocup eu ca Salieri să dispară.", 2);
            else if (rand==3) Seteaza(c, "Am uitat partitura pentru simfonia de diseară!", "Improvizează.", -2, "Caut-o repede.", -1, "O scrii din nou, ești un geniu.", 1, "Amânăm concertul, geniul nu se grăbește.", 2);
            else if (rand==4) Seteaza(c, "Am cheltuit toți banii din avans la cârciumă...", "Ești un dezastru.", -2, "Problema ta.", -1, "Mai atenție data viitoare.", 1, "Îți dublez avansul, arta cere sacrificii.", 2);
            else Seteaza(c, "Regele a adormit la opereta mea.", "Era plictisitoare.", -2, "E doar obosit.", -1, "Scrie ceva mai alert.", 1, "Regele nu are gusturi, muzica e perfectă.", 2);
        }
        else if (nume == "Zeus")
        {
            if (rand==1) Seteaza(c, "Am rămas fără fulgere și ciclopii sunt în grevă.", "Folosește o lanternă.", -2, "Așteaptă să-și revină.", -1, "Negociază cu ei.", 1, "Le dau salarii duble, vrem fulgere!", 2);
            else if (rand==2) Seteaza(c, "Hera mi-a citit mesajele cu nimfele. E scandal în Olimp!", "Meriți ce primești.", -2, "Ascunde-te.", -1, "Cere-ți scuze frumos.", 1, "Te primesc eu la mine acasă până se calmează.", 2);
            else if (rand==3) Seteaza(c, "Muritorii au început să se roage la tehnologie.", "Au evoluat.", -2, "Nu mai contează.", -1, "Dă-le un semn ceresc.", 1, "Le tăiem curentul și internetul!", 2);
            else if (rand==4) Seteaza(c, "Hades nu vrea să-mi mai împrumute câinele ăla cu 3 capete.", "Ia-ți un pechinez.", -2, "Vorbim cu el.", -1, "E câinele lui până la urmă.", 1, "Îi declarăm război lumii de dincolo!", 2);
            else Seteaza(c, "M-am plictisit să fiu zeu suprem. Vreau o pauză.", "Treci la treabă!", -2, "Mai rezistă un secol.", -1, "Ia un weekend liber.", 1, "Du-te în vacanță, preiau eu frâiele!", 2);
        }
        else
        {
            // Siguranță: Dacă scrii numele greșit, apare asta.
            Seteaza(c, "Avem o problemă generală la producție...", "Muncește mai tare!", -2, "Mai vedem.", -1, "Rezolvăm.", 1, "Îți dau un bonus.", 2);
        }
    }

    // Funcție ajutătoare ca să scriem întrebările frumos și scurt
    void Seteaza(NotificareCriza c, string msg, string r1, int i1, string r2, int i2, string r3, int i3, string r4, int i4)
    {
        c.mesajCriza = msg;
        c.texteRaspuns[0] = r1; c.scoruriRaspuns[0] = i1;
        c.texteRaspuns[1] = r2; c.scoruriRaspuns[1] = i2;
        c.texteRaspuns[2] = r3; c.scoruriRaspuns[2] = i3;
        c.texteRaspuns[3] = r4; c.scoruriRaspuns[3] = i4;
    }

    public void DeschideConversatia(NotificareCriza criza)
    {
        crizaCurentaDeschisa = criza;

        textNumeNPC.text = "Discuție cu: " + criza.npcImplicat.numeNPC;
        textContinutMesaj.text = criza.mesajCriza;

        // Folosim răspunsurile specifice din baza de date
        for (int i = 0; i < 4; i++)
        {
            SeteazaButonRaspuns(i, criza.texteRaspuns[i], criza.scoruriRaspuns[i]);
        }

        if (ecranInbox != null) ecranInbox.SetActive(false);
        if (ecranMesaj != null) ecranMesaj.SetActive(true);
    }

    void SeteazaButonRaspuns(int index, string textRaspuns, int impactIncredere)
    {
        if (texteButoaneRaspuns[index] != null) texteButoaneRaspuns[index].text = textRaspuns;
        
        if (butoaneRaspuns[index] != null)
        {
            butoaneRaspuns[index].onClick.RemoveAllListeners(); 
            butoaneRaspuns[index].onClick.AddListener(() => TrimiteRaspuns(impactIncredere));
        }
    }

    public void TrimiteRaspuns(int impact)
    {
        if (crizaCurentaDeschisa != null && crizaCurentaDeschisa.npcImplicat != null)
        {
            crizaCurentaDeschisa.npcImplicat.ModificaIncredereIndividuala(impact);
            StergeCriza(crizaCurentaDeschisa);
        }

        if (ecranMesaj != null) ecranMesaj.SetActive(false);
        if (ecranInbox != null) ecranInbox.SetActive(true);
    }

    void StergeCriza(NotificareCriza criza)
    {
        if (criza.butonUI != null) Destroy(criza.butonUI); 
        crizeActive.Remove(criza); 
        crizaCurentaDeschisa = null;
        ActualizeazaBulaNotificari();
    }

    void ActualizeazaBulaNotificari()
    {
        if (textNotificariBula != null)
        {
            textNotificariBula.text = "Mesaje (" + crizeActive.Count + ")";
        }
    }

    public void DeschideInbox() { if (ecranInbox != null) ecranInbox.SetActive(true); if (ecranMesaj != null) ecranMesaj.SetActive(false); }
    public void InchideTotTelefonul() { if (ecranInbox != null) ecranInbox.SetActive(false); if (ecranMesaj != null) ecranMesaj.SetActive(false); }
}