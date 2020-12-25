using System;

namespace JigsawPuzzle
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Delegate | AttributeTargets.Interface | AttributeTargets.Enum,
        AllowMultiple = false,
        Inherited = false)]
    public class ShareScriptsAttribute : Attribute
    {
    }
}