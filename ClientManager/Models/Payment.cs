using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ClientManager.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? ClientId { get; set; }

    public DateTime? DateOfPayment { get; set; }

    public decimal? AmountOfPayment { get; set; }

    public string? ReferenceForPayment { get; set; }

    public virtual Client? Client { get; set; }
}
