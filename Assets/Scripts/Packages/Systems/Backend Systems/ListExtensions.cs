using System.Collections.Generic;
using UnityEngine;

static public class ListExtensions
{
    // Extension method to shuffle any IList<T> in place
    public static void Shuffle<T>(this IList<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count - 1; i++)
        {
            // Pick a random index from the remaining elements
            int r = Random.Range(i, count);

            // Swap the elements
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }
}
