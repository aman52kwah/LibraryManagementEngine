namespace LibraryManagementEngine;

public sealed class StudentMember : Member
{
    public override int MaxBooksAllowed => 5;
    public override int BorrowingPeriodDays => 15;
    public override decimal OverdueFinePerDay => 0.50m;

    public StudentMember(string memberId, string name) : base(memberId, name) { }
}
