﻿namespace Nerosoft.Euonia.Business;

/// <summary>
/// Represent the marked method would create new domain object data.
/// And the method could be invoked by <see cref="IObjectFactory"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FactoryCreateAttribute : FactoryMethodAttribute
{
}