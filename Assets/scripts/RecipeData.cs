using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NouaReteta", menuName = "SchemaPiramidala/Reteta")]
public class RecipeData : ScriptableObject
{
    [Header("Ingrediente")]
    public List<ItemData> ingredienteNecesare;

    [Header("Rezultat")]
    public ItemData produsRezultat;
}