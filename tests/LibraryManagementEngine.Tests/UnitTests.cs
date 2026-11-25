using System;
using Xunit;
using LibraryManagementEngine;

namespace LibraryManagementEngine.Tests
{
    public class BookTests
    {
        [Fact]
        public void Book_Constructor_WithValidData_SetsProperties()
        {
            // Arrange & Act
            var book = new Book(new Isbn("978-0123456789"), "Clean Code", "Robert C. Martin", BookGenre.Technology, 3);

            // Assert
            Assert.Equal("978-0123456789", book.Isbn.Value);
            Assert.Equal("Clean Code", book.Title);
            Assert.Equal("Robert C. Martin", book.Author);
            Assert.Equal(BookGenre.Technology, book.Genre);
            Assert.Equal(3, book.TotalCopies);
            Assert.Equal(3, book.AvailableCopies);
        }

        [Fact]
        public void Book_Constructor_WithZeroCopies_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Book(new Isbn("978-0123456789"), "Clean Code", "Robert C. Martin", BookGenre.Technology, 0));
        }

        [Fact]
        public void Book_Constructor_WithNegativeCopies_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Book(new Isbn("978-0123456789"), "Clean Code", "Robert C. Martin", BookGenre.Technology, -1));
        }

        [Fact]
        public void TryBorrowCopy_WhenCopiesAvailable_DecrementsAndReturnsTrue()
        {
            // Arrange
            var book = new Book(new Isbn("978-0123456789"), "Clean Code", "Robert C. Martin", BookGenre.Technology, 2);

            // Act
            bool result1 = book.TryBorrowCopy();
            bool result2 = book.TryBorrowCopy();
            bool result3 = book.TryBorrowCopy();

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.False(result3);
            Assert.Equal(0, book.AvailableCopies);
        }

        [Fact]
        public void ReturnCopy_WhenNotAllCopiesInLibrary_Increments()
        {
            // Arrange
            var book = new Book(new Isbn("978-0123456789"), "Clean Code", "Robert C. Martin", BookGenre.Technology, 2);
            book.TryBorrowCopy(); // AvailableCopies: 2 -> 1

            // Act
            book.ReturnCopy();    // AvailableCopies: 1 -> 2

            // Assert
            Assert.Equal(2, book.AvailableCopies);
        }

        [Fact]
        public void ReturnCopy_WhenAllCopiesAlreadyInLibrary_ThrowsInvalidOperationException()
        {
            // Arrange
            var book = new Book(new Isbn("978-0123456789"), "Clean Code", "Robert C. Martin", BookGenre.Technology, 1);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => book.ReturnCopy());
        }
    }

    public class IsbnTests
    {
        [Fact]
        public void Isbn_WithSameValue_AreEqual()
        {
            // Arrange
            var isbn1 = new Isbn("978-0123456789");
            var isbn2 = new Isbn("978-0123456789");

            // Act & Assert
            Assert.Equal(isbn1, isbn2);
            Assert.True(isbn1 == isbn2);
            Assert.False(isbn1 != isbn2);
        }

        [Fact]
        public void Isbn_WithDifferentValue_AreNotEqual()
        {
            // Arrange
            var isbn1 = new Isbn("978-0123456789");
            var isbn2 = new Isbn("978-0987654321");

            // Act & Assert
            Assert.NotEqual(isbn1, isbn2);
            Assert.False(isbn1 == isbn2);
            Assert.True(isbn1 != isbn2);
        }
    }

    public class MemberTests
    {
        [Fact]
        public void StudentMember_Constructor_SetsCorrectRules()
        {
            // Arrange & Act
            var student = new StudentMember("STU001", "Alice");

            // Assert
            Assert.Equal("STU001", student.MemberId);
            Assert.Equal("Alice", student.Name);
            Assert.Equal(5, student.MaxBooksAllowed);
            Assert.Equal(15, student.BorrowingPeriodDays);
            Assert.Equal(0.50m, student.OverdueFinePerDay);
        }

        [Fact]
        public void FacultyMember_Constructor_SetsCorrectRules()
        {
            // Arrange & Act
            var faculty = new FacultyMember("FAC001", "Bob");

            // Assert
            Assert.Equal("FAC001", faculty.MemberId);
            Assert.Equal("Bob", faculty.Name);
            Assert.Equal(10, faculty.MaxBooksAllowed);
            Assert.Equal(30, faculty.BorrowingPeriodDays);
            Assert.Equal(0.10m, faculty.OverdueFinePerDay);
        }

        [Fact]
        public void Member_Constructor_WithNullMemberId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new StudentMember(null!, "Alice"));
        }

        [Fact]
        public void Member_Constructor_WithEmptyMemberId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new StudentMember("", "Alice"));
        }

        [Fact]
        public void Member_Constructor_WithWhiteSpaceMemberId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new StudentMember("   ", "Alice"));
        }

        [Fact]
        public void Member_Constructor_WithNullName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new StudentMember("STU001", null!));
        }

        [Fact]
        public void CanBorrow_WhenUnderLimit_ReturnsTrue()
        {
            // Arrange
            var student = new StudentMember("STU001", "Alice");

            // Act & Assert
            Assert.True(student.CanBorrow());
        }

        [Fact]
        public void CanBorrow_WhenAtLimit_ReturnsFalse()
        {
            // Arrange
            var student = new StudentMember("STU001", "Alice");
            for (int i = 0; i < 5; i++)
            {
                student.AddBorrowedBook(new Isbn($"978-00000000{i}"));
            }

            // Act & Assert
            Assert.False(student.CanBorrow());
        }

        [Fact]
        public void AddBorrowedBook_WhenOverLimit_ThrowsInvalidOperationException()
        {
            // Arrange
            var student = new StudentMember("STU001", "Alice");
            for (int i = 0; i < 5; i++)
            {
                student.AddBorrowedBook(new Isbn($"978-00000000{i}"));
            }

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                student.AddBorrowedBook(new Isbn("978-9999999999")));
        }

        [Fact]
        public void RemoveBorrowedBook_WhenBookBorrowed_RemovesAndReturnsTrue()
        {
            // Arrange
            var student = new StudentMember("STU001", "Alice");
            var isbn = new Isbn("978-0123456789");
            student.AddBorrowedBook(isbn);

            // Act
            bool result = student.RemoveBorrowedBook(isbn);

            // Assert
            Assert.True(result);
            Assert.Empty(student.BorrowedIsbns);
        }

        [Fact]
        public void RemoveBorrowedBook_WhenBookNotBorrowed_ReturnsFalse()
        {
            // Arrange
            var student = new StudentMember("STU001", "Alice");

            // Act
            bool result = student.RemoveBorrowedBook(new Isbn("978-0123456789"));

            // Assert
            Assert.False(result);
        }
    }

    public class LibraryEngineTests
    {
        private LibraryEngine CreateEngineWithData()
        {
            var engine = new LibraryEngine();

            var book1 = new Book(new Isbn("978-0123456789"), "Clean Code", "Robert C. Martin", BookGenre.Technology, 2);
            var book2 = new Book(new Isbn("978-0987654321"), "Design Patterns", "Erich Gamma", BookGenre.Technology, 1);
            engine.RegisterBook(book1);
            engine.RegisterBook(book2);

            var student = new StudentMember("STU001", "Alice");
            var faculty = new FacultyMember("FAC001", "Bob");
            engine.RegisterMember(student);
            engine.RegisterMember(faculty);

            return engine;
        }

        [Fact]
        public void TryFindBook_WithExistingIsbn_ReturnsTrueAndBook()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act
            bool found = engine.TryFindBook(new Isbn("978-0123456789"), out Book? book);

            // Assert
            Assert.True(found);
            Assert.NotNull(book);
            Assert.Equal("Clean Code", book!.Title);
        }

        [Fact]
        public void TryFindBook_WithNonExistingIsbn_ReturnsFalseAndNull()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act
            bool found = engine.TryFindBook(new Isbn("000-000000000"), out Book? book);

            // Assert
            Assert.False(found);
            Assert.Null(book);
        }

        [Fact]
        public void TryFindMember_WithExistingId_ReturnsTrueAndMember()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act
            bool found = engine.TryFindMember("STU001", out Member? member);

            // Assert
            Assert.True(found);
            Assert.NotNull(member);
            Assert.Equal("Alice", member!.Name);
        }

        [Fact]
        public void TryFindMember_WithNonExistingId_ReturnsFalseAndNull()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act
            bool found = engine.TryFindMember("UNKNOWN", out Member? member);

            // Assert
            Assert.False(found);
            Assert.Null(member);
        }

        [Fact]
        public void BorrowBook_WhenValid_ReturnsBorrowRecordWithDueDate()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act
            var record = engine.BorrowBook("STU001", new Isbn("978-0123456789"));

            // Assert
            Assert.Equal("STU001", record.MemberId);
            Assert.Equal(new Isbn("978-0123456789"), record.BookIsbn);
            Assert.Null(record.ReturnedDate);
            // Due date should be approximately 15 days from now for a student
            var expectedDueDate = DateTime.Today.AddDays(15);
            Assert.Equal(expectedDueDate.Date, record.DueDate.Date);
        }

        [Fact]
        public void BorrowBook_WhenUnknownMember_ThrowsKeyNotFoundException()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                engine.BorrowBook("UNKNOWN", new Isbn("978-0123456789")));
        }

        [Fact]
        public void BorrowBook_WhenUnknownBook_ThrowsKeyNotFoundException()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                engine.BorrowBook("STU001", new Isbn("000-000000000")));
        }

        [Fact]
        public void BorrowBook_WhenValid_UpdatesMemberAndBookState()
        {
            // Arrange
            var engine = CreateEngineWithData();
            var isbn = new Isbn("978-0123456789");

            // Act
            engine.BorrowBook("STU001", isbn);

            // Assert
            engine.TryFindBook(isbn, out Book? book);
            engine.TryFindMember("STU001", out Member? member);
            Assert.Equal(1, book!.AvailableCopies);
            Assert.Contains(isbn, member!.BorrowedIsbns);
        }

        [Fact]
        public void BorrowBook_WhenNoCopiesAvailable_ThrowsInvalidOperationException()
        {
            // Arrange
            var engine = new LibraryEngine();
            var book = new Book(new Isbn("978-0123456789"), "Clean Code", "Robert C. Martin", BookGenre.Technology, 1);
            engine.RegisterBook(book);
            engine.RegisterMember(new StudentMember("STU001", "Alice"));

            // Borrow the only copy
            engine.BorrowBook("STU001", new Isbn("978-0123456789"));

            // Act & Assert - no copies left
            Assert.Throws<InvalidOperationException>(() =>
                engine.BorrowBook("STU001", new Isbn("978-0123456789")));
        }

        [Fact]
        public void BorrowBook_WhenMemberReachedMaxBooks_ThrowsInvalidOperationException()
        {
            // Arrange
            var engine = new LibraryEngine();
            // Register 5 books with 1 copy each for a student (max 5)
            for (int i = 0; i < 5; i++)
            {
                engine.RegisterBook(new Book(new Isbn($"978-00000000{i}"), $"Book {i}", "Author", BookGenre.Fiction, 1));
            }
            engine.RegisterMember(new StudentMember("STU001", "Alice"));

            // Borrow all 5
            for (int i = 0; i < 5; i++)
            {
                engine.BorrowBook("STU001", new Isbn($"978-00000000{i}"));
            }

            // Register a 6th book and try to borrow
            engine.RegisterBook(new Book(new Isbn("978-9999999999"), "Extra Book", "Author", BookGenre.Fiction, 1));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                engine.BorrowBook("STU001", new Isbn("978-9999999999")));
        }

        [Fact]
        public void ReturnBook_WhenValid_UpdatesRecordAndReturnsBook()
        {
            // Arrange
            var engine = CreateEngineWithData();
            engine.BorrowBook("STU001", new Isbn("978-0123456789"));

            // Act
            var record = engine.ReturnBook("STU001", new Isbn("978-0123456789"));

            // Assert
            Assert.NotNull(record.ReturnedDate);
            Assert.True(record.ReturnedDate <= DateTime.Now);
        }

        [Fact]
        public void ReturnBook_WhenNoActiveRecord_ThrowsInvalidOperationException()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                engine.ReturnBook("STU001", new Isbn("978-0123456789")));
        }

        [Fact]
        public void ReturnBook_WhenUnknownMember_ThrowsKeyNotFoundException()
        {
            // Arrange
            var engine = CreateEngineWithData();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                engine.ReturnBook("UNKNOWN", new Isbn("978-0123456789")));
        }

        [Fact]
        public void CalculateOverDueFine_ForOverdueStudent_CalculatesCorrectly()
        {
            // Arrange
            var engine = CreateEngineWithData();
            var record = engine.BorrowBook("STU001", new Isbn("978-0123456789"));
            // Simulate overdue: returned 5 days late
            var overdueRecord = record with { ReturnedDate = record.DueDate.AddDays(5) };

            // Act
            decimal fine = engine.CalculateOverDueFine(overdueRecord);

            // Assert: 5 days * $0.50/day = $2.50
            Assert.Equal(2.50m, fine);
        }

        [Fact]
        public void CalculateOverDueFine_ForOverdueFaculty_CalculatesCorrectly()
        {
            // Arrange
            var engine = CreateEngineWithData();
            var record = engine.BorrowBook("FAC001", new Isbn("978-0987654321"));
            // Simulate overdue: returned 5 days late
            var overdueRecord = record with { ReturnedDate = record.DueDate.AddDays(5) };

            // Act
            decimal fine = engine.CalculateOverDueFine(overdueRecord);

            // Assert: 5 days * $0.10/day = $0.50
            Assert.Equal(0.50m, fine);
        }

        [Fact]
        public void CalculateOverDueFine_ForNotOverdue_ReturnsZero()
        {
            // Arrange
            var engine = CreateEngineWithData();
            var record = engine.BorrowBook("STU001", new Isbn("978-0123456789"));
            // Return before due date
            var earlyRecord = record with { ReturnedDate = record.DueDate.AddDays(-2) };

            // Act
            decimal fine = engine.CalculateOverDueFine(earlyRecord);

            // Assert
            Assert.Equal(0m, fine);
        }

        [Fact]
        public void GetAllBooks_AfterRegistration_ReturnsAllBooks()
        {
            // Arrange
            var engine = new LibraryEngine();
            engine.RegisterBook(new Book(new Isbn("978-000000001"), "Book 1", "Author 1", BookGenre.Fiction, 1));
            engine.RegisterBook(new Book(new Isbn("978-000000002"), "Book 2", "Author 2", BookGenre.NonFiction, 2));

            // Act
            var books = engine.GetAllBooks();

            // Assert
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public void GetAllMembers_AfterRegistration_ReturnsAllMembers()
        {
            // Arrange
            var engine = new LibraryEngine();
            engine.RegisterMember(new StudentMember("STU001", "Alice"));
            engine.RegisterMember(new FacultyMember("FAC001", "Bob"));

            // Act
            var members = engine.GetAllMembers();

            // Assert
            Assert.Equal(2, members.Count);
        }

        [Fact]
        public void RegisterBook_WithSameIsbn_ReplacesExisting()
        {
            // Arrange
            var engine = new LibraryEngine();
            var book1 = new Book(new Isbn("978-0123456789"), "Old Title", "Author", BookGenre.Fiction, 1);
            var book2 = new Book(new Isbn("978-0123456789"), "New Title", "Author", BookGenre.Fiction, 1);
            engine.RegisterBook(book1);

            // Act
            engine.RegisterBook(book2);

            // Assert
            var books = engine.GetAllBooks();
            Assert.Single(books);
            Assert.Equal("New Title", books[0].Title);
        }
    }

    public class BorrowRecordTests
    {
        [Fact]
        public void BorrowRecord_WithExpression_CreatesCopyWithModifiedDueDate()
        {
            // Arrange
            var original = new BorrowRecord(
                new Isbn("978-0123456789"),
                "STU001",
                DateTime.Now,
                DateTime.Now.AddDays(15));

            // Act
            var modified = original with { DueDate = DateTime.Now.AddDays(-5) };

            // Assert
            Assert.Equal(original.BookIsbn, modified.BookIsbn);
            Assert.Equal(original.MemberId, modified.MemberId);
            Assert.Equal(original.BorrowedDate, modified.BorrowedDate);
            Assert.NotEqual(original.DueDate, modified.DueDate);
            Assert.True(modified.DueDate < DateTime.Now);
        }
    }
}
