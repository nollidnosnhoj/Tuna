using System;

namespace Audiochan.API.Middlewares.Pipelines.Attributes;

/// <summary>
/// Use this attribute when you want to explicitly handle database transaction on a command/query.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class ExplicitTransactionAttribute : Attribute
{
    
}