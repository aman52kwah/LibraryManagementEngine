# Library Management Engine

A console-based library management system built in C# using object-oriented design. A menu-driven app manages books, members, borrowing, returns, and overdue fines.

## Project Structure

```
LibraryManagementEngine/
├── LibraryManagementEngine.sln
├── global.json                              # Pins .NET SDK 10.0.403 (rolls forward to newer 10.0.x feature bands)
├── .gitignore
├── src/
│   ├── LibraryManagementEngine/             # Class library: all domain logic
│   │   ├── Models/        Book.cs, BookGenre.cs, Isbn.cs, BorrowRecord.cs
│   │   ├── Members/       Member.cs, StudentMember.cs, FacultyMember.cs
│   │   └── Services/      ILibraryEngine.cs, LibraryEngine.cs
│   └── LibraryManagementEngine.ConsoleApp/  # Console UI (Program.cs), references the library
└── tests/
    └── LibraryManagementEngine.Tests/       # xUnit tests, reference the library
```

## OOP Design

- **Encapsulation**: `Book` restricts copy counts via private setters; `Member` hides its borrowed list behind a read-only view.
- **Inheritance / Polymorphism**: `StudentMember` and `FacultyMember` override abstract rules (`MaxBooksAllowed`, `BorrowingPeriodDays`, `OverdueFinePerDay`).
- **Interface-based design**: `ILibraryEngine` is the contract; `LibraryEngine` is the implementation.
- **Records**: `Isbn` (readonly record struct) and `BorrowRecord` (record class with `with` expressions).

## Member Types

| Rule                 | Student | Faculty |
| -------------------- | ------- | ------- |
| Max books allowed    | 5       | 10      |
| Borrowing period     | 15 days | 30 days |
| Overdue fine per day | $0.50   | $0.10   |

## Getting Started

Requires the [.NET SDK 10](https://dotnet.microsoft.com/download).

```bash
dotnet build                                                   # builds the whole solution
cd  src/LibraryManagementEngine.ConsoleApp.                    # navigate to the LibraryManagementEngine.ConsoleApp folder in src
      dotnet run                                                # runs the app
dotnet test                                                    # runs the unit tests
```

> Run `dotnet run` with `--project` from the solution folder; there is no project in the root folder.

## License

Provided as-is for educational purposes.
