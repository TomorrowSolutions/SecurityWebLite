using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecurityLite.Models;

namespace SecurityLite.Controllers
{
    [Authorize(Roles = "admin")]
    public class ManagersController : Controller
    {
        private readonly ModelsContext _context;

        public ManagersController(ModelsContext context)
        {
            _context = context;
        }

        // GET: Managers
        public async Task<IActionResult> Index()
        {
            var modelsContext = _context.Managers.Include(m => m.Category);
            return View(await modelsContext.ToListAsync());
        }

        // GET: Managers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Managers == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

        // GET: Managers/Create
        public IActionResult Create()
        {
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem { Text = "Среднее", Value = "Среднее", Selected=true },
                new SelectListItem { Text = "Среднее профессиональное", Value = "Среднее профессиональное", Selected=true },
                new SelectListItem { Text = "Высшее", Value = "Высшее" }
            };
            ViewBag.EduTypes = items;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Managers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Surname,Name,Patronymic,Education,CategoryId,DateOfStart,AccountNum")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", manager.CategoryId);
            return View(manager);
        }

        // GET: Managers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Managers == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }
            List<SelectListItem> items = new List<SelectListItem>();
            /*manager.Education == "Среднее" ? items.Add(new SelectListItem { Text = "Среднее", Value = "Среднее", Selected = true }) :
                                            items.Add(new SelectListItem { Text = "Среднее", Value = "Среднее" });*/
            if (manager.Education=="Среднее")
            {
                items.Add(new SelectListItem { Text = "Среднее", Value = "Среднее", Selected = true });
            }
            else
            {
                items.Add(new SelectListItem { Text = "Среднее", Value = "Среднее"});
            }
            if (manager.Education == "Среднее профессиональное")
            {
                items.Add(new SelectListItem { Text = "Среднее профессиональное", Value = "Среднее профессиональное", Selected = true });
            }
            else
            {
                items.Add(new SelectListItem { Text = "Среднее профессиональное", Value = "Среднее профессиональное" });
            }
            if (manager.Education == "Высшее")
            {
                items.Add(new SelectListItem { Text = "Высшее", Value = "Высшее", Selected = true });
            }
            else
            {
                items.Add(new SelectListItem { Text = "Высшее", Value = "Высшее" });
            }
            ViewBag.EduTypes = items;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", manager.CategoryId);
            return View(manager);
        }

        // POST: Managers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Surname,Name,Patronymic,Education,CategoryId,DateOfStart,AccountNum")] Manager manager)
        {
            if (id != manager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManagerExists(manager.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", manager.CategoryId);
            return View(manager);
        }

        // GET: Managers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Managers == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

        // POST: Managers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Managers == null)
            {
                return Problem("Entity set 'ModelsContext.Managers'  is null.");
            }
            var manager = await _context.Managers.FindAsync(id);
            if (manager != null)
            {
                _context.Managers.Remove(manager);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManagerExists(int id)
        {
          return (_context.Managers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
