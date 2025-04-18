using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace InternetCafeManagementSystem.Models;

public class Computer
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Tên máy")]
    [Remote(action: "VerifyComputerName", controller: "Computer", ErrorMessage = "Tên máy đã tồn tại")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Trạng thái")]
    public bool Status { get; set; } = false; // Default to false (not available)

    [Required]
    [Display(Name = "Đơn giá (giờ)")]
    [Range(0, double.MaxValue)]
    public decimal PricePerHour { get; set; }

    // Navigation property
    public ICollection<UserSession>? Sessions { get; set; }

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
}