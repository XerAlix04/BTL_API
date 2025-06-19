using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTL.Data;
using AutoMapper;
using BTL.Services;
using BTL.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace BTL.Controllers
{
    //[Authorize]
    public class HangHoasController : Controller
    {
        //private readonly Hshop2023Context _context;

        //public HangHoasController(Hshop2023Context context)
        //{
        //    _context = context;
        //}

        //// GET: HangHoas
        //public async Task<IActionResult> Index()
        //{
        //    var hshop2023Context = _context.HangHoas.Include(h => h.MaLoaiNavigation).Include(h => h.MaNccNavigation);
        //    return View(await hshop2023Context.ToListAsync());
        //}

        //// GET: HangHoas/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var hangHoa = await _context.HangHoas
        //        .Include(h => h.MaLoaiNavigation)
        //        .Include(h => h.MaNccNavigation)
        //        .FirstOrDefaultAsync(m => m.MaHh == id);
        //    if (hangHoa == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(hangHoa);
        //}

        //// GET: HangHoas/Create
        //public IActionResult Create()
        //{
        //    ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai");
        //    ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc");
        //    return View();
        //}

        //// POST: HangHoas/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("MaHh,TenHh,TenAlias,MaLoai,MoTaDonVi,DonGia,Hinh,NgaySx,GiamGia,SoLanXem,MoTa,MaNcc")] HangHoa hangHoa)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(hangHoa);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", hangHoa.MaLoai);
        //    ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", hangHoa.MaNcc);
        //    return View(hangHoa);
        //}

        //// GET: HangHoas/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var hangHoa = await _context.HangHoas.FindAsync(id);
        //    if (hangHoa == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", hangHoa.MaLoai);
        //    ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", hangHoa.MaNcc);
        //    return View(hangHoa);
        //}

        //// POST: HangHoas/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("MaHh,TenHh,TenAlias,MaLoai,MoTaDonVi,DonGia,Hinh,NgaySx,GiamGia,SoLanXem,MoTa,MaNcc")] HangHoa hangHoa)
        //{
        //    if (id != hangHoa.MaHh)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(hangHoa);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!HangHoaExists(hangHoa.MaHh))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", hangHoa.MaLoai);
        //    ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", hangHoa.MaNcc);
        //    return View(hangHoa);
        //}

        //// GET: HangHoas/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var hangHoa = await _context.HangHoas
        //        .Include(h => h.MaLoaiNavigation)
        //        .Include(h => h.MaNccNavigation)
        //        .FirstOrDefaultAsync(m => m.MaHh == id);
        //    if (hangHoa == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(hangHoa);
        //}

        //// POST: HangHoas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var hangHoa = await _context.HangHoas.FindAsync(id);
        //    if (hangHoa != null)
        //    {
        //        _context.HangHoas.Remove(hangHoa);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool HangHoaExists(int id)
        //{
        //    return _context.HangHoas.Any(e => e.MaHh == id);
        //}
        private readonly Hshop2023Context _context; // Keep for Index and Details for now, or refactor later
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;

        public HangHoasController(Hshop2023Context context, ApiService apiService, IMapper mapper)
        {
            _context = context;
            _apiService = apiService;
            _mapper = mapper;
        }

        // GET: HangHoas
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var productsResponse = await _apiService.GetHangHoasAsync(new ProductSearchRequest { Page = page, PageSize = pageSize }); // Adjust PageSize as needed
            if (productsResponse != null)
            {
                ViewBag.TotalPages = productsResponse.TotalPages;
                ViewBag.CurrentPage = page;
                return View(productsResponse.Items);
            }
            return View(new List<HangHoa>()); // Or handle error appropriately
            //var productsResponse = await _apiService.GetHangHoasAsync(new ProductSearchRequest { PageSize = 100 });
            //return View(productsResponse?.Items); // Items will now be List<HangHoa>
        }

        // GET: HangHoas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _apiService.GetHangHoasProductAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product); // Map back for the View
        }

        // GET: HangHoas/Create
        public IActionResult Create()
        {
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "TenLoai"); // Use TenLoai for display
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenNcc"); // Use TenNcc for display
            return View();
        }

        // POST: HangHoas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HangHoa hangHoa)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.CreateProductAsync(hangHoa);
                if (response?.Success == true && response.Data != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle error (e.g., log, add model error)
                    ModelState.AddModelError(string.Empty, "Failed to create product.");
                }
            }
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "TenLoai", hangHoa.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenNcc", hangHoa.MaNcc);
            return View(hangHoa);
        }

        // GET: HangHoas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _apiService.GetHangHoasProductAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            //var loais = _context.Loais.ToList(); // Force immediate evaluation
            //ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "TenLoai", product.MaLoai);
            //var nccs = _context.NhaCungCaps.ToList(); // Force immediate evaluation
            //ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenNcc", product.MaNcc);
            //return View(product); // Map to HangHoa for the form
            var loaiOptions = await _context.Loais
        .Select(l => new SelectOptionDto { Value = l.MaLoai.ToString(), Text = l.TenLoai })
        .ToListAsync();
            ViewData["MaLoai"] = new SelectList(loaiOptions, "Value", "Text", product.MaLoai);

            var nccOptions = await _context.NhaCungCaps
                .Select(n => new SelectOptionDto { Value = n.MaNcc, Text = n.TenCongTy })
                .ToListAsync();
            ViewData["MaNcc"] = new SelectList(nccOptions, "Value", "Text", product.MaNcc);
            return View(product);
        }

        // POST: HangHoas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HangHoa hangHoa)
        {
            if (id != hangHoa.MaHh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var response = await _apiService.UpdateProductAsync(id, hangHoa);
                if (response?.Success == true)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle error
                    ModelState.AddModelError(string.Empty, "Failed to update product.");
                }
            }
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "TenLoai", hangHoa.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenNcc", hangHoa.MaNcc);
            return View(hangHoa);
        }

        // GET: HangHoas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _apiService.GetProductAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<HangHoaVM, HangHoa>(product)); // Map back for the View
        }

        // POST: HangHoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _apiService.DeleteProductAsync(id);
            if (response?.Success == true)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Handle error
                ModelState.AddModelError(string.Empty, "Failed to delete product.");
                // Optionally, you could reload the delete view with an error message
                var product = await _apiService.GetHangHoasProductAsync(id);
                if (product == null) return NotFound();
                return View(product);
            }
        }
        private bool HangHoaExists(int id)
        {
            return _context.HangHoas.Any(e => e.MaHh == id);
        }
    }
}
