﻿using System;

namespace Tuna.Application.Persistence.Pipelines.Attributes;

/// <summary>
///     Use this attribute when you want to explicitly handle database transaction on a command/query.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ExplicitTransactionAttribute : Attribute
{
}