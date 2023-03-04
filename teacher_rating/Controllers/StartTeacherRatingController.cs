using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using teacher_rating.Models;
using teacher_rating.Models.Identity;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StartTeacherRatingController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ISelfCriticismRepository _selfCriticismRepository;
        private readonly IAssessmentCriteriaGroupRepository _assessmentCriteriaGroupRepository;
        private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;

        public StartTeacherRatingController(
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            ISelfCriticismRepository selfCriticismRepository,
            IAssessmentCriteriaGroupRepository assessmentCriteriaGroupRepository,
            IAssessmentCriteriaRepository assessmentCriteriaRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _selfCriticismRepository = selfCriticismRepository;
            _assessmentCriteriaGroupRepository = assessmentCriteriaGroupRepository;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
        }

        [HttpPost]
        [Route("create-default")]
        public async Task<IActionResult> CreateDefault()
        {
            try
            {
                var assessmentCriteriaGroups =
                    await _assessmentCriteriaGroupRepository.GetAllAssessmentCriteriaGroups();
                if (!assessmentCriteriaGroups.Any())
                {
                    var criteriaGroups = new List<AssessmentCriteriaGroup>()
                    {
                        new()
                        {
                            Id = "9f861ecb-779c-4f99-986b-60504df316e4",
                            Name = "Thực hiện ngày giờ công, ý thức lao động",
                        },
                        new()
                        {
                            Id = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            Name = "Thực hiện các hồ sơ sổ sách, giấy tờ, báo cáo, kế hoạch",
                        },
                        new()
                        {
                            Id = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            Name = "Thực hiện chuyên môn, nghiệp vụ theo phân công nhiệm vụ",
                        },
                        new()
                        {
                            Id = "112bb16e-0e37-40e1-a30e-805e928b6b50",
                            Name =
                                "Thực hiện công tác chủ nhiệm, HĐ ngoài giờ lên lớp, trải nghiệm; quản sinh, trực giám thị, quản sinh",
                        },
                        new()
                        {
                            Id = "175e324a-93e9-4b98-b7ec-57400667439b",
                            Name = "Tham gia hoạt động đoàn thể, CSVC",
                        },
                        new()
                        {
                            Id = "e800e9e2-f5b8-4e7a-b6a3-69f89006f6a7",
                            Name = "Thực hiện ngày giờ công, ý thức lao động",
                        },
                        new()
                        {
                            Id = "5bd81480-c41e-43b2-b985-370186434728",
                            Name = "Hồ sơ sổ sách",
                        },
                        new()
                        {
                            Id = "60536031-d330-4d1f-9057-4cabf6ad3416",
                            Name = "Công tác khác",
                        },
                    };


                    await _assessmentCriteriaGroupRepository.AddAssessmentCriteriaGroups(criteriaGroups);
                }
                
                var assessmentCriteriases = await _assessmentCriteriaRepository.GetAllAssessmentCriters();

                if (!assessmentCriteriases.Any())
                {
                    var allCriteriaGroups =
                        new List<AssessmentCriteria>();
                    var criteriasMinus1 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Nghỉ do điều động thực hiện nhiệm vụ khác. Nghỉ đúng theo chế độ quy định của luật lao động",
                            Value = 0,
                            DeductScore = 0,
                            Unit = "",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Nghỉ không phép (công việc, dạy học, hoạt động, hội họp, trực...)",
                            Value = 40,
                            DeductScore = 40,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Đi muộn, nghỉ sớm không phép (công việc, dạy học, hoạt động, hội họp, trực...)",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sử dụng điện thoại không phục vụ hoạt động dạy học và công việc của nhà trường trong giờ dạy",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Đổi giờ không xin phép",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Nghỉ dạy có phép",
                            Value = 1,
                            DeductScore = 1,
                            Unit = "Tiết",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Nghỉ hội họp, trực, coi thi, hoạt động của nhà trường, đoàn thể, tổ có phép",
                            Value = 3,
                            DeductScore = 3,
                            Unit = "Buổi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Đi muộn, nghỉ sớm có phép ≤ 10 phút (công việc, dạy học, hoạt động, hội họp, trực...) ",
                            Value = 0.5,
                            DeductScore = 0.5,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Đi muộn, nghỉ sớm có phép > 10 phút (công việc, dạy học, hoạt động, hội họp, trực...)",
                            Value = 1,
                            DeductScore = 1,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Bị phê bình nhắc nhở trong cuộc họp, các hoạt động",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Trang phục không lịch sự, không phù hợp nghề nghiệp, công việc",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Thiếu phù hiệu",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Thiếu ý thức trách nhiệm trong công việc, hoạt động, nhiệm vụ được giao. Sử dụng điện thoại di động, làm việc riêng hoạt động tập thể khi chưa được sự đồng ý của người phụ trách",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Hút thuốc trong nhà trường; còn ảnh hưởng của rượu, bia khi lên lớp và hội họp",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Quản sinh, Giám thị trực không làm đúng nhiệm vụ được giao",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4"
                        },
                    };

                    var criteriasMinus2 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Không có đủ hồ sơ sổ sách, không cập nhật KHGD, KHBD theo quy định",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Thiếu nội dung, nộp chậm, cập nhật chậm, ghi sai, xếp loại C",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "Hồ sơ sổ sách",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sai điểm học bạ, sổ gọi tên ghi điểm không sửa ",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sai điểm học bạ, sổ gọi tên ghi điểm sửa không đúng quy chế",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sai điểm học bạ sửa đúng quy chế",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sai sổ gọi tên ghi điểm sửa đúng quy chế",
                            Value = 1,
                            DeductScore = 1,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4"
                        },
                    };
                    var criteriasPlus2 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Hồ sơ loại xếp loại A",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4"
                        },
                    };
                    
                    var criteriasMinus3 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Vi phạm quy chế chuyên môn bị lập biên bản",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Lần",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ra đề, phản biện đề HSG, đề kiểm tra chung có lỗi văn bản",
                            Value = 2,
                            DeductScore = 2,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ra đề, phản biện đề HSG, đề kiểm tra chung có lỗi kiến thức",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Coi thi HSG, Kiểm tra chung không nghiêm túc",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Chấm HSG, kiểm tra có sai sót",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Kiểm tra hồ sơ, thanh tra chuyên môn không sát thực, bao che, chủ quan cảm tính",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lỗi",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Không tham gia hoạt động của tổ chuyên môn",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "HĐ",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Quản lý giờ học thiếu trách nhiệm, bỏ dạy để học sinh ngồi chơi",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Tiết",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                    };
                    
                     var criteriasPlus3 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ra đề HSG, Kiểm tra chung",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Đề",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Phản biện đề HSG, Kiểm tra chung",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Đề",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Chấm kiểm tra chung trắc nghiệm",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Khối",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Kiểm tra chung tự luận",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lần",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Chấm HSG",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Lần",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Kiểm tra hồ sơ",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Lần",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Dạy thay do tổ trưởng phân công",
                            Value = 3,
                            DeductScore = 3,
                            Unit = "Tiết",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ôn HSG được tổ trưởng xác nhận",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Buổi",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG Quốc gia",
                            Value = 60,
                            DeductScore = 60,
                            Unit = "",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG thành phố giải nhất",
                            Value = 30,
                            DeductScore = 30,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG thành phố giải nhì",
                            Value = 25,
                            DeductScore = 25,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG thành phố giải ba",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG thành phố khuyến khích",
                            Value = 15,
                            DeductScore = 15,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "GVG trường",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "GVG cụm",
                            Value = 25,
                            DeductScore = 25,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "GVG TP",
                            Value = 30,
                            DeductScore = 30,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "SK A trường",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "SK A TP",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Giải",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469"
                        },
                    };
                    allCriteriaGroups.AddRange(criteriasMinus1);
                    allCriteriaGroups.AddRange(criteriasMinus2);
                    allCriteriaGroups.AddRange(criteriasPlus2);
                    allCriteriaGroups.AddRange(criteriasMinus3);
                    allCriteriaGroups.AddRange(criteriasPlus3);
                    await _assessmentCriteriaRepository.AddAssessmentCriterList(allCriteriaGroups);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return Ok();
            }
        }
    }
}