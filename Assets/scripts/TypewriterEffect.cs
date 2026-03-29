using System.Collections;
using UnityEngine;
using TMPro; // Obligatoriu pentru TextMeshPro

public class TypewriterEffect : MonoBehaviour
{
    [Header("Setari Efect")]
    [Tooltip("Timpul în secunde între fiecare literă (mai mic = mai rapid)")]
    public float vitezaScriere = 0.05f;

    [Header("Referinte UI")]
    public TextMeshProUGUI textUI; // Caseta text unde scriem

    // Variabile interne
    private string textComplet;
    private Coroutine coroutineCurenta;
    private bool seScrieAcum = false;

    void Awake()
    {
        // Daca nu am tras manual caseta text, o cautam pe acelasi obiect
        if (textUI == null) textUI = GetComponent<TextMeshProUGUI>();
        
        // Initial, golim textul
        if (textUI != null) textUI.text = "";
    }

    // --- FUNCTIA PRINCIPALA (O chemi din alte scripturi) ---
    // Exemplu apel: managerDialog.StartDialogue("Salut nepoate!");
    public void StartDialogue(string textDeAfisat)
    {
        textComplet = textDeAfisat;

        // Daca scria deja ceva, oprim scrierea veche
        if (coroutineCurenta != null)
        {
            StopCoroutine(coroutineCurenta);
        }

        // Pornim timer-ul de scriere litera cu litera
        coroutineCurenta = StartCoroutine(ScrieTextLiteraCuLitera());
    }

    // Coroutine (Timer-ul propriu-zis)
    IEnumerator ScrieTextLiteraCuLitera()
    {
        seScrieAcum = true;
        textUI.text = ""; // Golim caseta la inceput

        // Trecem prin fiecare litera din textul complet
        foreach (char litera in textComplet.ToCharArray())
        {
            textUI.text += litera; // Adaugam o litera

            // Aici poti adauga un sunet scurt de "click" daca ai

            // Asteptam timpul setat (ex: 0.05 secunde)
            yield return new WaitForSeconds(vitezaScriere);
        }

        seScrieAcum = false;
        coroutineCurenta = null;
    }

    // (Optional) Functie ca sa sari peste efect si sa arati tot textul instant
    public void AfiseazaTotTextulInstant()
    {
        if (seScrieAcum && coroutineCurenta != null)
        {
            StopCoroutine(coroutineCurenta);
            textUI.text = textComplet;
            seScrieAcum = false;
            coroutineCurenta = null;
        }
    }
}