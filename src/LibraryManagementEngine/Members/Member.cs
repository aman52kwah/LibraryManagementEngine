namespace LibraryManagementEngine;

public abstract class Member
{
    // Private field protecting the internal list.
    private readonly List<Isbn> _borrowedIsbns = [];

    public string MemberId { get; }
    public string Name { get; }

    // Read-only public view of borrowed books.
    public IReadOnlyList<Isbn> BorrowedIsbns => _borrowedIsbns.AsReadOnly();

    // Polymorphic rules supplied by each member type.
    public abstract int MaxBooksAllowed { get; }
    public abstract int BorrowingPeriodDays { get; }
    public abstract decimal OverdueFinePerDay { get; }

    protected Member(string memberId, string name)
    {
        if (string.IsNullOrWhiteSpace(memberId))
            throw new ArgumentException("Member ID cannot be empty.", nameof(memberId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        MemberId = memberId;
        Name = name;
    }

    public bool CanBorrow() => _borrowedIsbns.Count < MaxBooksAllowed;

    public void AddBorrowedBook(Isbn isbn)
    {
        if (!CanBorrow())
            throw new InvalidOperationException($"{Name} has reached the borrowing limit of {MaxBooksAllowed} books.");
        _borrowedIsbns.Add(isbn);
    }

    public bool RemoveBorrowedBook(Isbn isbn) => _borrowedIsbns.Remove(isbn);
}
