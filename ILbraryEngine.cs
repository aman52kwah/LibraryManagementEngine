using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LibraryManagementEngine
{
    public interface ILbraryEngine
    {
        void RegisterBook (Book book);
        void RegisterMember(Member member);

        bool TryFindBook(Isbn isbn, [NotNullWhen(true)] out Book? book);
        bool TryFindMember(string memberId,[NotNullWhen(true)] out Member? member);

        BorrowRecord BorrowBook(string memberId, Isbn isbn);
        BorrowRecord ReturnBook(string memberId, Isbn isbn);
        decimal CalculateOverDueFine(BorrowRecord record);
    }

    public class LibraryEngine : ILbraryEngine
    {
        private readonly Dictionary<Isbn, Book> _books = [];
        private readonly Dictionary<string, Member> _members = [];
        private readonly List<BorrowRecord> _borrowRecords = [];

        public void RegisterBook(Book book) => _books[book.Isbn] = book;
        public void RegisterMember(Member member) => _members[member.MemberId] = member;

       // API Attribute informs compiler when 'book' is guaranteed non-null
       public bool TryFindBook(Isbn isbn,[NotNullWhen(true)] out Book? book)
        {
            return _books.TryGetValue(isbn, out book);
        }

        public bool TryFindMember(string memberId,[NotNullWhen(true)] out Member? member)
        {
            return _members.TryGetValue(memberId, out member);
        }

        public BorrowRecord BorrowBook(string memberId, Isbn isbn)
        {
            // validation: verify memeber existence safely
            if (!TryFindMember(memberId, out Member? member))
            throw new KeyNotFoundException($"Book with ISBN '{memberId}' was not found.");
        
      // Validation: Verify book existence safely
        if (!TryFindBook(isbn, out Book? book))
        throw new KeyNotFoundException($"Book with ISBN '{isbn.Value}' was not found.");

       // Validation: Polymorphic rule check
       if(!member.CanBorrow())
       throw new InvalidOperationException($"Borrowing rejected: '{member.Name}' has reached their limit of {member.MaxBooksAllowed} books.");

        if(!book.TryBorrowCopy())
        throw new InvalidOperationException($"Borrowing rejected: '{book.Title}'  has no available copies.");


        //Update member and record state
        member.AddBorrowedBook(isbn);

//borrowing duration
        DateTime now = DateTime.Now;
        DateTime dueDate = now.AddDays(member.BorrowingPeriodDays);

        BorrowRecord record = new (isbn, memberId, now, dueDate);
        _borrowRecords.Add(record);
        return record;

        }

        public BorrowRecord ReturnBook(string memberId, Isbn isbn)
        {
            if (!TryFindMember(memberId, out Member? member))
            throw new KeyNotFoundException($"Member with ID '{memberId}' was not found");

            if(!TryFindBook(isbn, out  Book? book))
            throw new KeyNotFoundException($"Book with ISBN'{isbn.Value}' was not found.");

            //find active borrowing record
            int recordIndex = _borrowRecords.FindIndex(r => r.MemberId == memberId && r.BookIsbn == isbn && r.ReturnedDate is null);
            if(recordIndex == -1)
            throw new InvalidOperationException($"No active borrow recod found for member '{memberId}' and ISBN '{isbn.Value}'.");

            BorrowRecord activeRecord = _borrowRecords[recordIndex];

            BorrowRecord updatedRecord = activeRecord with {ReturnedDate = DateTime.Now};
            _borrowRecords[recordIndex] = updatedRecord;


            member.RemoveBorrowedBook(isbn);
            book.ReturnCopy();

            return updatedRecord;
        }


        public decimal CalculateOverDUeFine(BorrowRecord record)
        {
            if (!TryFindMember(record.MemberId, out Member? member))
            return 0m;

             DateTime effectiveReturnDate = record.ReturnedDate ?? DateTime.Now;

             int overDueDays = (int)Math.Ceiling((effectiveReturnDate - record.DueDate).TotalDays);

             //polymorphic calculationg based on member type fine rate
             return overDueDays * member.OverdueFinePerDay;
        }

        public decimal CalculateOverDueFine(BorrowRecord record)
        {
            throw new NotImplementedException();
        }
    }
}

