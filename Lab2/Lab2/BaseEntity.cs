using System;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}