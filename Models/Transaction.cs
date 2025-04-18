using System.ComponentModel.DataAnnotations;

namespace InternetCafeManagementSystem.Models;

public enum TransactionType
{
    Deposit,
    Withdraw,
    Payment
}

public class Transaction
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public virtual User? User { get; set; }
}