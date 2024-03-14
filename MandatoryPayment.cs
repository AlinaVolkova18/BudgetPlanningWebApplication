using System;
using System.Collections.Generic;

namespace BudgetPlanningWebApplication;

public partial class MandatoryPayment
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public float Sum { get; set; }

    public string? Comment { get; set; }

    public virtual ICollection<FinancialPlan> FinancialPlans { get; set; } = new List<FinancialPlan>();
}
