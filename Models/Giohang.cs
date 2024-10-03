using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToyStore.Models
{
    public class Giohang
    {
        PhoneStoreDataContext data = new PhoneStoreDataContext();
        public int iMasp { set; get; }
        public String sTensp { set; get; }
        public String sAnhbia { set; get; }
        public Double dDongia { get; set; }
        public int iSoluong { set; get; }
        public Double dThanhtien
        {
            get
            {
                return iSoluong * dDongia;
            }
        }
        public Giohang(int MaSP)
        {
            iMasp = MaSP;
            SanPham sanPham = data.SanPhams.Single(model => model.MaSP == iMasp);
            sTensp = sanPham.TenSP;
            sAnhbia = sanPham.AnhBia;
            dDongia = double.Parse(sanPham.GiaBan.ToString());
            iSoluong = 1;
        }
    }
}