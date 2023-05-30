using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecurityLite.Models;

namespace SecurityLite.Controllers
{
    [Authorize(Roles = "admin,manager")]
    public class GuardedObjectsController : Controller
    {
        private readonly ModelsContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public GuardedObjectsController(ModelsContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: GuardedObjects
        public async Task<IActionResult> Index()
        {
              return _context.GuardedObjects != null ? 
                          View(await _context.GuardedObjects.ToListAsync()) :
                          Problem("Entity set 'ModelsContext.GuardedObjects'  is null.");
        }
        public IActionResult DeleteImage(int id)
        {
            GuardedObject gObject = _context.GuardedObjects.Find(id);
            if (gObject == null)
            {
                return NotFound();
            }
            if (System.IO.File.Exists(_appEnvironment.WebRootPath+gObject.Image)) 
                System.IO.File.Delete(_appEnvironment.WebRootPath + gObject.Image);
            gObject.Image = null;

            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = gObject.Id });
        }

        // GET: GuardedObjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GuardedObjects == null)
            {
                return NotFound();
            }

            var guardedObject = await _context.GuardedObjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (guardedObject == null)
            {
                return NotFound();
            }
            if (!guardedObject.Image.IsNullOrEmpty())
            {
                byte[] photodata = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + guardedObject.Image);

                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }

            return View(guardedObject);
        }

        // GET: GuardedObjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GuardedObjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,City,Street,Building")] GuardedObject guardedObject, IFormFile? upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/Files/" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }
                    guardedObject.Image = path;
                }
                _context.Add(guardedObject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guardedObject);
        }

        // GET: GuardedObjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GuardedObjects == null)
            {
                return NotFound();
            }

            var guardedObject = await _context.GuardedObjects.FindAsync(id);
            if (guardedObject == null)
            {
                return NotFound();
            }
            if (!guardedObject.Image.IsNullOrEmpty())
            {
                byte[] photodata = System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + guardedObject.Image);

                ViewBag.Photodata = photodata;
            }
            else
            {
                ViewBag.Photodata = null;
            }
            return View(guardedObject);
        }

        // POST: GuardedObjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,City,Street,Building")] GuardedObject guardedObject,
            IFormFile? upload)
        {
            if (id != guardedObject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    string path = "/Files/" + upload.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await upload.CopyToAsync(fileStream);
                    }

                    if (!guardedObject.Image.IsNullOrEmpty())
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + guardedObject.Image);
                    }

                    guardedObject.Image = path;
                }
                try
                {
                    _context.Update(guardedObject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuardedObjectExists(guardedObject.Id))
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
            return View(guardedObject);
        }

        // GET: GuardedObjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GuardedObjects == null)
            {
                return NotFound();
            }

            var guardedObject = await _context.GuardedObjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (guardedObject == null)
            {
                return NotFound();
            }

            return View(guardedObject);
        }

        // POST: GuardedObjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GuardedObjects == null)
            {
                return Problem("Entity set 'ModelsContext.GuardedObjects'  is null.");
            }
            var guardedObject = await _context.GuardedObjects.FindAsync(id);
            if (guardedObject != null)
            {
                _context.GuardedObjects.Remove(guardedObject);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GuardedObjectExists(int id)
        {
          return (_context.GuardedObjects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
