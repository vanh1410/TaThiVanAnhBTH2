using Microsoft.AspNetCore.Mvc;
using TaThiVanAnhBTH2.Data;
using TaThiVanAnhBTH2.Models;
using Microsoft.EntityFrameworkCore;

namespace TaThiVanAnhBTH2.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _context.Persons.ToListAsync();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Person ps)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ps);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ps);
        }

        private bool PersonExists(string id)
        {
            return _context.Persons.Any(e => e.PersonID == id);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return View("NotFound");
            }
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PersonID,PersonName")] Person ps)
        {

            if (id != ps.PersonID)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ps);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(ps.PersonID))
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
            return View(ps);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var ps = await _context.Persons
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (ps == null)
            {
                return View("NotFound");
            }
            return View(ps);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ps = await _context.Persons.FindAsync(id);
            _context.Persons.Remove(ps);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
