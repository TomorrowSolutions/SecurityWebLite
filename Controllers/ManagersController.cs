using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SecurityLite.Areas.Identity.Data;
using SecurityLite.Models;

namespace SecurityLite.Controllers
{
    [Authorize(Roles = "admin")]
    public class ManagersController : Controller
    {
        private readonly ModelsContext _context;
        private readonly UserManager<SecurityLiteUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public ManagersController(ModelsContext context, UserManager<SecurityLiteUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
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
        public IActionResult removeFromCategory(int id)
        {
            Manager manager = _context.Managers.Find(id);
            if (manager == null)
                return NotFound();
            manager.CategoryId = null;
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = manager.Id });
        }
        [Authorize(Roles = "admin")]
        public FileResult GetReport()
        {
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + "/Reports/ManagerReportTemplate.xlsx");
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + "/Reports/ManagerReport.xlsx");
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Охранная организация";
                excelPackage.Workbook.Properties.Title = "Отчет по менеджерам";
                excelPackage.Workbook.Properties.Subject = "Менеджеры";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                excelPackage.Workbook.Worksheets["Менеджеры"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Manager> managers = _context.Managers.Include(m => m.Category).ToList();
                foreach (Manager m in managers)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = m.Id;
                    worksheet.Cells[startLine, 3].Value = m.GetFIO;
                    worksheet.Cells[startLine, 4].Value = m.Education;
                    worksheet.Cells[startLine, 5].Value = m.Category.Name;
                    worksheet.Cells[startLine, 6].Value = m.DateOfStart.ToString();
                    worksheet.Cells[startLine, 7].Value = m.AccountNum;
                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type =
            "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "ManagerReport.xlsx";
            return File("/Reports/ManagerReport.xlsx", file_type, file_name);
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
