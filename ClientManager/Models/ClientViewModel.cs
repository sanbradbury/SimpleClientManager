using System.ComponentModel;

namespace ClientManager.Models
{
    public class ClientViewModel
    {
        public int ClientId { get; set; }
        [DisplayName("First Name")]
        public string? ClientName { get; set; }

        public string? ClientSurname { get; set; }

        public string? ContactNumber { get; set; }

        public string? CallCenterName { get; set; }

        public string? Email { get; set; }

        public string? Idnumber { get; set; }

        public decimal? AccountBalance { get; set; }

        public DateTime? CaptureDate { get; set; }

        public string? CapturedBy { get; set; }
        [DisplayName("Payments To Date")]
        public decimal? PaymentsToDate { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        //Displaying name + surname
        [DisplayName("Full Name")]
        public string FullName
        {
            get { return ClientName + " " + ClientSurname; }
        }
    }
}
