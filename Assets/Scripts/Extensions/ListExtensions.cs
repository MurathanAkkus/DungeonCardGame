using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T Draw<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("Kart listesi null veya s�f�r. Kart �ekilemez.");
            return default;
        }
        int index = Random.Range(0, list.Count); // Rasgele bir indeks se� kart listesinden
        T item = list[index];                    // Se�ilen kart� al
        list.RemoveAt(index);                    // Se�ilen kart�, index de�eri ile listeden kald�r
        return item;                             // Se�ilen kart� geri d�nd�r
    }
}
