using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T Draw<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("Kart listesi null veya sýfýr. Kart çekilemez.");
            return default;
        }
        int index = Random.Range(0, list.Count); // Rasgele bir indeks seç kart listesinden
        T item = list[index];                    // Seçilen kartý al
        list.RemoveAt(index);                    // Seçilen kartý, index deðeri ile listeden kaldýr
        return item;                             // Seçilen kartý geri döndür
    }
}
