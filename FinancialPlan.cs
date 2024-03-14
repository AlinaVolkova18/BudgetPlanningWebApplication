using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BudgetPlanningWebApplication;

public partial class FinancialPlan
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }
    public float? Limit { get; set; }

    public int? MandatoryPaymentId { get; set; }

    public int UserId { get; set; }
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual Category? Category { get; set; } = null!;

    public virtual MandatoryPayment? MandatoryPayment { get; set; }

    public virtual User? User { get; set; } = null!;
}
