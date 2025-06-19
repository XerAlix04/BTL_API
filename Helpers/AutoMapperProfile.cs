using AutoMapper;
using BTL.Data;
using BTL.ViewModel;

namespace BTL.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<HangHoa, HangHoaVM>()
            .ForMember(dest => dest.MaHh, opt => opt.MapFrom(src => src.MaHh))
            .ForMember(dest => dest.TenHh, opt => opt.MapFrom(src => src.TenHh))
            .ForMember(dest => dest.Hinh, opt => opt.MapFrom(src => src.Hinh))
            .ForMember(dest => dest.Gia, opt => opt.MapFrom(src => src.DonGia)) // Assuming DonGia in HangHoa maps to Gia in HangHoaVM
            .ForMember(dest => dest.MoTaNgan, opt => opt.MapFrom(src => src.MoTa))
            .ForMember(dest => dest.TenLoai, opt => opt.MapFrom(src => src.MaLoaiNavigation.TenLoai)) // Mapping from the related LoaiHangHoa entity
            .ForMember(dest => dest.MaLoai, opt => opt.MapFrom(src => src.MaLoai))
            .ForMember(dest => dest.MaNcc, opt => opt.MapFrom(src => src.MaNcc)); // Assuming MaNcc is a string in HangHoa

            CreateMap<HangHoaVM, HangHoa>()
            .ForMember(dest => dest.MaHh, opt => opt.MapFrom(src => src.MaHh))
            .ForMember(dest => dest.TenHh, opt => opt.MapFrom(src => src.TenHh))
            .ForMember(dest => dest.Hinh, opt => opt.MapFrom(src => src.Hinh))
            .ForMember(dest => dest.DonGia, opt => opt.MapFrom(src => src.Gia))
            .ForMember(dest => dest.MoTa, opt => opt.MapFrom(src => src.MoTaNgan))
            //.ForMember(dest => dest.MaLoaiNavigation.TenLoai, opt => opt.MapFrom(src => src.TenLoai)) // Assuming you don't want to map this directly
            .ForMember(dest => dest.MaLoai, opt => opt.MapFrom(src => src.MaLoai))
            .ForMember(dest => dest.MaNcc, opt => opt.MapFrom(src => src.MaNcc));

            CreateMap<HangHoa, ChiTietHangHoaVM>()
                .ForMember(dest => dest.MaHh, opt => opt.MapFrom(src => src.MaHh))
                .ForMember(dest => dest.TenHH, opt => opt.MapFrom(src => src.TenHh))
                .ForMember(dest => dest.Hinh, opt => opt.MapFrom(src => src.Hinh))
                .ForMember(dest => dest.DonGia, opt => opt.MapFrom(src => src.DonGia))
                .ForMember(dest => dest.MoTaNgan, opt => opt.MapFrom(src => src.MoTa))
                .ForMember(dest => dest.TenLoai, opt => opt.MapFrom(src => src.MaLoaiNavigation.TenLoai))
                .ForMember(dest => dest.ChiTiet, opt => opt.MapFrom(src => src.MoTa));
            // You might need to add mappings for DiemDanhGia and SoLuongTon if they come from HangHoa or related entities.

            // Mapping from RegisterVM to KhachHang
            CreateMap<RegisterVM, KhachHang>()
                .ForMember(dest => dest.MaKh, opt => opt.MapFrom(src => src.MaKh))
                .ForMember(dest => dest.HoTen, opt => opt.MapFrom(src => src.HoTen))
                .ForMember(dest => dest.GioiTinh, opt => opt.MapFrom(src => src.GioiTinh))
                .ForMember(dest => dest.NgaySinh, opt => opt.MapFrom(src => src.NgaySinh))
                .ForMember(dest => dest.DiaChi, opt => opt.MapFrom(src => src.DiaChi))
                .ForMember(dest => dest.DienThoai, opt => opt.MapFrom(src => src.DienThoai))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Hinh, opt => opt.MapFrom(src => src.Hinh))
                // You might not want to map MatKhau directly here as it will be hashed later
                // .ForMember(dest => dest.MatKhau, opt => opt.MapFrom(src => src.MatKhau))
                // These properties are typically set during registration logic
                .ForMember(dest => dest.HieuLuc, opt => opt.Ignore()) // Set in controller
                .ForMember(dest => dest.VaiTro, opt => opt.Ignore())   // Set in controller
                .ForMember(dest => dest.RandomKey, opt => opt.Ignore()) // Set in controller
                                                                        // Ignore navigation properties
                .ForMember(dest => dest.BanBes, opt => opt.Ignore())
                .ForMember(dest => dest.HoaDons, opt => opt.Ignore())
                .ForMember(dest => dest.YeuThiches, opt => opt.Ignore());

            // Optional: Mapping from KhachHang to RegisterVM (if needed for display or API responses)
            CreateMap<KhachHang, RegisterVM>()
                .ForMember(dest => dest.MaKh, opt => opt.MapFrom(src => src.MaKh))
                .ForMember(dest => dest.HoTen, opt => opt.MapFrom(src => src.HoTen))
                .ForMember(dest => dest.GioiTinh, opt => opt.MapFrom(src => src.GioiTinh))
                .ForMember(dest => dest.NgaySinh, opt => opt.MapFrom(src => src.NgaySinh))
                .ForMember(dest => dest.DiaChi, opt => opt.MapFrom(src => src.DiaChi))
                .ForMember(dest => dest.DienThoai, opt => opt.MapFrom(src => src.DienThoai))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Hinh, opt => opt.MapFrom(src => src.Hinh));
            // You might not want to map MatKhau back
            // .ForMember(dest => dest.MatKhau, opt => opt.MapFrom(src => src.MatKhau));
           
            CreateMap<KhachHang, KhachHangResponseDto>()
            .ForMember(dest => dest.MaKh, opt => opt.MapFrom(src => src.MaKh))
            .ForMember(dest => dest.HoTen, opt => opt.MapFrom(src => src.HoTen))
            .ForMember(dest => dest.GioiTinh, opt => opt.MapFrom(src => src.GioiTinh))
            .ForMember(dest => dest.NgaySinh, opt => opt.MapFrom(src => src.NgaySinh))
            .ForMember(dest => dest.DiaChi, opt => opt.MapFrom(src => src.DiaChi))
            .ForMember(dest => dest.DienThoai, opt => opt.MapFrom(src => src.DienThoai))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Hinh, opt => opt.MapFrom(src => src.Hinh));

            CreateMap<KhachHang, LoginVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.MaKh))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.MatKhau));
        }

    }
}