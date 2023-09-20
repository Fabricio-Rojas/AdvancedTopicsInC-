using BankingApp.Areas.Identity.Data;
using BankingApp.Data;
using BankingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Controllers
{
    public class AccountsController : Controller
    {
        private readonly BankingAppContext _context;

        public AccountsController(BankingAppContext context)
        {
            _context = context;
        }

        // GET: Accounts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            BankingAppUser? user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var bankingAppContext = _context.Accounts.Include(a => a.User).Where(a => a.UserId == user.Id);
            return View(await bankingAppContext.ToListAsync());
        }

        // GET: Accounts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            ViewBag.AmountMessage = "";
            return View(account);
        }

        // GET: Accounts/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.UserId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,Balance,UserId")] Account account)
        {
            if (ModelState.IsValid)
            {
                BankingAppUser? user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                user.Accounts.Add(account);
                _context.Add(account);
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UserId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;
            return View(account);
        }

        // GET: Accounts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Balance,UserId")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id, string userId)
        {
            BankingAppUser? user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'BankingAppContext.Accounts'  is null.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                user.Accounts.Remove(account);
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public IActionResult Transaction(int accountId, int amount, string submitButton)
        {
            if (submitButton == "deposit")
            {
                return RedirectToAction(nameof(Deposit), new { id = accountId, moneyAmount = amount });
            }
            else if (submitButton == "withdraw")
            {
                return RedirectToAction(nameof(Withdraw), new { id = accountId, moneyAmount = amount });
            }
            return RedirectToAction(nameof(Details), new { id = accountId });
        }

        public IActionResult Deposit(int id, int moneyAmount)
        {
            Account? account = _context.Accounts.FirstOrDefault(u => u.Id == id);

            if (moneyAmount <= 0)
            {
                ViewBag.AmountMessage = "Deposit amount must be more more than zero";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            if (account != null)
            {
                account.Balance += moneyAmount;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        public IActionResult Withdraw(int id, int moneyAmount)
        {
            Account? account = _context.Accounts.FirstOrDefault(u => u.Id == id);

            if (moneyAmount > account.Balance)
            {
                ViewBag.AmountMessage = "Withdraw amount cannot exceed the balance of the account";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            if (account != null)
            {
                account.Balance -= moneyAmount;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        private bool AccountExists(int id)
        {
            return (_context.Accounts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
