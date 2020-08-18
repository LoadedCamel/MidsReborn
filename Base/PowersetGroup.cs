using System;
using System.Collections.Generic;

public class PowersetGroup : IComparable
{
    public PowersetGroup(string name)
    {
        Name = name;
        Powersets = new Dictionary<string, IPowerset>();
    }

    public string Name { get; }

    public IDictionary<string, IPowerset> Powersets { get; }

    public int CompareTo(object obj)
    {
        if (obj is PowersetGroup powersetGroup)
            return string.Compare(Name, powersetGroup.Name, StringComparison.OrdinalIgnoreCase);
        throw new ArgumentException("Comparison failed - Passed object was not an Archetype Class!");
    }
}