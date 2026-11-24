using System;
using System.Threading.Tasks.Dataflow;
namespace LibraryManagementEngine
{
    public class Program
    
    {

        public static void Main()
        {
            Console.WriteLine("========================================================================");
            Console.WriteLine("             WELCOME TO LIBRABRY MANAGEMENT ENGINE");
            Console.WriteLine("========================================================================");


            // initialize Engine and Data Primitives
            ILbraryEngine engine = new LibraryEngine();

            Isbn cleanCodeIsbn = new("978-012345793");
            Isbn designPatternIsbn = new("978-0201633610");

            Book book1 = new(cleanCodeIsbn, "Clean Code", "Robert C. Martin", BookGenre.Technology, totalCopies: 2);
             Book book2 = new(designPatternIsbn, "Design Patterns", "Erich Gamma", BookGenre.Technology, totalCopies: 1);

             // Register Member
             Member student = new StudentMember("STU001", "Michael Amankwah");
             Member faculty = new FacultyMember("FAC-001","Clifford Opoku");

             engine.RegisterMember(student);
             engine.RegisterMember(faculty);


             //Borrowing Rules & Due Dates
             Console.WriteLine("--- Borrowing Operations ---");
             BorrowRecord rec1 = engine.BorrowBook(student.MemberId, cleanCodeIsbn);
             Console.WriteLine($"[SUCCESS] {student.Name} borrowed '{book1.Title}' .");
             Console.WriteLine($".          Due Date: {rec1.DueDate:yyy-MM-dd} ({student.BorrowingPeriodDays} days limit)");

             BorrowRecord rec2 = engine.BorrowBook(faculty.MemberId, designPatternIsbn);
             Console.WriteLine($"[SUCCESS] {faculty.Name} borrowed '{book2.Title}' .");
             Console.WriteLine($".          Due Date: {rec2.DueDate:yyy-MM-dd} ({faculty.BorrowingPeriodDays} days limit)");


        // test validaytion & copy limits
        Console.WriteLine(" ---  Validation and Inventory Checks ---");
        try
        {
            Console.WriteLine("Attempting to borrow 'Design Patterns' when 0 copies remain... ");
            engine.BorrowBook(student.MemberId, designPatternIsbn);
        }
        catch (InvalidOperationException ex)
        {
            
            Console.WriteLine($"[VALIDATION BLOCKED]: {ex.Message}\n");
        }

        // Test value value equality of Record Struct ISBNS
        Console.WriteLine(" --- Value Equality Verification --");
        Isbn duplicateIsbnSearch = new("978-0132350884");
        Console.WriteLine($"CleanCodeIsbn == duplicateIsbnSearch: {cleanCodeIsbn == duplicateIsbnSearch}");

        //Return and Fine Calculation
        Console.WriteLine("\n--- Returns & Overdue Fine Calculations ---");
        BorrowRecord returnedRec = engine.ReturnBook(student.MemberId, cleanCodeIsbn);
        Console.WriteLine($"[SUCCESS] {student.Name} returned '{book1.Title}' on {returnedRec.ReturnedDate:yyy-MM-dd HH:MM:SS}.");


        //Simulate and overdue record using a 'with' expression for testing
        BorrowRecord simulatedOverdue = returnedRec with
        {
            DueDate = DateTime.Now.AddDays(-5)
            
        };
        decimal fine = engine.CalculateOverDueFine(simulatedOverdue);
        Console.WriteLine($"[FINE CALCULATION] Simulated 5 days overdye fine for {student.Name}: ${fine:F2}");

        }
    }

    
}
