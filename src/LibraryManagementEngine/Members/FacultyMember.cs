namespace LibraryManagementEngine;

public sealed class FacultyMember : Member
{
    public override int MaxBooksAllowed => 10;
    public override int BorrowingPeriodDays => 30;
    public override decimal OverdueFinePerDay => 0.10m;

    public FacultyMember(string memberId, string name) : base(memberId, name) { }
}
