using System;

namespace WonderLab.Classes.Attributes;

public class BindToConfigAttribute(string configName) : Attribute
{
    public string ConfigName => configName;
}
