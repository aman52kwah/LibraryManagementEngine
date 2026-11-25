namespace LibraryManagementEngine;

/// Represents an ISBN. Record structs give us value equality for free.</summary>
public readonly record struct Isbn(string Value);
