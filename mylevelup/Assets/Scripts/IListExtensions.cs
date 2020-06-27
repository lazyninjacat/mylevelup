using System.Collections.Generic;

public static class IListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++) {
            T temp = list[i];
            int randIdx = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randIdx];
            list[randIdx] = temp;
        }
    }
}