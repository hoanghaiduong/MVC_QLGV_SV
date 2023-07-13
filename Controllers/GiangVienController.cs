using Microsoft.AspNetCore.Mvc;
using MVC_QLGV_SV.Models;
using System.Data.SqlClient;

namespace MVC_QLGV_SV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GiangVienController : Controller
    {
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> LayThongTinGiangVien([FromQuery] string maGiaoVien)
        {
            string storedProcedureName = "[Tracking].[PSP_230711_GiangVien_LayThongTinGiangVien]";
            var parameters = new Dictionary<string, object>
            {
                { "@MaGiaoVien", maGiaoVien },
            };
            List<Dictionary<string, object>> results = SQLHelper.ExecuteStoredProcedure(storedProcedureName, parameters);

            if (results.Count > 0)
            {
                return Json(results[0]);
            }
            else
            {
                return NotFound(new
                {
                    Message="Không tìm thấy giảng viên",
                    status=404
                });
            }
        }
        [HttpPost]
        [Route("giang-vien-dang-ky-thong-tin")]
        public async Task<IActionResult> DangKyThongTin([FromBody] GiaoVienHuongDanDangKy request)
        {
            string storedProcedureName = "[Tracking].[PSP_230711_GiaoVienHuongDan_DangKyThongTin]";
            var parameters = new Dictionary<string, object>
            {
                { "@MaGiaoVien", request.MaGiaoVien },
                { "@HoTenGiaoVien", request.HoTenGiaoVien },
                { "@HocVi", request.HocVi },
                { "@DienThoai", request.DienThoai },
                { "@Email", request.Email },
                { "@MoTaDinhHuong", request.MoTaDinhHuong },
                { "@SoLuong", request.SoLuong },
            };
            try
            {
                List<Dictionary<string, object>> results = SQLHelper.ExecuteStoredProcedure(storedProcedureName, parameters);
                return Ok(new
                {
                    Message="ĐĂNG KÝ THÔNG TIN THÀNH CÔNG CHO GIẢNG VIÊN",
                    StatusCode=200
                });

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
        [Route("lay-dinh-huong-va-so-luong")]
        public async Task<IActionResult> GetDinhHuongVaSoLuong([FromQuery]string maGV)
        {
            string storedProcedureName = "[Tracking].[PSP_230711_GiaoVienHuongDan_LayDinhHuongVaSoLuong]";
            var parameters = new Dictionary<string, object>
            {
                {"@MaGiaoVien",maGV }
            };
            List<Dictionary<string, object>> results = SQLHelper.ExecuteStoredProcedure(storedProcedureName, parameters);

            if (results.Count > 0)
            {
                var formattedResults = results.Select(r => new
                {
                    moTaDinhHuong = r["MoTaDinhHuong"].ToString(),
                    soLuongConLai = Convert.ToInt32(r["SoLuongConLai"])
                });

                return Json(formattedResults);
            }
            else
            {
                return NotFound();
            }
        }

    }

}
