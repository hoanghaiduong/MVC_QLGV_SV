using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Text;
using MVC_QLGV_SV.Models;
using System.Data.SqlClient;
using System.Data;

namespace MVC_QLGV_SV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SinhVienController : Controller
    {
        [HttpGet]
        [Route("lay-sinh-vien")]
        public async Task<IActionResult> GetSinhVienTheoID([FromQuery] string mssv, [FromQuery] int madot)
        {
            string storedProcedureName = "[Tracking].[PSP_230711_SinhVien_LaySinhVienTheoID]";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                {"@MSSV", mssv},
                 {"@MaDot", madot }
             };
     

            List<Dictionary<string, object>> results = SQLHelper.ExecuteStoredProcedure(storedProcedureName, parameters);

            if (results.Count > 0)
            {
                return Json(results[0]);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("dang-ky-giao-vien-huong-dan")]
        public async Task<IActionResult> DangKyGiaoVien(DangKyGiaoVienModel model)
        {
            string storedProcedureName = "[Tracking].[PSP_230711_DangKyGiaoVien_Insert]";

            var parameters = new Dictionary<string, object>
            {
                { "@MaGiaoVien", model.MaGiaoVien },
                { "@MSSV", model.MSSV },
                { "@MaDot", model.MaDot }
            };
            try
            {
                List<Dictionary<string, object>> results = SQLHelper.ExecuteStoredProcedure(storedProcedureName, parameters);

                // Kiểm tra xem có kết quả trả về từ stored procedure không
                if (results.Count > 0)
                {
                    // Xử lý khi đăng ký giáo viên thành công
                    return Ok(new
                    {
                        Message = "Đăng ký giáo viên hướng dẫn thành công!",
                        StatusCode = HttpStatusCode.OK,
                    });
                }
                else
                {
                    // Xử lý khi giá trị MaDot không hợp lệ
                    return BadRequest(new
                    {
                        ErrorCode = 400,
                        ErrorMessage = "Giá trị MaDot không hợp lệ.",
                    });
                }
            }
            catch (SqlException e)
            {
                var errorResponse = new
                {
                    ErrorCode = e.Number,
                    ErrorMessage = e.Message
                };

                return BadRequest(errorResponse);
            }
        }
        [HttpGet]
        [Route("lay-danh-sach-GVHD")]
        public async Task<IActionResult> GetGiaoVienHuongDanCombo()
        {
            string storedProcedureName = "[Tracking].[PSP_230711_GiaoVienHuongDan_LayCombo]";
            List<Dictionary<string, object>> results = SQLHelper.ExecuteStoredProcedure(storedProcedureName);

            if (results.Count > 0)
            {
                var formattedResults = results.Select(r => new
                {
                    maGiaoVien = r["MaGiaoVien"].ToString().TrimEnd(),
                    hoTenGiaoVien = r["HoTenGiaoVien"].ToString()
                });
                return Json(formattedResults);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("lay-thong-tin-gvhd")]
        public async Task<IActionResult> GetGiaoVienHuongDanInfo([FromQuery] string maGV)
        {
            string storedProcedureName = "[Tracking].[PSP_230711_GiaoVienHuongDan_LayThongTinTheoID]";

            var parameters = new Dictionary<string, object>
    {
        { "@MaGiaoVien", maGV }
    };
            List<Dictionary<string, object>> results = SQLHelper.ExecuteStoredProcedure(storedProcedureName, parameters);

            if (results.Count > 0)
            {
                if (results.Count == 1)
                {
                    var result = results[0];
                    var formattedResult = new
                    {
                        maGiaoVien = result["MaGiaoVien"].ToString(),
                        hoTenGiaoVien = result["HoTenGiaoVien"].ToString(),
                        hocVi = result["HocVi"].ToString(),
                        dienThoai = result["DienThoai"].ToString(),
                        email = result["Email"].ToString(),
                        moTaDinhHuong = result["MoTaDinhHuong"].ToString(),
                        soLuong = Convert.ToInt32(result["SoLuong"]),
                        soLuongConLai = Convert.ToInt32(result["SoLuongConLai"])
                    };

                    return Json(formattedResult);
                }
                else
                {
                    var formattedResults = results.Select(r => new
                    {
                        maGiaoVien = r["MaGiaoVien"].ToString(),
                        hoTenGiaoVien = r["HoTenGiaoVien"].ToString(),
                        hocVi = r["HocVi"].ToString(),
                        dienThoai = r["DienThoai"].ToString(),
                        email = r["Email"].ToString(),
                        moTaDinhHuong = r["MoTaDinhHuong"].ToString(),
                        soLuong = Convert.ToInt32(r["SoLuong"]),
                        soLuongConLai = Convert.ToInt32(r["SoLuongConLai"])
                    });

                    return Json(formattedResults);
                }
            }
            else
            {
                return NotFound(new { Message = "Không tìm thấy giáo viên hướng dẫn!", StatusCode = HttpStatusCode.NotFound });
            }
        }

    }
}
