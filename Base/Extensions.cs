using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

public static class Extensions
{
    public static Color ReadRGB(this BinaryReader reader)
    {
        return Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
    }

    //public static T[] Append<T>(this T[] source, T item)
    //{
    //    var next = new T[source.Length + 1];
    //    Array.Copy(source, next, source.Length);
    //    next[next.Length - 1] = item;
    //    return next;
    //}

    public static T[] RemoveIndex<T>(this T[] source, int index)
    {
        return source.Where((_, i) => i != index).ToArray();
    }

    public static T[] RemoveLast<T>(this T[] items)
    {
        return items.Take(items.Length - 1).ToArray();
    }

    public static string ToStringOrNull(this object o)
    {
        return o?.ToString();
    }

    // we use + 1 such that FirstOrDefault gives 0, which still isn't valid
    // so we take that back out if it didn't find one
    // examples
    // case 1: item doesn't exist so first or default returns 0
    // case 2: item exists so the value must be > 0 because of + 1
    // -1 for not found
    public static int TryFindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        return items.Select((x, i) => predicate(x) ? i + 1 : -1).FirstOrDefault(i => i > 0) - 1;
    }

    public static IEnumerable<T> WhereI<T>(this IEnumerable<T> items, Func<T, int, bool> predicate)
    {
        return items.Select((x, i) => new {value = x, index = i})
            .Where(v => predicate(v.value, v.index))
            .Select(v => v.value);
    }

    public static IEnumerable<T> ExceptIndex<T>(this IEnumerable<T> items, int badIndex)
    {
        var i = 0;
        foreach (var item in items)
        {
            if (i != badIndex)
                yield return item;
            i++;
        }
    }

    public static IEnumerable<int> FindIndexes<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        return items
            // include index
            .Select((x, i) => new {value = x, index = i})
            // filter on predicate
            .Where(x => predicate(x.value))
            .Select(x => x.index);
    }

    // works just fine when x is null, extension methods aren't instance methods.
    public static bool IsValueString(this string x)
    {
        return !string.IsNullOrWhiteSpace(x);
    }

    public static string After(this string x, string delimiter)
    {
        if (x == null) throw new ArgumentNullException(nameof(x));
        if (string.IsNullOrEmpty(delimiter))
            throw new ArgumentException("must not be null or empty", nameof(delimiter));
        if (x.Length < delimiter.Length)
            throw new InvalidOperationException($"{nameof(delimiter)} was longer than input");
        var ind = x.IndexOf(delimiter, StringComparison.Ordinal);
        if (ind < 0)
            throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(delimiter)} was not found in string");

        return x.Substring(ind + delimiter.Length);
    }

    public static string Before(this string x, string delimiter)
    {
        if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));
        if (string.IsNullOrEmpty(delimiter))
            throw new InvalidOperationException(nameof(delimiter) + "must not be empty");
        if (x == null) throw new ArgumentNullException(nameof(x));
        var i = x.IndexOf(delimiter, StringComparison.Ordinal);
        if (i < 0) throw new InvalidOperationException($"{nameof(x)} did not contain '{delimiter}'");
        return x.Substring(0, i);
    }

    // model after F#, both indexes are inclusive, unlike substring which is (index, count), this is (index, index)
    // in F# this would be x.[start..stop]
    private static string GetRange(this string x, int start, int stop)
    {
        return x.Substring(start, stop - start + 1);
    }

    public static string GetLine(this string x, int lineIndex)
    {
        while (true)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (lineIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(lineIndex)} must be 0 or greater");
            var strIndex = x.IndexOfAny(new[]
            {
                '\r', '\n'
            });
            switch (lineIndex)
            {
                case 0 when strIndex < 0:
                    return x;
                case 0 when strIndex == 0:
                    return string.Empty;
                case 0 when 0 < strIndex:
                    return x.GetRange(0, strIndex - 1);
            }

            if (0 < lineIndex && strIndex < 0)
                throw new InvalidOperationException("Reached end of string before finding desired index");
            var rem = x.Substring(strIndex + 1);
            if (0 < rem.Length && x[strIndex] == '\r' && rem[0] == '\n')
            {
                x = rem.Substring(1);
                lineIndex = lineIndex - 1;
                continue;
            }

            x = rem;
            lineIndex = lineIndex - 1;
        }
    }
}