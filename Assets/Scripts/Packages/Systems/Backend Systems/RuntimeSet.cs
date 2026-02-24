using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class RuntimeSet<T> : ScriptableObject
{//Looking to create a list to hold research, usually of a specific type, for reference for each weapon
    [Expandable] public List<T> items = new List<T>();
    public void Add(T t)
    {
        if (!items.Contains(t))
            items.Add(t);
    }
    public void Remove(T t)
    {
        if (items.Contains(t))
            items.Remove(t);
    }

    /// <summary>
    ///   <para>Checks if this RuntimeSet is null or empty, or if the first item is null.</para>
    /// </summary>
    /// <returns>
    /// <para>True if null or empty, otherwise false.</para>
    /// </returns>
    public bool GetNullOrEmpty()
    {
        return items == null || items.Count <= 0 || (items.Count == 1 && items[0] == null);
    }

    /// <summary>
    ///   <para>Checks if this RuntimeSet is null or empty, or if ALL items are null.</para>
    /// </summary>
    /// <returns>
    /// <para>True if null, empty, or all entries are null, otherwise false.</para>
    /// </returns>
    public bool AllNullOrEmpty()
    {
        foreach (var item in items) if (item != null) return false;
        return true;
    }
}