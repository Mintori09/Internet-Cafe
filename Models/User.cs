using System.ComponentModel.DataAnnotations;

namespace InternetCafeManagementSystem.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Số dư")]
    [Range(0, double.MaxValue)]
    public decimal Balance { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<UserSession>? Sessions { get; set; }
}