using System.Security.Claims;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using teacher_rating.Common.Const;
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
        private readonly IGradeConfigurationRepository _configurationRepository;
        private readonly ISchoolRepository _schoolRepository;

        public StartTeacherRatingController(
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            ISelfCriticismRepository selfCriticismRepository,
            IAssessmentCriteriaGroupRepository assessmentCriteriaGroupRepository,
            IAssessmentCriteriaRepository assessmentCriteriaRepository,
            IGradeConfigurationRepository configurationRepository, ISchoolRepository schoolRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _selfCriticismRepository = selfCriticismRepository;
            _assessmentCriteriaGroupRepository = assessmentCriteriaGroupRepository;
            _assessmentCriteriaRepository = assessmentCriteriaRepository;
            _configurationRepository = configurationRepository;
            _schoolRepository = schoolRepository;
        }

        [HttpPost]
        [Route("create-default")]
        public async Task<IActionResult> CreateDefault()
        {
            try
            {
                // create default school
                var schools = await _schoolRepository.GetAll();
                if (!schools.Any())
                {
                    var school = new School
                    {
                        Id = DefaultConfigs.DefaultSchoolId,
                        Name = "Tr?????ng THPT Tr??n Nguy??n H??n",
                        Address =
                            "Ng?? 185 T??n ?????c Th???ng, Ph?????ng An D????ng, Qu???n L?? Ch??n, H???i Ph??ng",
                        IsDeleted = false,
                        Teachers = new List<Teacher>(),
                        AssessmentGroups = new List<AssessmentGroup>()
                    };

                    // add school to db
                    await _schoolRepository.Create(school);
                }

                var assessmentCriteriaGroups =
                    await _assessmentCriteriaGroupRepository.GetAllAssessmentCriteriaGroups();

                if (!assessmentCriteriaGroups.Any())
                {
                    var criteriaGroups = new List<AssessmentCriteriaGroup>()
                    {
                        new()
                        {
                            Id = "9f861ecb-779c-4f99-986b-60504df316e4",
                            Name = "Th???c hi???n ng??y gi??? c??ng, ?? th???c lao ?????ng",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            Name = "Th???c hi???n c??c h??? s?? s??? s??ch, gi???y t???, b??o c??o, k??? ho???ch",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            Name = "Th???c hi???n chuy??n m??n, nghi???p v??? theo ph??n c??ng nhi???m v???",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = "112bb16e-0e37-40e1-a30e-805e928b6b50",
                            Name =
                                "Th???c hi???n c??ng t??c ch??? nhi???m, H?? ngo??i gi??? l??n l???p, tr???i nghi???m; qu???n sinh, tr???c gi??m th???, qu???n sinh",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = "175e324a-93e9-4b98-b7ec-57400667439b",
                            Name = "Tham gia ho???t ?????ng ??o??n th???, CSVC",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = "e800e9e2-f5b8-4e7a-b6a3-69f89006f6a7",
                            Name = "Th???c hi???n ng??y gi??? c??ng, ?? th???c lao ?????ng",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = "5bd81480-c41e-43b2-b985-370186434728",
                            Name = "H??? s?? s??? s??ch",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = "60536031-d330-4d1f-9057-4cabf6ad3416",
                            Name = "C??ng t??c kh??c",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
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
                                "Ngh??? do ??i???u ?????ng th???c hi???n nhi???m v??? kh??c. Ngh??? ????ng theo ch??? ????? quy ?????nh c???a lu???t lao ?????ng",
                            Value = 0,
                            DeductScore = 0,
                            Unit = "",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ngh??? kh??ng ph??p (c??ng vi???c, d???y h???c, ho???t ?????ng, h???i h???p, tr???c...)",
                            Value = 40,
                            DeductScore = 40,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "??i mu???n, ngh??? s???m kh??ng ph??p (c??ng vi???c, d???y h???c, ho???t ?????ng, h???i h???p, tr???c...)",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "S??? d???ng ??i???n tho???i kh??ng ph???c v??? ho???t ?????ng d???y h???c v?? c??ng vi???c c???a nh?? tr?????ng trong gi??? d???y",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "?????i gi??? kh??ng xin ph??p",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ngh??? d???y c?? ph??p",
                            Value = 1,
                            DeductScore = 1,
                            Unit = "Ti???t",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ngh??? h???i h???p, tr???c, coi thi, ho???t ?????ng c???a nh?? tr?????ng, ??o??n th???, t??? c?? ph??p",
                            Value = 3,
                            DeductScore = 3,
                            Unit = "Bu???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "??i mu???n, ngh??? s???m c?? ph??p ??? 10 ph??t (c??ng vi???c, d???y h???c, ho???t ?????ng, h???i h???p, tr???c...) ",
                            Value = 0.5,
                            DeductScore = 0.5,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "??i mu???n, ngh??? s???m c?? ph??p > 10 ph??t (c??ng vi???c, d???y h???c, ho???t ?????ng, h???i h???p, tr???c...)",
                            Value = 1,
                            DeductScore = 1,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "B??? ph?? b??nh nh???c nh??? trong cu???c h???p, c??c ho???t ?????ng",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Trang ph???c kh??ng l???ch s???, kh??ng ph?? h???p ngh??? nghi???p, c??ng vi???c",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Thi???u ph?? hi???u",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Thi???u ?? th???c tr??ch nhi???m trong c??ng vi???c, ho???t ?????ng, nhi???m v??? ???????c giao. S??? d???ng ??i???n tho???i di ?????ng, l??m vi???c ri??ng ho???t ?????ng t???p th??? khi ch??a ???????c s??? ?????ng ?? c???a ng?????i ph??? tr??ch",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "H??t thu???c trong nh?? tr?????ng; c??n ???nh h?????ng c???a r?????u, bia khi l??n l???p v?? h???i h???p",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Qu???n sinh, Gi??m th??? tr???c kh??ng l??m ????ng nhi???m v??? ???????c giao",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "9f861ecb-779c-4f99-986b-60504df316e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                    };

                    var criteriasMinus2 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Kh??ng c?? ????? h??? s?? s??? s??ch, kh??ng c???p nh???t KHGD, KHBD theo quy ?????nh",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Thi???u n???i dung, n???p ch???m, c???p nh???t ch???m, ghi sai, x???p lo???i C",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "H??? s?? s??? s??ch",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sai ??i???m h???c b???, s??? g???i t??n ghi ??i???m kh??ng s???a ",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sai ??i???m h???c b???, s??? g???i t??n ghi ??i???m s???a kh??ng ????ng quy ch???",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sai ??i???m h???c b??? s???a ????ng quy ch???",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Sai s??? g???i t??n ghi ??i???m s???a ????ng quy ch???",
                            Value = 1,
                            DeductScore = 1,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                    };
                    var criteriasPlus2 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "H??? s?? lo???i x???p lo???i A",
                            Value = 5,
                            DeductScore = 5,
                            Unit = "",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "46e5e5c4-3590-4575-a3e2-b5139a0998e4",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                    };

                    var criteriasMinus3 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Vi ph???m quy ch??? chuy??n m??n b??? l???p bi??n b???n",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "L???n",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ra ?????, ph???n bi???n ????? HSG, ????? ki???m tra chung c?? l???i v??n b???n",
                            Value = 2,
                            DeductScore = 2,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ra ?????, ph???n bi???n ????? HSG, ????? ki???m tra chung c?? l???i ki???n th???c",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Coi thi HSG, Ki???m tra chung kh??ng nghi??m t??c",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ch???m HSG, ki???m tra c?? sai s??t",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ki???m tra h??? s??, thanh tra chuy??n m??n kh??ng s??t th???c, bao che, ch??? quan c???m t??nh",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???i",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Kh??ng tham gia ho???t ?????ng c???a t??? chuy??n m??n",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "H??",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Qu???n l?? gi??? h???c thi???u tr??ch nhi???m, b??? d???y ????? h???c sinh ng???i ch??i",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Ti???t",
                            IsDeduct = true,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                    };

                    var criteriasPlus3 = new List<AssessmentCriteria>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ra ????? HSG, Ki???m tra chung",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "?????",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ph???n bi???n ????? HSG, Ki???m tra chung",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "?????",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ch???m ki???m tra chung tr???c nghi???m",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Kh???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ki???m tra chung t??? lu???n",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???n",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ch???m HSG",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "L???n",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "Ki???m tra h??? s??",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "L???n",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "D???y thay do t??? tr?????ng ph??n c??ng",
                            Value = 3,
                            DeductScore = 3,
                            Unit = "Ti???t",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "??n HSG ???????c t??? tr?????ng x??c nh???n",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Bu???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG Qu???c gia",
                            Value = 60,
                            DeductScore = 60,
                            Unit = "",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG th??nh ph??? gi???i nh???t",
                            Value = 30,
                            DeductScore = 30,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG th??nh ph??? gi???i nh??",
                            Value = 25,
                            DeductScore = 25,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG th??nh ph??? gi???i ba",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "HSG th??nh ph??? khuy???n kh??ch",
                            Value = 15,
                            DeductScore = 15,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "GVG tr?????ng",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "GVG c???m",
                            Value = 25,
                            DeductScore = 25,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "GVG TP",
                            Value = 30,
                            DeductScore = 30,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "SK A tr?????ng",
                            Value = 10,
                            DeductScore = 10,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                        new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name =
                                "SK A TP",
                            Value = 20,
                            DeductScore = 20,
                            Unit = "Gi???i",
                            IsDeduct = false,
                            AssessmentCriteriaGroupId = "e74719f0-1477-4e39-bc07-618b5f3cc469",
                            SchoolId = DefaultConfigs.DefaultSchoolId,
                        },
                    };
                    allCriteriaGroups.AddRange(criteriasMinus1);
                    allCriteriaGroups.AddRange(criteriasMinus2);
                    allCriteriaGroups.AddRange(criteriasPlus2);
                    allCriteriaGroups.AddRange(criteriasMinus3);
                    allCriteriaGroups.AddRange(criteriasPlus3);
                    await _assessmentCriteriaRepository.AddAssessmentCriterList(allCriteriaGroups);
                }

                var roles = _roleManager.Roles.ToList();

                if (roles.Count <= 0)
                {
                    // add roles default for system - application role

                    var roleAdmin = new ApplicationRole()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Admin",
                        NormalizedName = "Admin".ToUpper(),
                        Version = 1,
                        DisplayName = "Admin",
                        CanDelete = false,
                        Claims = new List<MongoClaim>()
                        {
                            new MongoClaim()
                            {
                                Type = "Permission",
                                Value = "Admin",
                            },
                            new MongoClaim()
                            {
                                Type = "Permission",
                                Value = "Teacher",
                            }
                        }
                    };

                    // role teacher
                    var roleTeacher = new ApplicationRole()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Teacher",
                        NormalizedName = "Teacher".ToUpper(),
                        Version = 1,
                        DisplayName = "Teacher",
                        CanDelete = false,
                        Claims = new List<MongoClaim>()
                        {
                            new MongoClaim()
                            {
                                Type = "Permission",
                                Value = "Teacher",
                            }
                        }
                    };

                    // add role to db
                    await _roleManager.CreateAsync(roleAdmin);
                    await _roleManager.CreateAsync(roleTeacher);
                }

                var users = _userManager.Users.ToList();
                if (users.Count <= 0)
                {
                    // create user admin
                    var userAdmin = new ApplicationUser()
                    {
                        Id = Guid.NewGuid(),
                        UserName = "admin",
                        NormalizedUserName = "admin".ToUpper(),
                        Email = "admin@gmail.com",
                        DisplayName = "Qu???n tr??? h??? th???ng",
                        NormalizedEmail = "",
                        EmailConfirmed = true,
                        PhoneNumber = "0123456789",
                        PhoneNumberConfirmed = true,
                        IsActive = true,
                        SchoolId = DefaultConfigs.DefaultSchoolId,
                    };
                    var result = await _userManager.CreateAsync(userAdmin, "123123aA@");
                    if (result.Succeeded)
                    {
                        // add role admin for user admin
                        await _userManager.AddToRoleAsync(userAdmin, "Admin");

                        // add role teacher for user admin
                        await _userManager.AddToRoleAsync(userAdmin, "Teacher");

                        // add claim for user admin
                        await _userManager.AddClaimAsync(userAdmin, new Claim("Permission", "Admin"));
                        await _userManager.AddClaimAsync(userAdmin, new Claim("Permission", "Teacher"));
                    }
                }


                var grades = await _configurationRepository.GetAllGradeConfigurations();
                if (!grades.Any())
                {
                    // create grade default

                    var grade1 = new GradeConfiguration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Xu???t s???c",
                        Description =
                            "Ho??n th??nh xu???t s???c nhi???m v???: T??? 130 ??i???m tr??? l??n. (C??? n??m: C?? ??t nh???t 01 c??ng tr??nh khoa h???c, ????? ??n, ????? t??i ho???c s??ng ki???n ???????c ??p d???ng v?? mang l???i hi???u qu??? trong vi???c th???c hi???n c??ng t??c chuy??n m??n, ngh??? nghi???p ???????c nh?? tr?????ng c??ng nh???n)",
                        MinimumScore = 130,
                        MaximumScore = 99999,
                        SchoolId = DefaultConfigs.DefaultSchoolId,
                    };

                    var grade2 = new GradeConfiguration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "T???t",
                        Description = "Ho??n th??nh t???t nhi???m v???: T??? 110 ??i???m ?????n d?????i 130 ??i???m",
                        MinimumScore = 110,
                        MaximumScore = 129,
                        SchoolId = DefaultConfigs.DefaultSchoolId,
                    };

                    var grade3 = new GradeConfiguration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Kh??",
                        Description = "Ho??n th??nh kh?? nhi???m v???: T??? 90 ??i???m ?????n d?????i 110 ??i???m",
                        MinimumScore = 90,
                        MaximumScore = 109,
                        SchoolId = DefaultConfigs.DefaultSchoolId,
                    };

                    var grade4 = new GradeConfiguration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Kh??ng ho??n th??nh",
                        Description = "Kh??ng ho??n th??nh nhi???m v???: D?????i 90 ??i???m.",
                        MinimumScore = 0,
                        MaximumScore = 89,
                        SchoolId = DefaultConfigs.DefaultSchoolId,
                    };

                    // add grade to db
                    await _configurationRepository.AddGradeConfiguration(grade1);
                    await _configurationRepository.AddGradeConfiguration(grade2);
                    await _configurationRepository.AddGradeConfiguration(grade3);
                    await _configurationRepository.AddGradeConfiguration(grade4);
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