using System.ComponentModel.DataAnnotations;

namespace VatLieuXayDung.DTO
{
    public class CategoryDTO
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        [MinLength(4, ErrorMessage = "Tên phải có ít nhất 4 kí tự.")]
        [MaxLength(250, ErrorMessage = "Tên không được vượt quá 250 kí tự.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [MinLength(10, ErrorMessage = "Tên phải có ít nhất 4 kí tự.")]
        [MaxLength(2000, ErrorMessage = "Tên không được vượt quá 2000 kí tự.")]
        public string Description { get; set; } = null!;
    }

}
