using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller {
    public IActionResult index() {
        return View();
    }
}