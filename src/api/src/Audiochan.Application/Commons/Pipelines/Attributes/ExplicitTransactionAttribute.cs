using System;

namespace Audiochan.Application.Commons.Pipelines.Attributes;

/// <summary>
/// Use this attribute when you want to explicitly handle database transaction on a command/query.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class ExplicitTransactionAttribute : Attribute
{
    
}