using System;
using System.Collections.Generic;

namespace BudgetPlanningWebApplication;

public partial class Active
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
