#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace Booky.Models;
public class Book
{
    [Key]
    public int BookId { get; set; }

    [Required(ErrorMessage = "Title is required !!!!!!!")]
    [MinLength(2, ErrorMessage = "Title  must be at least 3 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Book Cover is required !!!!!!!")]
    public string Cover { get; set; }

    [Required(ErrorMessage = "Category is required !!!!!!!")]
    public string Category { get; set; }

    [Required(ErrorMessage = "Publication Year is required !!!!!!!")]
    public int PublicationYear { get; set; }

    [Required]
    public bool IsAvailable  { get; set; } = false;

    public int UserId { get; set; }
    // Navigation property for related User object
    public User? Author { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

     // navigation properties many to many
    public List<Like> Likes { get; set; } = new List<Like>(); 

}