using System;
using System.Collections.Generic;

namespace BudgetPlanningWebApplication;

public partial class Transaction
{
    public int Id { get; set; }

    public int TypeOfTransactionId { get; set; }

    public int CategoryId { get; set; }

    public int ActiveId { get; set; }

    public float Sum { get; set; }

    public DateTime Date { get; set; }

    public string? Comment { get; set; }

    public int UserId { get; set; }

    public virtual Active Active { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual TypeOfTransaction TypeOfTransaction { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
