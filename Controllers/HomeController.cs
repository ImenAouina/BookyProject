using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Booky.Models;

namespace Booky.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("userId") != null)
        {
            return RedirectToAction("Dashboard");
        }
        return View();
    }

    //Register-login-Logout

    [HttpPost("users/create")]
    public IActionResult CreateUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already taken!!");
                return View("Index");
            }
            // Hash Password
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            System.Console.WriteLine(newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            // Add User ID in Session
            HttpContext.Session.SetInt32("userId", newUser.UserId);
            HttpContext.Session.SetString("username", newUser.FirstName);
            return RedirectToAction("Dashboard");
        }
        return View("Index");
    }

    [HttpPost("/users/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if (ModelState.IsValid)
        {
            // Login
            // search for a user that match the login email
            var UserFromDB = _context.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail);
            if (UserFromDB == null)
            {
                ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                return View("Index");
            }
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            //  verify Password 
            var result = hasher.VerifyHashedPassword(loginUser, hashedPassword: UserFromDB.Password, loginUser.LoginPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                return View("Index");
            }
            HttpContext.Session.SetInt32("userId", UserFromDB.UserId);
            HttpContext.Session.SetString("username", UserFromDB.FirstName);
            return RedirectToAction("Dashboard");
        }
        return View("Index");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    //Dashboard//
    [HttpGet("books")]
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        List<Book> AllBooks = _context.Books
        .Include(l=> l.Likes)
        .Include(a=>a.Author)
        .OrderByDescending(o => o.CreatedAt)
        .ToList();
        return View(AllBooks);
    }

    //search by Category
    [HttpPost]
    public IActionResult SearchByCategory(string category)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        List<Book> SearchedBooks = _context.Books
        .Include(l=> l.Likes)
        .Include(a=>a.Author)
        .OrderByDescending(o => o.CreatedAt)
        .Where(c => category.Contains(c.Category))
        .ToList();
        return View("Dashboard", SearchedBooks);
    }

    //Add Book//
    [HttpGet("books/new")]
    public IActionResult AddBook()
    {
         if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        return View();
    }

    [HttpPost]
    public IActionResult CreateBook(Book newBook)
    {
         if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        
        if (ModelState.IsValid)
        {
            _context.Books.Add(newBook);
            _context.SaveChanges();
            return RedirectToAction("ShowDetails", new { bookId = newBook.BookId});
        }
        return View("AddBook");
    }

    //Show book Deatils //
    [HttpGet("books/{bookId}")]
    public IActionResult ShowDetails(int bookId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Book? book = _context.Books
        .Include(l=>l.Likes)
        .Include(a=>a.Author)
        .FirstOrDefault(b=> b.BookId == bookId);
        return View(book);
    }

     //Delete Book //
    public IActionResult DeleteBook(int bookId)
    {
        Book? BookToRemove = _context.Books.SingleOrDefault(b=> b.BookId == bookId);
        _context.Books.Remove(BookToRemove);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }
     //Edit Book //
    [HttpGet("books/{bookId}/edit")]
     public IActionResult EditBook(int bookId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Book oneBook = _context.Books
        .FirstOrDefault(b => b.BookId == bookId);
        return View(oneBook);
    }

    [HttpPost]
    public IActionResult UpdateBook(int bookId, Book newUpdateBook)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Book oldBook = _context.Books.FirstOrDefault(b => b.BookId == bookId);
        if (ModelState.IsValid)
        {
            oldBook.Title = newUpdateBook.Title;
            oldBook.Cover = newUpdateBook.Cover;
            oldBook.Category = newUpdateBook.Category;
            oldBook.PublicationYear = newUpdateBook.PublicationYear;
            oldBook.IsAvailable = newUpdateBook.IsAvailable;
            oldBook.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("ShowDetails", new { bookId });
        }
        return View("EditBook", oldBook);
    }
    // like and unlike book in the show details page //
    //like
    [HttpPost]
    public IActionResult LikeBook(Like newLike)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        _context.Likes.Add(newLike);
        _context.SaveChanges();
        return RedirectToAction("ShowDetails", new { bookId = newLike.BookId });   
    } 

    //unlike 
    [HttpPost]
    public IActionResult UnLike(int bookId, Like newLike)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Like? likeToDelete = _context.Likes
        .FirstOrDefault(a => a.BookId == newLike.BookId
         && a.UserId == newLike.UserId);

        _context.Likes.Remove(likeToDelete);
        _context.SaveChanges();
        return RedirectToAction("ShowDetails", new { bookId });   
    }

    // like and unlike book in the Dashboard //
    //like 
    public IActionResult Like(int bookId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Like newLike = new Like
        {
            UserId = (int)HttpContext.Session.GetInt32("userId"),
            BookId = bookId
        };
            _context.Likes.Add(newLike);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");   
    }
    //UnLike
    public IActionResult UnLikeBook(int bookId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Like? LikeToDelete = _context.Likes
        .FirstOrDefault(l => l.BookId == bookId 
        && l.UserId == (int)HttpContext.Session.GetInt32("userId"));
        
        _context.Likes.Remove(LikeToDelete);
        _context.SaveChanges();     
        return RedirectToAction("Dashboard");   
    }

    //display favorite Books //
    [HttpGet("books/favoriteBooks")]
    public IActionResult FavoriteBooks()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        List<Like> FavoriteBooks = _context.Likes
        .Include(b => b.Book)
        .ThenInclude(a => a.Author)
        .Where(b=>b.UserId == (int)HttpContext.Session.GetInt32("userId"))
        .ToList();
        return View(FavoriteBooks);
    }

    //delete favorite book //
    [HttpPost]
    public IActionResult DeleteFavoriteBook(int likeId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Like likeToRemove = _context.Likes.SingleOrDefault(l => l.LikeId == likeId);
        _context.Likes.Remove(likeToRemove);
        _context.SaveChanges();
        return RedirectToAction("FavoriteBooks");
    }


    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
