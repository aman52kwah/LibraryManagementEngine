using System;

namespace LibraryManagementEngine
{
    //represent discrete book genres
        public enum BookGenre
    {
        Fiction,
        NonFiction,
        Science,
        History,
        Technology
    }

    // Represent an ISBN
    public readonly record struct Isbn(string Value);

    //Represents an immutable borrow record
    // Record class generates property-by-property value equality and and supports 'with' expressions

    public record class BorrowRecord
    (
        Isbn BookIsbn,
        string MemberId,
        DateTime BorrowedDate,
        DateTime DueDate,
        DateTime? ReturnedDate = null
    
);



}

        