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
    [Authorize(Roles = "admin,manager")]
    public class ServicesController : Controller
    {
        private readonly ModelsContext _context;
        private readonly UserManager<SecurityLiteUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public ServicesController(ModelsContext context, UserManager<SecurityLiteUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }


        [Authorize(Roles = "client,manager,admin")]
        // GET: Services
        public async Task<IActionResult> Index()
        {
              return _context.Services != null ? 
                          View(await _context.Services.ToListAsync()) :
                          Problem("Entity set 'ModelsContext.Services'  is null.");
        }
        [Authorize(Roles = "client,manager,admin")]
        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }
        [Authorize(Roles = "admin")]
        public FileResult GetReport()
        {
            FileInfo fi = new FileInfo(_appEnvironment.WebRootPath + "/Reports/ServiceReportTemplate.xlsx");
            FileInfo fr = new FileInfo(_appEnvironment.WebRootPath + "/Reports/ServiceReport.xlsx");
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Охранная организация";
                excelPackage.Workbook.Properties.Title = "Отчет по услугам";
                excelPackage.Workbook.Properties.Subject = "Услуги";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet =
                excelPackage.Workbook.Worksheets["Услуги"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Service> services = _context.Services.ToList();
                List<OrderDetail> orderDetails = _context.OrderDetails.Include(o => o.GuardedObject).Include(o => o.Order).Include(o => o.Service).ToList();
                foreach (Service s in services)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = s.Id;
                    worksheet.Cells[startLine, 3].Value = s.Name;
                    worksheet.Cells[startLine, 4].Value = s.PeriodOfExecution;
                    worksheet.Cells[startLine, 5].Value = s.Price;
                    //Частота пользования услугой
                    worksheet.Cells[startLine, 6].Value = Math.Round((double)orderDetails.Where(o => o.Service.Id == s.Id).
                        GroupBy(o => o.Order).Count() / orderDetails.GroupBy(o => o.Order).Count(), 2);
                    worksheet.Cells[startLine, 7].Value = s.Description;
                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type =
            "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "ServiceReport.xlsx";
            return File("/Reports/ServiceReport.xlsx", file_type, file_name);
        }
        [Authorize(Roles = "manager,admin")]
        // GET: Services/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "manager,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,PeriodOfExecution,Description")] Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }
        [Authorize(Roles = "manager,admin")]
        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }
        [Authorize(Roles = "manager,admin")]
        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,PeriodOfExecution,Description")] Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.Id))
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
            return View(service);
        }
        [Authorize(Roles = "manager,admin")]
        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }
        [Authorize(Roles = "manager,admin")]
        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Services == null)
            {
                return Problem("Entity set 'ModelsContext.Services'  is null.");
            }
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "manager,admin")]
        private bool ServiceExists(int id)
        {
          return (_context.Services?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
