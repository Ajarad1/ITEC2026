using UnityEngine;

[CreateAssetMenu(fileName = "NouItem", menuName = "SchemaPiramidala/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identitate")]
    public string numeItem;
    public Sprite iconita;

    // [Header("Economie")]
    // public int pretVanzare;      // Cati bani aduce la buget
    // public int impactIncredere;  // + sau - la bara de incredere a comunitatii

    // [Header("Timp")]
    // [Tooltip("La cate secunde se vinde o bucata de catre NPC")]
    // public float timpVanzare;   
}