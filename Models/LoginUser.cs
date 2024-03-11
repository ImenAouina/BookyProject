#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace Booky.Models;
public class LoginUser
{

    [Required(ErrorMessage = "Email address must be present")]
    [EmailAddress] 
    [Display(Name = "Email")]
    public string LoginEmail { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string LoginPassword { get; set; }

}