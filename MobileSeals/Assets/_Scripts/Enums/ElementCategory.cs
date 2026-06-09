using System;

[Flags]
public enum ElementCategory
{
    Water = 1 << 0,
    Earth = 1 << 1,
    Wind = 1 << 2,
}
