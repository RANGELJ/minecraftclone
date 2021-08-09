using System.Collections.Generic;

public class RUtils {
    public delegate bool ListPredicate<T>(T item, int index);

    public static List<T> FilterList<T>(
        List<T> originalList,
        ListPredicate<T> predicate
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
}
