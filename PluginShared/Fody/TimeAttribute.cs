using System;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module |
                AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
internal class TimeAttribute : Attribute
{
}