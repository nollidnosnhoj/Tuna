namespace Tuna.Shared.Models;

public abstract record OffsetPagedQuery(int? Skip, int? Take);