using System;
using JetBrains.Annotations;

namespace _Game.Scripts.DI {
    [MeansImplicitUse(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Itself)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor)]
    public class InjectAttribute : Attribute { }
}