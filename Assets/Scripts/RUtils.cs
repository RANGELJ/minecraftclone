using System.Collections.Generic;

public class RUtils {
    public delegate bool ListFilterPredicate<T>(T item, int index);

    public static List<T> FilterList<T>(
        List<T> originalList,
        ListFilterPredicate<T> predicate
    ) {
        List<T> newList = new List<T>();

        for (int index = 0; index < originalList.Count; index += 1) {
            T item = originalList[index];
            if (predicate(item, index)) {
                newList.Add(item);
            }
        }

        return newList;
    }

    public delegate void ListIteratePredicate<T>(T item, int index);

    public static void ForEachInList<T>(
        List<T> list,
        ListIteratePredicate<T> predicate
    ) {
        for (int index = 0; index < list.Count; index += 1) {
            T item = list[index];
            predicate(item, index);
        }
    }
}
