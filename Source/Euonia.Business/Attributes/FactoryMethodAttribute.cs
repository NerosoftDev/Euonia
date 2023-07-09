﻿namespace Nerosoft.Euonia.Business;

/// <summary>
/// Represent the marked method could be invoked by <see cref="IObjectFactory"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public abstract class FactoryMethodAttribute : Attribute
{
}