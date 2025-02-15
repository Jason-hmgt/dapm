﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToyStore.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Web.Security;
using System.Net.Http;

namespace ToyStore.Controllers
{
    public class AdminController : Controller
    {
        PhoneStoreDataContext data = new PhoneStoreDataContext();
        // GET: Admint
        [Authorize]
        public ActionResult Index(int? page)
        {

            int pageNumber = (page ?? 1);
            int pageSize = 7;
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            else
            {
                return View(data.SanPhams.ToList().OrderByDescending(n => n.NgayCapNhat).ToPagedList(pageNumber, pageSize));
            }
            
        }
        public ActionResult UserAccount(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            else
            {
                return View(data.KhachHangs.ToList().OrderBy(n => n.MaKH).ToPagedList(pageNumber, pageSize));
            }
            
        }
        public ActionResult DonDatHang(int ? page)
        {
            DonDatHang ddh = new DonDatHang();
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            else
            {
                return View(data.DonDatHangs.ToList().OrderByDescending(n => n.NgayDH).ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult Chitietdonhang (int id, int ? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            else
            {
                return View(data.ChiTietDatHangs.ToList().Where(n => n.SoDH == id).OrderBy(n => n.MaCT).ToPagedList(pageNumber, pageSize));
            }
        }
        [HttpGet]
        public ActionResult ThemmoiSP()
        {
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            ViewBag.MaD = new SelectList(data.DongSPs.ToList().OrderBy(n => n.TenDong), "MaD", "TenDong");
            ViewBag.MaPL = new SelectList(data.DongPLs.ToList().OrderBy(n => n.TenPL), "MaPL", "TenPL");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiSP(SanPham sanpham, HttpPostedFileBase fileUpload)
        {
            //Đưa dữ liệu vào dropdownload
            ViewBag.MaD = new SelectList(data.DongSPs.ToList().OrderBy(n => n.TenDong), "MaD", "TenDong");
            ViewBag.MaPL = new SelectList(data.DongPLs.ToList().OrderBy(n => n.TenPL), "MaPL", "TenPL");
            //kiểm tra đường dẫn file
            if (fileUpload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {

                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Assets/Images/sanPham"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    sanpham.AnhBia = fileName;
                    //Lưu vào CSDL
                    data.SanPhams.InsertOnSubmit(sanpham);
                    data.SubmitChanges();
                }
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public ActionResult Edit (int id)
        {
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaD = new SelectList(data.DongSPs.ToList().OrderBy(n => n.TenDong), "MaD", "TenDong", sp.MaD);
            ViewBag.MaPL = new SelectList(data.DongPLs.ToList().OrderBy(n => n.TenPL), "MaPL", "TenPL", sp.MaPL);
            return View(sp);
        }
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase ImageUpload, FormCollection collection)
        {
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);

            var tensp = collection["TenSP"];
            var gia = collection["GiaBan"];
            var mota = collection["Mota"];
            var ngaynhaphang = collection["Ngaycapnhat"];
            var kho = collection["SoLuongTon"];
            var mad = collection["MaD"];
            var mapl = collection["MaPL"];

            ViewBag.MaD = new SelectList(data.DongSPs.ToList().OrderBy(n => n.TenDong), "MaD", "TenDong", sp.MaD);
            ViewBag.MaPL = new SelectList(data.DongPLs.ToList().OrderBy(n => n.TenPL), "MaPL", "TenPL", sp.MaPL);
            if (ImageUpload == null)
            {
                sp.TenSP = tensp;
                sp.GiaBan = Convert.ToInt32(Convert.ToDecimal(gia));
                sp.MoTa = mota;
                sp.NgayCapNhat = Convert.ToDateTime(ngaynhaphang);
                sp.SoLuongTon = Convert.ToInt32(kho);
                sp.MaD = Convert.ToInt32((mad == "") ? null : mad );
                sp.MaPL = Convert.ToInt32((mapl == "") ? null : mapl);
                if (sp.MaD == 0)
                {
                    sp.MaD = null;
                }
                if (sp.MaPL == 0)
                {
                    sp.MaPL = null;
                }
                data.SubmitChanges();
                return RedirectToAction("Index", "Admin");
            }
            //Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(ImageUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Assets/Images/sanPham"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        ImageUpload.SaveAs(path);
                    }
                    //Lưu vào CSDL
                    sp.TenSP = tensp;
                    sp.GiaBan = Convert.ToInt32(Convert.ToDecimal(gia));
                    sp.MoTa = mota;
                    sp.AnhBia = fileName;
                    sp.NgayCapNhat = Convert.ToDateTime(ngaynhaphang);
                    sp.SoLuongTon = Convert.ToInt32(kho);
                    sp.MaD = Convert.ToInt32((mad == "") ? null : mad);
                    sp.MaPL = Convert.ToInt32((mapl == "") ? null : mapl);

                    if (sp.MaD == 0)
                    {
                        sp.MaD = null;
                    }
                    if (sp.MaPL == 0)
                    {
                        sp.MaPL = null;
                    }

                    data.SubmitChanges();
                }
                return RedirectToAction("Index", "Admin");
            }           
        }
        //Xóa sản phẩm
        [HttpGet]
        public ActionResult Delete (int id)
        {
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Confirm(int id)
        {
            //Lấy đối tượng sản phẩm cần xóa theo mã
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SanPhams.DeleteOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }
        //Hiển thị sản phẩm
        public ActionResult Details(int id)
        {
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            else
                return View(sp);
        }
        public ActionResult Search(string searchString, int? page)
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
            Admin ad = (Admin)Session["Taikhoanadmin"];
            if (ad == null || ad.ToString() == "")
            {
                return RedirectToAction("Login", "AdLogin");
            }
            else
            {
                return View(toy.ToPagedList(pageNum, pageSize));
            }            
        }
    }
}