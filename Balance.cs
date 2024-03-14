using System;
using System.Collections.Generic;

namespace BudgetPlanningWebApplication;

public partial class Balance
{
    public int Id { get; set; }

    public double Sum { get; set; }

    public int ActiveId { get; set; }

    public int UserId { get; set; }

    public virtual Active Active { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
