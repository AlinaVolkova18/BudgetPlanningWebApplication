using System;
using System.Collections.Generic;

namespace BudgetPlanningWebApplication;

public partial class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();

    public virtual ICollection<FinancialPlan> FinancialPlans { get; set; } = new List<FinancialPlan>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
