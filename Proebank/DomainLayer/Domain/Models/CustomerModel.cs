namespace DomainLayer.Models
{
    public class CustomerModel
    {
        public long Id { get; set; }

        public string IdentificationNumber { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
