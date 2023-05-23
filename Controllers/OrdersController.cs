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
using SecurityLite.Areas.Identity.Data;
using SecurityLite.Models;

namespace SecurityLite.Controllers
{
    [Authorize(Roles = "admin,manager,client")]
    public class OrdersController : Controller
    {
        private readonly ModelsContext _context;
        private UserManager<SecurityLiteUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public OrdersController(ModelsContext context, UserManager<SecurityLiteUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }


        [Authorize(Roles = "client,manager,admin")]
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user!=null)
            {
                if (await _userManager.IsInRoleAsync(user,"admin"))
                {
                    var modelsContext = _context.Orders.Include(o => o.Client).Include(o => o.Manager);
                    return View(await modelsContext.ToListAsync());
                }
                else if (await _userManager.IsInRoleAsync(user, "client"))
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.AccountNum == user.AccountNum);
                    if (user == null || client == null)
                    {
                        return Forbid();
                    }
                    else
                    {
                        ViewBag.client = client;
                        var securityDBContext = _context.Orders.Include(o => o.Client).Include(o => o.Manager).Where(o => o.Client.AccountNum == client.AccountNum);
                        return View(await securityDBContext.ToListAsync());
                    }
                }
                else if (await _userManager.IsInRoleAsync(user, "manager"))
                {
                    var manager = await _context.Managers.FirstOrDefaultAsync(c => c.AccountNum == user.AccountNum);
                    if (user == null || manager == null)
                    {
                        return Forbid();
                    }
                    else
                    {
                        ViewBag.empl = manager;
                        var securityDBContext = _context.Orders.Include(o => o.Client).Include(o => o.Manager).Where(o => o.Manager.AccountNum == manager.AccountNum);
                        return View(await securityDBContext.ToListAsync());
                    }
                }
                else
                {
                    return new StatusCodeResult(403);
                }
            }
            else
            {
                return Forbid();
            }
        }
        [Authorize(Roles = "manager,admin")]
        public IActionResult finalDataCost(int? id)
        {
            Order order = _context.Orders.Find(id);
            if (order == null) return NotFound();
            order.price = _context.OrderDetails.Where(o => o.Order.Id == order.Id).Sum(o => o.Service.Price);
            order.DateOfComplete = order.DateOfSigning.AddDays(
                _context.OrderDetails.Where(o => o.Order.Id == order.Id).Sum(o => o.Service.PeriodOfExecution)
                );
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }
        [Authorize(Roles = "client,manager,admin")]
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            Order? order = null;
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "admin"))
                {
                    order = await _context.Orders
                        .Include(o => o.Client)
                        .Include(o => o.Manager)
                        .FirstOrDefaultAsync(m => m.Id == id);
                }
                else if (await _userManager.IsInRoleAsync(user, "client"))
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.AccountNum == user.AccountNum);
                    if (user == null || client == null)
                    {
                        return Forbid();
                    }
                    else
                    {
                        order = await _context.Orders
                        .Include(o => o.Client.AccountNum == client.AccountNum)
                        .Include(o => o.Manager)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    }
                }
                else if (await _userManager.IsInRoleAsync(user, "manager"))
                {
                    var manager = await _context.Managers.FirstOrDefaultAsync(c => c.AccountNum == user.AccountNum);
                    if (user == null || manager == null)
                    {
                        return Forbid();
                    }
                    else
                    {
                        order = await _context.Orders
                        .Include(o => o.Client)
                        .Include(o => o.Manager.AccountNum == manager.AccountNum)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    }
                }
            }
            else
            {
                return Forbid();
            }
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [Authorize(Roles = "client,manager,admin")]
        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            Order? order = null;
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "admin"))
                {
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "GetFIO");
                    ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "GetFIO");
                }
                else if (await _userManager.IsInRoleAsync(user, "client"))
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.AccountNum == user.AccountNum);
                    if (user == null || client == null)
                    {
                        return Forbid();
                    }
                    else
                    {
                        ViewData["ClientId"] = new SelectList(_context.Clients.Where(c => c.AccountNum == client.AccountNum), "Id", "GetFIO");
                        ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "GetFIO");
                    }
                }
                else if (await _userManager.IsInRoleAsync(user, "manager"))
                {
                    var manager = await _context.Managers.FirstOrDefaultAsync(c => c.AccountNum == user.AccountNum);
                    if (user == null || manager == null)
                    {
                        return Forbid();
                    }
                    else
                    {
                        ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "GetFIO");
                        ViewData["ManagerId"] = new SelectList(_context.Managers.Where(e => e.AccountNum == manager.AccountNum), "Id", "GetFIO");
                    }
                }
            }
            else
            {
                return Forbid();
            }            
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "client,manager,admin")]
        public async Task<IActionResult> Create([Bind("Id,ManagerId,ClientId,DateOfSigning,DateOfComplete,price")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "GetFIO", order.ClientId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "GetFIO", order.ManagerId);
            return View(order);
        }
        [Authorize(Roles = "manager,admin")]
        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();
            else
            {
                if (await _userManager.IsInRoleAsync(user, "admin"))
                {
                    ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "GetFIO", order.ClientId);
                    ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "GetFIO", order.ManagerId);
                }
                else if (await _userManager.IsInRoleAsync(user, "manager"))
                {
                    var manager = await _context.Managers.FirstOrDefaultAsync(c => c.AccountNum == user.AccountNum);
                    if (user == null || manager == null)
                    {
                        return Forbid();
                    }
                    else
                    {
                        ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "GetFIO", order.ClientId);
                        ViewData["ManagerId"] = new SelectList(_context.Managers.Where(m => m.Id==manager.Id), "Id", "GetFIO", order.ManagerId);
                    }
                }
            }
            
            return View(order);
        }
        [Authorize(Roles = "manager,admin")]
        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ManagerId,ClientId,DateOfSigning,DateOfComplete,price")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "GetFIO", order.ClientId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "GetFIO", order.ManagerId);
            return View(order);
        }
        [Authorize(Roles = "manager,admin")]
        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            Order? order = null;
            if (user == null) return Forbid();
            else
            {
                if (await _userManager.IsInRoleAsync(user,"admin"))
                {
                    order = await _context.Orders
                            .Include(o => o.Client)
                            .Include(o => o.Manager)
                            .FirstOrDefaultAsync(m => m.Id == id);
                }
                else if (await _userManager.IsInRoleAsync(user, "manager"))
                {
                    var manager = _context.Managers.FirstOrDefaultAsync(m => m.AccountNum == user.AccountNum);
                    if (manager == null) return NotFound();
                    else
                    {
                        order = await _context.Orders
                            .Include(o => o.Client)
                            .Include(o => o.Manager.Id==manager.Id)
                            .FirstOrDefaultAsync(m => m.Id == id);
                    }
                }
            }
            
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        [Authorize(Roles = "manager,admin")]
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ModelsContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "manager,admin")]
        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
