using System.Diagnostics.CodeAnalysis;

namespace LibraryManagementEngine;

public interface ILibraryEngine
{
    void RegisterBook(Book book);
    void RegisterMember(Member member);

    bool TryFindBook(Isbn isbn, [NotNullWhen(true)] out Book? book);
    bool TryFindMember(string memberId, [NotNullWhen(true)] out Member? member);

    BorrowRecord BorrowBook(string memberId, Isbn isbn);
    BorrowRecord ReturnBook(string memberId, Isbn isbn);
    decimal CalculateOverDueFine(BorrowRecord record);

    IReadOnlyList<Book> GetAllBooks();
    IReadOnlyList<Member> GetAllMembers();
}
