using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using teacher_rating.Common.Models;
using teacher_rating.Models;
using teacher_rating.Models.Identity;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Services;

public interface ITeacherService
{
    Task<XLWorkbook> GetAccessTeacherExcelFile(string schoolId, int month, int year, string userId);
    Task<bool> CheckTeacherOfGrade(Teacher teacher, string gradeId, int month, int year, string schoolId);
    Task<List<TeachersOfGradeOfGroup>> GetTeachersOfGradeOfGroup(List<GradeConfiguration> grades, List<Teacher> teachers, List<TeacherGroup> teacherGroups, int month, int year);
    Task<RespondApi<string>> CreateTeachersFromExcel(Stream stream);
}

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
    private readonly IGradeConfigurationRepository _gradeConfigurationRepository;
    private readonly ISelfCriticismRepository _selfCriticismRepository;
    private readonly ITeacherGroupRepository _teacherGroupRepository;
    private string _schoolId;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public TeacherService(ITeacherRepository teacherRepository, IHttpContextAccessor httpContext,
        IAssessmentCriteriaRepository assessmentCriteriaRepository,
        IGradeConfigurationRepository gradeConfigurationRepository, ISelfCriticismRepository selfCriticismRepository, ITeacherGroupRepository teacherGroupRepository, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _teacherRepository = teacherRepository;
        _assessmentCriteriaRepository = assessmentCriteriaRepository;
        _gradeConfigurationRepository = gradeConfigurationRepository;
        _selfCriticismRepository = selfCriticismRepository;
        _teacherGroupRepository = teacherGroupRepository;
        _roleManager = roleManager;
        _userManager = userManager;
        _schoolId = httpContext.HttpContext?.User.FindFirst("SchoolId") != null ? 
            httpContext.HttpContext?.User.FindFirst("SchoolId").Value : "";
    }

    public async Task<XLWorkbook> GetAccessTeacherExcelFile(string schoolId, int month, int year, string userId)
    {
        XLWorkbook workbook = new XLWorkbook();
        var workSheet = workbook.Worksheets.Add();
        int row = 1;
        int col = 3;
        var teachers = await _teacherRepository.GetAllTeachers();
        teachers = teachers.Where(x => x.SchoolId == schoolId && Guid.TryParse(x.GroupId, out Guid parsedGuid)).ToList();
        var teacherGroups = await _teacherGroupRepository.GetAllTeacherGroups();
        workSheet.Cell(row, col++).Value = "Tổng cộng";
        workSheet.Cell(row, col++).Value = teachers.Count().ToString();
        var grades = await _gradeConfigurationRepository.GetAllGradeConfigurations();
        var teacherOfGroupOfGrades = await GetTeachersOfGradeOfGroup(grades, teachers, teacherGroups.ToList(), month, year);
        var allTeacherOfGrade = teacherOfGroupOfGrades.SelectMany(x => x.TeachersOfGrade).ToList();
        foreach (var grade in grades)
        {
            var teachersOfGrade = allTeacherOfGrade.Where(x => x.Grade.Id == grade.Id).SelectMany(e => e.Teachers).ToList();
            workSheet.Cell(row, col).Value = grade.Name;
            workSheet.Cell(row + 1, col).Value = teachersOfGrade.Count().ToString();
            col = col + 1;
        }

        col = 3;
        row = row + 5;
        workSheet.Cell(row, col++).Value = "Tổ";
        workSheet.Cell(row, col++).Value = "Số lượng";
        foreach (var grade in grades)
        {
            workSheet.Cell(row, col++).Value = grade.Name;
        }

        col = 3;
        row = row + 1;
        foreach (var teacherOfGroupOfGrade in teacherOfGroupOfGrades)
        {
            workSheet.Cell(row, col++).Value = teacherOfGroupOfGrade.Group.Name;
            var count = teacherOfGroupOfGrade.TeachersOfGrade.Sum(x => x.Teachers.Count);
            workSheet.Cell(row, col++).Value = count.ToString();
            if (count > 0)
            {
                foreach (var teacherOfGrade in teacherOfGroupOfGrade.TeachersOfGrade)
                {
                    workSheet.Cell(row, col).Value = teacherOfGrade.Teachers.Count;
                    workSheet.Cell(row + 1, col++).Value = (teacherOfGrade.Teachers.Count * 100/ count).ToString() + "%" ;
                }
            }
            row = row + 2;
            col = 3;
        }
        workSheet.Cell(row, col++).Value = "Tổng:";
        workSheet.Cell(row, col++).Value = teachers.Count().ToString();
        foreach (var grade in grades)
        {
            var teachersOfGrade = allTeacherOfGrade.Where(x => x.Grade.Id == grade.Id).SelectMany(e => e.Teachers).ToList();
            workSheet.Cell(row, col++).Value = teachersOfGrade.Count().ToString();
        }
        workSheet.Columns().AdjustToContents();
        workSheet.Range(workSheet.Cell(6, 3), workSheet.Cell(row, col-1)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(6, 3), workSheet.Cell(row, col-1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        return workbook;
    }

    public async Task<bool> CheckTeacherOfGrade(Teacher teacher, string gradeId, int month, int year, string schoolId)
    {
        var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(teacher.Id, month, year);
        var total = (int)selfCriticisms.Sum(x => x.TotalScore);
        var gradeOfTeacher = await _gradeConfigurationRepository.GetGradeConfigurationByScore(total, schoolId);
        if (gradeOfTeacher.Id == gradeId)
        {
            return true;
        }

        return false;
    }

    public async Task<List<TeachersOfGradeOfGroup>> GetTeachersOfGradeOfGroup(List<GradeConfiguration> grades, List<Teacher> teachers, List<TeacherGroup> teacherGroups, int month, int year)
    {
        var result = new List<TeachersOfGradeOfGroup>();
        foreach (var group in teacherGroups)
        {
            var teachersOfGroup = teachers.Where(x => x.GroupId == group.Id).ToList();
            var teachersOfGrades = new List<TeachersOfGrade>();
            foreach (var grade in grades)
            {
                var newTeacherGrade = new TeachersOfGrade
                {
                    Grade = grade,
                    Teachers =new List<Teacher>()
                };
                foreach (var teacher in teachersOfGroup)
                {
                    var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(teacher.Id, month, year);
                    var total = (int)selfCriticisms.Sum(x => x.TotalScore);
                    var gradeOfTeacher = await _gradeConfigurationRepository.GetGradeConfigurationByScore(total, teacher.SchoolId);
                    if (gradeOfTeacher.Id == grade.Id)
                    {
                        newTeacherGrade.Teachers.Add(teacher);
                    }
                }
                teachersOfGrades.Add(newTeacherGrade);
            }
            result.Add(new TeachersOfGradeOfGroup()
            {
                Group = group,
                TeachersOfGrade = teachersOfGrades
            });
        }

        return result;
    }

    public async Task<RespondApi<string>> CreateTeachersFromExcel(Stream stream)
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        using (var package = new ExcelPackage(stream))
        {
            var workSheet = package.Workbook.Worksheets[0];
            var totalRows = workSheet.Dimension.Rows;
            var totalCols = workSheet.Dimension.Columns;
            var teachers = new List<Teacher>();
            int row = 1;
            while (workSheet.Cells[row, 1].Value == null || !workSheet.Cells[row, 1].Value.ToString().Contains("STT"))
            {
                row++;
            }

            row++;
            while(workSheet.Cells[row, 1].Value != null && !string.IsNullOrEmpty(workSheet.Cells[row, 1].Value.ToString()))
            {
                var teacher = new Teacher();
                teacher.Id = Guid.NewGuid().ToString();
                teacher.Name = workSheet.Cells[row, 2].Value != null ? workSheet.Cells[row, 2].Value.ToString() : "";
                teacher.CMND = workSheet.Cells[row, 10].Value != null ? workSheet.Cells[row, 10].Value.ToString() : "";
                teacher.Gender = workSheet.Cells[row, 4].Value != null ? workSheet.Cells[row, 4].Value.ToString() : "";
                teacher.PhoneNumber = workSheet.Cells[row, 5].Value != null ? workSheet.Cells[row, 5].Value.ToString() : "";
                teacher.Email = workSheet.Cells[row, 9].Value != null ? workSheet.Cells[row, 9].Value.ToString() : "";
                teacher.SchoolId = _schoolId;
                var oldTeacher = await _teacherRepository.GetTeacherByCMND(teacher.CMND);
                if (oldTeacher != null)
                {
                    teacher.Id = oldTeacher.Id;
                    await _teacherRepository.UpdateTeacher(teacher);
                }
                else
                {
                    await _teacherRepository.AddTeacher(teacher);
                    var user = new ApplicationUser()
                    {
                        UserName = teacher.PhoneNumber,
                        Email = teacher.Email,
                        PhoneNumber = teacher.PhoneNumber,
                        DisplayName = teacher.Name,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        ActiveCode = Guid.NewGuid().ToString(),
                        IsDeleted = false,
                        SchoolId = teacher.SchoolId,
                    };

                    var result = await _userManager.CreateAsync(user, "123456aA@");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Teacher");
                    }
                }
                row++;
            }

            return new RespondApi<string>()
            {
                Result = ResultRespond.Success, Message = "Thành công"
            };
        }
    }
}