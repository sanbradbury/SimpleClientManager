using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientManager.Models;

public partial class Client
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ClientId { get; set; }
    [DisplayName("Client Name")]
    public string? ClientName { get; set; }
    [DisplayName("Client Surname")]
    public string? ClientSurname { get; set; }
    [DisplayName("Contact Number")]
    public string? ContactNumber { get; set; }
    [DisplayName("Call Center Name")]
    public string? CallCenterName { get; set; }
   
    public string? Email { get; set; }
    [DisplayName("ID Number")]
    public string? Idnumber { get; set; }
    [DisplayName("Account Balance")]
    public decimal? AccountBalance { get; set; }
    [DisplayName("Capture Date")]
    public DateTime? CaptureDate { get; set; }
    [DisplayName("Captured By")]
    public string? CapturedBy { get; set; }
    [DisplayName("Payments To Date")]
    public decimal? PaymentsToDate { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

   

}
