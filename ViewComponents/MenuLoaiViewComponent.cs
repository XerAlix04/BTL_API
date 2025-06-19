using BTL.Data;
using BTL.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BTL.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;

        public MenuLoaiViewComponent(Hshop2023Context context)=> db = context;
        public IViewComponentResult Invoke()
        {
            var items = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai= lo.MaLoai,
                TenLoai = lo.TenLoai,
                Soluong=lo.HangHoas.Count
            }).OrderBy(p=>p.TenLoai);
            return View(items);
        }

    }
}
