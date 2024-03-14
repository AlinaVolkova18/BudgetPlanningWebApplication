using System;
using System.Collections.Generic;

namespace BudgetPlanningWebApplication;

public partial class TypeOfTransaction
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
