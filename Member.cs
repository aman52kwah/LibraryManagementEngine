using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementEngine
{
    public abstract class Member
    {

        //Private field protecting internal list
        private readonly List<Isbn> _borrowedIsbns = [];
        public string MemberId {get;}
        public string Name {get; }

        // Read-only public view of borrowed books
        public IReadOnlyList<Isbn>BorrowedIsbns => _borrowedIsbns.AsReadOnly();


        // polymorphic abstract and virtual members
        public abstract int MaxBooksAllowed {get;}
        public abstract int BorrowingPeriodDays { get ;}
        public abstract decimal OverdueFinePerDay {get; }

        protected Member(string memberId, string name)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
               if (string.IsNullOrWhiteSpace(name))
               throw new ArgumentException("Name cannot be empty.", nameof(name));

               MemberId = memberId;
               Name = name;
        }

        public bool CanBorrow()=> _borrowedIsbns.Count < MaxBooksAllowed;

        public void AddBorrowedBook(Isbn isbn)
        {
            if (!CanBorrow())
            throw new InvalidOperationException($"{Name} has reached the borrowing limit of {MaxBooksAllowed} books.");
            _borrowedIsbns.Add(isbn);
        }

        public bool RemoveBorrowedBook(Isbn isbn) => _borrowedIsbns.Remove(isbn);
    }

    //Derived Type 1: Student Member
    public sealed class StudentMember : Member
    {
        public override int MaxBooksAllowed => 5;
        public override int BorrowingPeriodDays => 15;
        public override decimal OverdueFinePerDay => 0.50m;

        public StudentMember(string memberId, string name) : base(memberId, name){}
    }

    //Faculty Member
    public sealed class FacultyMember : Member
    {
        public override int MaxBooksAllowed => 10;
        public override int BorrowingPeriodDays => 30;
        public override decimal OverdueFinePerDay => 0.10m;

        public FacultyMember(string memberId, string name): base(memberId, name){}

    }
}