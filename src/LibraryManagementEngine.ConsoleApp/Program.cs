namespace LibraryManagementEngine.ConsoleApp;

public class Program
{
    private static readonly ILibraryEngine Engine = new LibraryEngine();

    public static void Main()
    {
        bool running = true;
        while (running)
        {
            ShowMainMenu();
            string? choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1": RegisterBookMenu(); break;
                case "2": RegisterMemberMenu(); break;
                case "3": BorrowBookMenu(); break;
                case "4": ReturnBookMenu(); break;
                case "5": ListBooksMenu(); break;
                case "6": ListMembersMenu(); break;
                case "7":
                    running = false;
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter 1-7.");
                    break;
            }

            if (running)
            {
                PauseAndClear();
            }
        }
    }

    private static void ShowMainMenu()
    {
        Console.WriteLine("================================================================");
        Console.WriteLine("             WELCOME TO THE LIBRARY MANAGEMENT SYSTEM");
        Console.WriteLine("================================================================");
        Console.WriteLine("1. Register Book");
        Console.WriteLine("2. Register Member");
        Console.WriteLine("3. Borrow Book");
        Console.WriteLine("4. Return Book");
        Console.WriteLine("5. List All Books");
        Console.WriteLine("6. List All Members");
        Console.WriteLine("7. Exit");
        Console.Write("Select an option: ");
    }

    // Console.ReadKey / Console.Clear throw when input or output is redirected
    // (pipes, CI, some IDE consoles), so fall back gracefully.
    private static void PauseAndClear()
    {
        Console.WriteLine("\nPress any key to return to the main menu...");
        if (Console.IsInputRedirected)
        {
            Console.ReadLine();
            return;
        }

        Console.ReadKey(true);
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }
    }

    // ------------------------------------------------------------------
    // Book registration
    // ------------------------------------------------------------------
    private static void RegisterBookMenu()
    {
        Console.WriteLine("--- Register Book ---");
        string isbn = ReadNonEmptyString("Enter ISBN: ");
        string title = ReadNonEmptyString("Enter title: ");
        string author = ReadNonEmptyString("Enter author: ");
        BookGenre genre = ReadGenre();
        int totalCopies = ReadPositiveInt("Enter total copies: ");

        try
        {
            var book = new Book(new Isbn(isbn), title, author, genre, totalCopies);
            Engine.RegisterBook(book);
            Console.WriteLine($"Book '{title}' registered successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // ------------------------------------------------------------------
    // Member registration
    // ------------------------------------------------------------------
    private static void RegisterMemberMenu()
    {
        Console.WriteLine("--- Register Member ---");
        string memberId = ReadNonEmptyString("Enter Member ID: ");
        string name = ReadNonEmptyString("Enter Name: ");

        Console.WriteLine("Select member type:");
        Console.WriteLine("1. Student (5 books, 15-day period, $0.50/day fine)");
        Console.WriteLine("2. Faculty (10 books, 30-day period, $0.10/day fine)");
        Console.Write("Select (1-2): ");
        string? typeInput = Console.ReadLine();

        try
        {
            Member member = typeInput switch
            {
                "2" => new FacultyMember(memberId, name),
                _ => new StudentMember(memberId, name)
            };

            Engine.RegisterMember(member);
            Console.WriteLine($"Member '{name}' registered successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // ------------------------------------------------------------------
    // Borrow / Return
    // ------------------------------------------------------------------
    private static void BorrowBookMenu()
    {
        Console.WriteLine("--- Borrow Book ---");
        string memberId = ReadNonEmptyString("Enter Member ID: ");
        string isbn = ReadNonEmptyString("Enter ISBN: ");

        try
        {
            var record = Engine.BorrowBook(memberId, new Isbn(isbn));
            Console.WriteLine($"Book borrowed successfully. Due date: {record.DueDate:yyyy-MM-dd}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void ReturnBookMenu()
    {
        Console.WriteLine("--- Return Book ---");
        string memberId = ReadNonEmptyString("Enter Member ID: ");
        string isbn = ReadNonEmptyString("Enter ISBN: ");

        try
        {
            var record = Engine.ReturnBook(memberId, new Isbn(isbn));
            Console.WriteLine($"Book returned successfully on {record.ReturnedDate:yyyy-MM-dd}");

            decimal fine = Engine.CalculateOverDueFine(record);
            if (fine > 0)
            {
                Console.WriteLine($"Overdue fine due: ${fine:F2}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // ------------------------------------------------------------------
    // Listings
    // ------------------------------------------------------------------
    private static void ListBooksMenu()
    {
        Console.WriteLine("--- All Books ---");
        var books = Engine.GetAllBooks();
        if (books.Count == 0)
        {
            Console.WriteLine("No books registered.");
            return;
        }

        foreach (var book in books)
        {
            Console.WriteLine(
                $"ISBN: {book.Isbn.Value} | {book.Title} by {book.Author} | " +
                $"Genre: {book.Genre} | Copies: {book.AvailableCopies}/{book.TotalCopies}");
        }
    }

    private static void ListMembersMenu()
    {
        Console.WriteLine("--- All Members ---");
        var members = Engine.GetAllMembers();
        if (members.Count == 0)
        {
            Console.WriteLine("No members registered.");
            return;
        }

        foreach (var member in members)
        {
            string type = member.GetType().Name.Replace("Member", "");
            Console.WriteLine(
                $"ID: {member.MemberId} | {member.Name} | Type: {type} | " +
                $"Borrowed: {member.BorrowedIsbns.Count}/{member.MaxBooksAllowed}");
        }
    }

    // ------------------------------------------------------------------
    // Input helpers (validation without crashing)
    // ------------------------------------------------------------------
    private static string ReadNonEmptyString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }
            Console.WriteLine("Input cannot be empty. Please try again.");
        }
    }

    private static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value > 0)
            {
                return value;
            }
            Console.WriteLine("Please enter a valid positive whole number.");
        }
    }

    private static BookGenre ReadGenre()
    {
        Console.WriteLine("Select genre:");
        Console.WriteLine("1. Fiction");
        Console.WriteLine("2. NonFiction");
        Console.WriteLine("3. Science");
        Console.WriteLine("4. History");
        Console.WriteLine("5. Technology");
        Console.Write("Select (1-5): ");

        return Console.ReadLine() switch
        {
            "2" => BookGenre.NonFiction,
            "3" => BookGenre.Science,
            "4" => BookGenre.History,
            "5" => BookGenre.Technology,
            _ => BookGenre.Fiction
        };
    }
}
