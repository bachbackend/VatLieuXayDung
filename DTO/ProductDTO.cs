namespace VatLieuXayDung.DTO
{
        public class ProductDTO
        { 

        }
        public class PagingReturn
        {
            public int TotalPageCount { get; set; }
            public int CurrentPage { get; set; }
            public int NextPage { get; set; }
            public int PreviousPage { get; set; }
        }

        public class ProductReturnDTO
        {
            public int Id { get; set; }

            public int CategoryId { get; set; }

            public string Name { get; set; } = null!;

            public string Image { get; set; } = null!;
            public int? SaleQuantity { get; set; }
            public string Description { get; set; } = null!;

            public sbyte Status { get; set; }

            public DateTime CreatedAt { get; set; }

            public string CategoryName { get; set; } = null!;
            public List<CertificateDTO> Certificates { get; set; }
        }

        public class CertificateDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

    public class ProductRequest
    {
        public int CategoryId { get; set; }
        public int? CertificateId { get; set; }  // CertificateId có thể null
        public string Name { get; set; }
        public string Description { get; set; }
        public sbyte Status { get; set; }
        public List<int>? CertificateIds { get; set; } // Các chứng chỉ có thể gắn với sản phẩm
    }

}
