namespace VatLieuXayDung.DTO
{
    public class ContactRequest
    {
        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int Status { get; set; }

        public DateTime ContactDate { get; set; }
    }
}
