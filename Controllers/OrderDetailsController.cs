using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecurityLite.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SecurityLite.Controllers
{
    [Authorize(Roles = "admin,manager")]
    public class OrderDetailsController : Controller
    {
        private readonly ModelsContext _context;

        public OrderDetailsController(ModelsContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var modelsContext = _context.OrderDetails.Include(o => o.GuardedObject).Include(o => o.Order).Include(o => o.Service);
            return View(await modelsContext.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.GuardedObject)
                .Include(o => o.Order)
                .Include(o => o.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public IActionResult Create(int? ordId, int? gObjId, int? servId)
        {
            var order = _context.Orders.FindAsync(ordId).Result;
            var gObject = _context.GuardedObjects.FindAsync(gObjId).Result;
            var service = _context.Services.FindAsync(servId).Result;
            if (order == null || gObject == null || service == null) return NotFound();
            ViewBag.Order = order;
            ViewBag.GuardedObject = gObject;
            ViewBag.Service = service;
            //ViewData["ServiceId"] = new SelectList(_context.Services.Select(s=>s.Id==servId), "Id", "Name");
            return View();
        }
        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,GuardedObjectId,ServiceId,Quantity")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //viewbag
            //var order = orderDetail.Order;
            //var gObject = orderDetail.GuardedObject;
            //var service = orderDetail.Service;
            //if (order == null || gObject == null || service == null) return NotFound();
            ViewBag.Order = orderDetail.Order;
            ViewBag.GuardedObject = orderDetail.GuardedObject;
            ViewBag.Service = orderDetail.Service;
            return View(orderDetail);
        }
        public async Task<IActionResult> SelectOrder()
        {
            return View(await _context.Orders.Include(o => o.Client).Include(o => o.Manager).ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> SelectOrder(int? orderId)
        {
            var order = _context.Orders.FindAsync(orderId).Result;
            if (order == null) return NotFound();
            ViewBag.Order = order;
            return View("SelectObject", await _context.GuardedObjects.ToListAsync());
        }
        public async Task<IActionResult> SelectObject()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SelectObject(int? orderId, int? objectId)
        {
            var order = _context.Orders.FindAsync(orderId).Result;
            var gObject = _context.GuardedObjects.FindAsync(objectId).Result;
            if (order == null||gObject==null) return NotFound();
            ViewBag.Order = order;
            ViewBag.Object = gObject;
            return View("SelectService", await _context.Services.ToListAsync());
        }
        public async Task<IActionResult> SelectService()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SelectService(int? orderId, int? objectId,int? serviceId)
        {
            var order = _context.Orders.FindAsync(orderId).Result;
            var gObject = _context.GuardedObjects.FindAsync(objectId).Result;
            var service = _context.Services.FindAsync(serviceId).Result;
            if (order == null || gObject == null||service==null) return NotFound();
            ViewBag.OrderId = order;
            ViewBag.ObjectId = gObject;
            ViewBag.ServiceId = service;
            return RedirectToAction(nameof(Create), new {ordId=orderId, gObjId=objectId, servId=serviceId });
        }




        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["GuardedObjectId"] = new SelectList(_context.GuardedObjects, "Id", "Building", orderDetail.GuardedObjectId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", orderDetail.ServiceId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,GuardedObjectId,ServiceId,Quantity")] OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.Id))
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
            ViewData["GuardedObjectId"] = new SelectList(_context.GuardedObjects, "Id", "Building", orderDetail.GuardedObjectId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", orderDetail.ServiceId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.GuardedObject)
                .Include(o => o.Order)
                .Include(o => o.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderDetails == null)
            {
                return Problem("Entity set 'ModelsContext.OrderDetails'  is null.");
            }
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
          return (_context.OrderDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
