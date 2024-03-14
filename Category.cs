using System;
using System.Collections.Generic;

namespace BudgetPlanningWebApplication;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TypeOfTransactionId { get; set; }

    public virtual ICollection<FinancialPlan> FinancialPlans { get; set; } = new List<FinancialPlan>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual TypeOfTransaction TypeOfTransaction { get; set; } = null!;
}
