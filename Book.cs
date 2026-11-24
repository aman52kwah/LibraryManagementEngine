using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementEngine
{
    public class Book
    {
        public Isbn Isbn { get;}
        public string Title { get;}
        public string Author { get;}
        public BookGenre Genre {get;}

        //encapsulated state: private setter restricts copy adjustments
        public int TotalCopies { get; private set;}
        public int AvailableCopies {get; private set;} 

        public Book(Isbn isbn, string title, string author, BookGenre genre, int totalCopies)
        {
            if (totalCopies <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalCopies),"Total copies must be at least 1.");

            Isbn = isbn;
            Title = title;
            Author = author;
            Genre = genre;
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies;
        }

        public bool TryBorrowCopy()
        {
            if (AvailableCopies <= 0) return false;
            AvailableCopies--;
            return true;
        }

        public void ReturnCopy()
        {
            if(AvailableCopies >= TotalCopies)
            throw new InvalidOperationException("All copies of this book are already in the library. ");
            AvailableCopies++;
        }

           }
}