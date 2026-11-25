using System.Diagnostics.CodeAnalysis;

namespace LibraryManagementEngine;

public class LibraryEngine : ILibraryEngine
{
    private readonly Dictionary<Isbn, Book> _books = [];
    private readonly Dictionary<string, Member> _members = [];
    private readonly List<BorrowRecord> _borrowRecords = [];

    public void RegisterBook(Book book) => _books[book.Isbn] = book;
    public void RegisterMember(Member member) => _members[member.MemberId] = member;

    // [NotNullWhen(true)] tells the compiler 'book' is non-null when this returns true.
    public bool TryFindBook(Isbn isbn, [NotNullWhen(true)] out Book? book) =>
        _books.TryGetValue(isbn, out book);

    public bool TryFindMember(string memberId, [NotNullWhen(true)] out Member? member) =>
        _members.TryGetValue(memberId, out member);

    public BorrowRecord BorrowBook(string memberId, Isbn isbn)
    {
        if (!TryFindMember(memberId, out Member? member))
            throw new KeyNotFoundException($"Member with ID '{memberId}' was not found.");

        if (!TryFindBook(isbn, out Book? book))
            throw new KeyNotFoundException($"Book with ISBN '{isbn.Value}' was not found.");

        // Polymorphic rule check.
        if (!member.CanBorrow())
            throw new InvalidOperationException(
                $"Borrowing rejected: '{member.Name}' has reached their limit of {member.MaxBooksAllowed} books.");

        if (!book.TryBorrowCopy())
            throw new InvalidOperationException(
                $"Borrowing rejected: '{book.Title}' has no available copies.");

        member.AddBorrowedBook(isbn);

        DateTime now = DateTime.Now;
        DateTime dueDate = now.AddDays(member.BorrowingPeriodDays);

        BorrowRecord record = new(isbn, memberId, now, dueDate);
        _borrowRecords.Add(record);
        return record;
    }

    public BorrowRecord ReturnBook(string memberId, Isbn isbn)
    {
        if (!TryFindMember(memberId, out Member? member))
            throw new KeyNotFoundException($"Member with ID '{memberId}' was not found.");

        if (!TryFindBook(isbn, out Book? book))
            throw new KeyNotFoundException($"Book with ISBN '{isbn.Value}' was not found.");

        int recordIndex = _borrowRecords.FindIndex(
            r => r.MemberId == memberId && r.BookIsbn == isbn && r.ReturnedDate is null);
        if (recordIndex == -1)
            throw new InvalidOperationException(
                $"No active borrow record found for member '{memberId}' and ISBN '{isbn.Value}'.");

        BorrowRecord updatedRecord = _borrowRecords[recordIndex] with { ReturnedDate = DateTime.Now };
        _borrowRecords[recordIndex] = updatedRecord;

        member.RemoveBorrowedBook(isbn);
        book.ReturnCopy();

        return updatedRecord;
    }

    public decimal CalculateOverDueFine(BorrowRecord record)
    {
        if (!TryFindMember(record.MemberId, out Member? member))
            return 0m;

        DateTime effectiveReturnDate = record.ReturnedDate ?? DateTime.Now;
        int overDueDays = (int)Math.Ceiling((effectiveReturnDate - record.DueDate).TotalDays);

        // No fine if returned on or before the due date.
        if (overDueDays <= 0)
            return 0m;

        return overDueDays * member.OverdueFinePerDay;
    }

    public IReadOnlyList<Book> GetAllBooks() => _books.Values.ToList().AsReadOnly();
    public IReadOnlyList<Member> GetAllMembers() => _members.Values.ToList().AsReadOnly();
}
