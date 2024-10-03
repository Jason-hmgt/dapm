using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToyStore.Models;
using ToyStore.ViewModels;

using PagedList;
using PagedList.Mvc;

namespace ToyStore.Controllers
{
    public class ToyStoreController : Controller
    {
        ToyStoreDataContext data = new ToyStoreDataContext();
        // GET: ToyStore

        private List<HinhAnh_SP> listImg(int product_id)
        {
            return data.HinhAnh_SPs.OrderBy(a => a.id).Where(n => n.product_id == product_id).ToList();
        }
        private List<SanPham> SanPhamMoi (int count)
        {
            return data.SanPhams.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var spmoi = SanPhamMoi(4);
            return View(spmoi);
        }
        public ActionResult DongSP()
        {
            var dongsp = from dong in data.DongSPs select dong;
            return PartialView(dongsp);
        }
        public ActionResult DongSanPham (int id, int ? page)
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);

            var toy = from t in data.SanPhams where t.MaD == id select t;
            ViewBag.Page = pageNum;
            return View(toy.ToPagedList(pageNum, pageSize));
        }

        public ActionResult PhanLoai()
        {
            var phanloai = from pl in data.DongPLs select pl;
            return PartialView(phanloai);
        }
        public ActionResult SPtheoLoai (int id, int ? page)
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);

            var toy = from sp in data.SanPhams
                      join d in data.DongSPs
                      on sp.MaD equals d.MaD
                      where sp.MaPL == id
                      select sp;
            ViewBag.Page = pageNum;
            return View(toy.ToPagedList(pageNum, pageSize));
        }
        public ActionResult BannerAd()
        {
            return PartialView();
        }
        public ActionResult TitlePage()
        {
            return PartialView();
        }
        public ActionResult ImagePro(int id)
        {
            var imgpro = from img in data.HinhAnh_SPs where img.product_id == id select img;
            return PartialView(imgpro);
        }
        public ActionResult Details (int id)
        {
            var model = new DetailProViewModel()
            {
                Detail = data.SanPhams.Where(x => x.MaSP == id).ToList(),
                listImg = data.HinhAnh_SPs.Where(y => y.product_id == id).ToList(),
            };

            return View(model);
        }
        public ActionResult Products (int ? page)
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);
                        
            var sanpham = from sp in data.SanPhams select sp;
            ViewBag.Page = pageNum;
            return View(sanpham.ToPagedList(pageNum, pageSize));
        }

        public ActionResult CollectionPro(int id)
        {
            var collection = (from t in data.SanPhams where t.MaD == id select t).Take(4).ToList();
            return PartialView(collection);
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult SizeChart()
        {
            return View();
        }
        public ActionResult News()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Search (string searchString, int ? page)
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);
            var toy = from t in data.SanPhams
                      select t;
            if (!String.IsNullOrEmpty(searchString))
            {
                toy = toy.Where(s => s.TenSP.Contains(searchString));                
            }
            ViewBag.Search = searchString;
            ViewBag.Page = pageNum;
            return View(toy.ToPagedList(pageNum, pageSize));
        }
        public IEnumerable<SanPham> SanPhamDeXuat ()
        {
            Random rand = new Random();
            int ranNum = rand.Next(1, 229);
            return data.SanPhams.ToList().Skip(ranNum).Take(6);
        }
    }
}