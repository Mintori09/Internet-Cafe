using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetCafeManagementSystem.Models;

public class UserSession
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Mã người dùng")]
    public int UserId { get; set; }

    [Required]
    [Display(Name = "Mã máy")]
    public int ComputerId { get; set; }

    [Required]
    [Display(Name = "Thời gian bắt đầu")]
    public DateTime StartTime { get; set; }

    [Display(Name = "Thời gian kết thúc")]
    public DateTime? EndTime { get; set; }

    [Display(Name = "Tổng chi phí")]
    public decimal TotalCost { get; set; }

    [NotMapped]
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;

    // Navigation properties
    public User? User { get; set; }
    public Computer? Computer { get; set; }
}