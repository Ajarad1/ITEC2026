using UnityEngine;
using UnityEngine.UI;
using System.Collections; // NOU: Ne trebuie asta pentru cronometre (Coroutines)

public class ContractMinigame : MonoBehaviour
{
    [Header("Ecrane Final")]
    public GameObject winScreen;  
    public GameObject loseScreen; 

    [Header("UI Elements")]
    public RectTransform cursor;       
    public RectTransform targetZone;   
    public RectTransform backgroundBar;

    [Header("Settings")]
    public float speed = 500f;         
    public int requiredHits = 3;       
    
    private bool isPlaying = true;
    private float backgroundWidth;
    private int currentHits = 0;       

    private HubManager banca;

void OnEnable()
    {
        banca = FindFirstObjectByType<HubManager>();
        
        // NOU: Preluăm viteza direct de la NPC-ul curent!
        if (banca != null && banca.npcCurentLaUsa != null)
        {
            speed = banca.npcCurentLaUsa.vitezaMinigame;
        }
        
        isPlaying = true;
        currentHits = 0;
        backgroundWidth = backgroundBar.rect.width;

        cursor.gameObject.SetActive(true);
        targetZone.gameObject.SetActive(true);
        backgroundBar.gameObject.SetActive(true);

        if(winScreen != null) winScreen.SetActive(false);
        if(loseScreen != null) loseScreen.SetActive(false);
    }

    void Update()
    {
        if (!isPlaying) return;

        float limit = backgroundWidth / 2f;
        
        float xPos = Mathf.PingPong(Time.time * speed, backgroundWidth) - limit;
        cursor.anchoredPosition = new Vector2(xPos, cursor.anchoredPosition.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckHit();
        }
    }

    void CheckHit()
    {
        float dist = Mathf.Abs(cursor.anchoredPosition.x - targetZone.anchoredPosition.x);
        float allowedDistance = (targetZone.rect.width / 2f);

        if (dist <= allowedDistance)
        {
            currentHits++;
            Debug.Log("Lovitură corectă! " + currentHits + "/" + requiredHits);

            if (currentHits >= requiredHits)
            {
                isPlaying = false;
                ShowResult(true); 
            }
        }
        else
        {
            isPlaying = false;
            ShowResult(false); 
        }
    }

    void ShowResult(bool isWin)
    {
        cursor.gameObject.SetActive(false);
        targetZone.gameObject.SetActive(false);
        backgroundBar.gameObject.SetActive(false);

        if (isWin)
        {
            winScreen.SetActive(true);
            if (banca != null && banca.npcCurentLaUsa != null) banca.npcCurentLaUsa.SuccesLaContract();
        }
        else
        {
            loseScreen.SetActive(true);
            if (banca != null && banca.npcCurentLaUsa != null) banca.npcCurentLaUsa.EsecLaContract();
        }

        // NOU: Pornim cronometrul de 2 secunde pentru ieșire automată!
        StartCoroutine(IesireAutomata());
    }

    // NOU: Funcția care așteaptă 2 secunde reale și apoi închide
private IEnumerator IesireAutomata()
    {
        // Acum numără 2 secunde din timpul jocului, nu din viața reală
        yield return new WaitForSeconds(2f); 
        Buton_InchideMinigame();
    }

    public void Buton_InchideMinigame()
    {
        gameObject.SetActive(false); 
        if (banca != null) banca.InchideMinigameSiReiaTimpul(); 
    }
}