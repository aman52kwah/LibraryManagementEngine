# Library Management Engine

A console-based library management system built in C# using object-oriented design principles. The application provides a menu-driven interface for managing books, members, borrowing, returns, and overdue fines.

## Features

| Feature | Description |
|---------|-------------|
| **Book Management** | Register books with ISBN, title, author, genre, and copy count |
| **Member Management** | Register members as Students or Faculty, each with different borrowing rules |
| **Borrowing** | Borrow books with automatic due date calculation based on member type |
| **Returns** | Return borrowed books and update availability |
| **Due Dates** | Student: 15-day borrowing period | Faculty: 30-day borrowing period |
| **Overdue Fines** | Polymorphic fine calculation: Student $0.50/day | Faculty $0.10/day |
| **Validation** | Input validation throughout — no crashes on bad input |
| **Listing** | View all registered books and members with availability status |
| **Unit Tests** | 39 xUnit tests covering all core functionality |

## OOP Design

The system is built using core object-oriented principles rather than putting everything in one class:

- **Encapsulation** — `Book` restricts copy adjustments via private setters; `Member` keeps its borrowed-books list private with a read-only public view
- **Inheritance** — `StudentMember` and `FacultyMember` inherit from abstract `Member`
- **Polymorphism** — `MaxBooksAllowed`, `BorrowingPeriodDays`, and `OverdueFinePerDay` are abstract members overridden by each member type; `LibraryEngine` treats all members through the base `Member` type
- **Interface-based design** — `ILbraryEngine` defines the contract, decoupled from the `LibraryEngine` implementation
- **Records** — `Isbn` (readonly record struct with value equality) and `BorrowRecord` (record class supporting `with` expressions for immutability)

## Project Structure

```
LibraryManagementEngine/
├── Book.cs                 # Book class — title, author, genre, ISBN, copy tracking
├── BookGenre.cs            # BookGenre enum, Isbn record struct, BorrowRecord record class
├── Member.cs               # Abstract Member base + StudentMember / FacultyMember derived types
├── ILbraryEngine.cs        # ILbraryEngine interface + LibraryEngine implementation
├── Program.cs              # Console entry point — menu-driven interactive interface
├── LibraryManagementEngine.csproj  # .NET 10 console project
├── global.json             # Pins .NET SDK 10.0.403
├── .gitignore
└── LibraryManagementEngine.Tests/
    └── UnitTests.cs        # 39 xUnit tests
```

## Member Types

| Rule | Student | Faculty |
|------|---------|---------|
| Max books allowed | 5 | 10 |
| Borrowing period | 15 days | 30 days |
| Overdue fine per day | $0.50 | $0.10 |

## Book Genres

Fiction, NonFiction, Science, History, Technology

## Getting Started

### Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) (pinned via `global.json`)

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run
```

The application displays a main menu:

```
===============================================================
             WELCOME TO THE  LIBRARY MANAGEMENT SYSTEM
===============================================================
1. Register Book
2. Register Member
3. Borrow Book
4. Return Book
5. List All Books
6. List All Members
7. Exit
Select an option:
```

### Example Workflow

1. **Register a book** — Choose option 1, enter ISBN, title, author, genre, and copy count
2. **Register a member** — Choose option 2, enter ID, name, and select Student or Faculty
3. **Borrow a book** — Choose option 3, enter Member ID and ISBN
4. **Return a book** — Choose option 4, enter Member ID and ISBN
5. **List books/members** — Choose option 5 or 6 to view current inventory and members
6. **Exit** — Choose option 7

All input is validated. Entering blank text, non-numeric values where numbers are expected, or attempting invalid operations (e.g., borrowing when no copies remain) will show an error message instead of crashing.

## Running Tests

The project includes 39 xUnit tests covering books, members, ISBN equality, engine operations (borrow/return/find), overdue fine calculation, and borrow record immutability.

```bash
dotnet test LibraryManagementEngine.Tests/LibraryManagementEngine.Tests.csproj
```

Expected output: `Passed! - Failed: 0, Passed: 39, Skipped: 0, Total: 39`

## License

This project is provided as-is for educational purposes.