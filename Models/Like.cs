#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace Booky.Models;
public class Like
{
    
    [Key]
    public int LikeId  { get; set; }
    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }
    [Required]
    public int BookId { get; set; }
    public Book? Book { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}