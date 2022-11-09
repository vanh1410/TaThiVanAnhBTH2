using Microsoft.AspNetCore.Mvc;
using TaThiVanAnhBTH2.Data;
using TaThiVanAnhBTH2.Models;
using Microsoft.EntityFrameworkCore;
using TaThiVanAnhBTH2.Models.Process;

namespace TaThiVanAnhBTH2.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ExcelProcess _excelProcess = new ExcelProcess();
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
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

        public async Task<IActionResult> Upload()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                string fileExtension = Path.GetExtension(file.FileName);
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    ModelState.AddModelError("", "Please choose excel file to upload!");
                }
                else
                {
                    //rename file when upload to server
                    var fileName = DateTime.Now.ToShortTimeString() + fileExtension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory() + "/Upload/Excels", fileName);
                    var fileLocation = new FileInfo(filePath).ToString();
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        //save file to server
                        await file.CopyToAsync(stream);
                        //read data from file and write to database
                        var dt = _excelProcess.ExcelToDataTable(fileLocation);
                        //using for loop to read data from dt
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //create a new Customer object
                            var cus = new Customer();
                            //set values for attributes
                            cus.CustomerID = dt.Rows[i][0].ToString();
                            cus.CustomerName = dt.Rows[i][1].ToString();
                            cus.CustomerAge = dt.Rows[i][2].ToString();
                            cus.CustomerPhone = dt.Rows[i][3].ToString();
                            cus.CustomerAddress = dt.Rows[i][4].ToString();
                            //add object to Context
                            _context.Customers.Add(cus);
                        }
                        //save to database
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View();
        }
    }
}
