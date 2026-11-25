namespace LibraryManagementEngine;


/// Immutable borrow record. Record classes provide value equality and
/// support with expressions for non-destructive updates.
 
public record class BorrowRecord(
    Isbn BookIsbn,
    string MemberId,
    DateTime BorrowedDate,
    DateTime DueDate,
    DateTime? ReturnedDate = null);
