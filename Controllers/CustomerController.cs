using Microsoft.AspNetCore.Mvc;
using TaThiVanAnhBTH2.Data;
using TaThiVanAnhBTH2.Models;
using Microsoft.EntityFrameworkCore;

namespace TaThiVanAnhBTH2.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _context.Customers.ToListAsync();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Customer cus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cus);
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return View("NotFound");
            }
            return View(customer);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, 
            [Bind("CustomerID,CustomerName,CustomerAge,CustomerPhone,CustomerAddress")] Customer cus)
        {
            Console.WriteLine("edit");
            if (id != cus.CustomerID)
            {
                Console.WriteLine("nf");
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("ivl");
                try
                {
                    _context.Update(cus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("er");
                    if (!CustomerExists(cus.CustomerID))
                    {
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cus);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var cus = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (cus == null)
            {
                return View("NotFound");
            }
            return View(cus);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cus = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(cus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
