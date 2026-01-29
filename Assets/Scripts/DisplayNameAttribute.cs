using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DisplayNameAttribute : Attribute
{
    public string Name { get; }

    public DisplayNameAttribute(string name)
    {
        Name = name;
    }
}
