using FoodDelivery.MVC.Models;
using FoodDelivery.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _userService.GetAllRolesAsync();
            return View();
        }

        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _userService.GetAllRolesAsync();
                return View(model);
            }

            var success = await _userService.CreateUserAsync(model);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error creating user.");
            ViewBag.Roles = await _userService.GetAllRolesAsync();
            return View(model);
        }

        // GET: /Users/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userService.GetAllRolesAsync();
            ViewBag.Roles = roles;

            return View(user);
        }

        // POST: /Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _userService.GetAllRolesAsync();
                ViewBag.Roles = roles;
                return View(model);
            }

            var result = await _userService.UpdateUserAsync(model);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to update user.");
                var roles = await _userService.GetAllRolesAsync();
                ViewBag.Roles = roles;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: /Users/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to delete user.");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
