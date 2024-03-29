﻿using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver.Linq;
using ServiceStack;
using teacher_rating.Models;
using teacher_rating.Models.Identity;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

namespace teacher_rating.Mongodb.Services;

public interface ISelfCriticismService
{
    Task<XLWorkbook> GetSelfCriticismExcelFile(string schoolId, int month, int year, string userId, List<string> groupIds);
    Task<XLWorkbook> GetSelfCriticismExcelFileNew(string schoolId, int month, int year, string userId, List<string> groupIds);
    Task<XLWorkbook> GetSampleExcelFile(int year);
    Task<XLWorkbook> GetCompetitionBoard(GetCompetitionBoardRequest model);
    Task<XLWorkbook> GetPersonalArchievement(GetPersonalArchievementRequest model);
}

public class SelfCriticismService : ISelfCriticismService
{
    private readonly ISelfCriticismRepository _selfCriticismRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IGradeConfigurationRepository _gradeConfigurationRepository;
    private readonly ITeacherGroupRepository _teacherGroupRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IAssessmentCriteriaRepository _assessmentCriteria;
    private readonly IAssessmentCriteriaGroupRepository _assessmentCriteriaGroup;
    private UserManager<ApplicationUser> userManager;
    private RoleManager<ApplicationRole> roleManager;
    private List<string>? _roles;

    public SelfCriticismService(ISelfCriticismRepository selfCriticismRepository, ITeacherRepository teacherRepository, IGradeConfigurationRepository gradeConfigurationRepository
    ,IHttpContextAccessor httpContext, ITeacherGroupRepository teacherGroupRepository, ISchoolRepository schoolRepository, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IAssessmentCriteriaRepository assessmentCriteria, IAssessmentCriteriaGroupRepository assessmentCriteriaGroup)
    {
        _selfCriticismRepository = selfCriticismRepository;
        _teacherRepository = teacherRepository;
        _gradeConfigurationRepository = gradeConfigurationRepository;
        _teacherGroupRepository = teacherGroupRepository;
        _schoolRepository = schoolRepository;
        this.roleManager = roleManager;
        this.userManager = userManager;
        _assessmentCriteria = assessmentCriteria;
        _assessmentCriteriaGroup = assessmentCriteriaGroup;
        _roles = httpContext?.HttpContext?.User?.FindAll(ClaimTypes.Role) != null ?
            httpContext?.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(x => x.Value).ToList() : null;
    }
    public async Task<bool> HasClaim(string userId, string Value)
    {
        var user = await userManager.FindByIdAsync(userId);
        IEnumerable<string> applicationRoles = await userManager.GetRolesAsync(user);
        foreach(string applicationRoleName in applicationRoles)
        {
            ApplicationRole applicationRoleData = await roleManager.FindByNameAsync(applicationRoleName);
            IEnumerable<Claim> claims = await roleManager.GetClaimsAsync(applicationRoleData);
            if (claims != null && claims.Select(e => e.Value).Contains(Value))
                return true;
        }
        return false;
    }

    public async Task<XLWorkbook> CreateSheet(XLWorkbook workbook, List<Teacher> teachers, int month, int year, string schoolId, TeacherGroup group = null )
    {
        var workSheet = workbook.Worksheets.Add();
        workSheet.Name = group == null ? "Все" : group.Name;
        var row = 7;
        var col = 1;
        string titleOfSheet = $"KẾT QUẢ THI ĐUA THÁNG {month}/{year}" + (group == null ? "" : $" - {group.Name}");
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 10)).Merge().Value = titleOfSheet;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 10)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 10)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 10)).Merge().Style.Font.FontSize = 25;
        
        var Titles = new[]
        {
            "STT",
            "Họ và tên",
            "Tổng điểm",
            "Xếp loại",
        };
        foreach (var title in Titles)
        {
            workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row + 1, col++)).Merge().Value = title;
            workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row + 1, col)).Merge().Style.Font.Bold = true;
            workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row + 1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row + 1, col)).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row, col + 2)).Merge().Value = "Điểm cộng";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row, col + 2)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row, col + 2)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(row, col + 3), workSheet.Cell(row, col + 5)).Merge().Value = "Điểm trừ";
        workSheet.Range(workSheet.Cell(row, col + 3), workSheet.Cell(row, col + 5)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col + 3), workSheet.Cell(row, col + 5)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row + 1, col).Value = "Số điểm";
        workSheet.Cell(row + 1, col).Style.Font.Bold = true;
        workSheet.Cell(row + 1, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row + 1, col + 1).Value = "Lí do";
        workSheet.Cell(row + 1, col + 1).Style.Font.Bold = true;
        workSheet.Cell(row + 1, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row + 1, col + 2).Value = "Ghi chú";
        workSheet.Cell(row + 1, col + 2).Style.Font.Bold = true;
        workSheet.Cell(row + 1, col + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row + 1, col + 3).Value = "Số điểm";
        workSheet.Cell(row + 1, col + 3).Style.Font.Bold = true;
        workSheet.Cell(row + 1, col + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row + 1, col + 4).Value = "Lí do";
        workSheet.Cell(row + 1, col + 4).Style.Font.Bold = true;
        workSheet.Cell(row + 1, col + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row + 1, col + 5).Value = "Ghi chú";
        workSheet.Cell(row + 1, col + 5).Style.Font.Bold = true;
        workSheet.Cell(row + 1, col + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(row, 1), workSheet.Cell(row + 1, col + 5)).Style.Border.InsideBorder =
            XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(row, 1), workSheet.Cell(row + 1, col + 5)).Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        row = row + 2;
        col = 1;
        int max;
        max = row;
        
        for (var i = 0 ; i < teachers.Count(); i++)
        {
            col = 1;
            workSheet.Cell(row, col++).Value = (i + 1).ToString();
            workSheet.Cell(row, col++).Value = teachers[i].Name;
            var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(teachers[i].Id, month, year);
            var total = (int)selfCriticisms.Sum(x => x.TotalScore);
            workSheet.Cell(row, col++).Value = total.ToString();
            var grade = await _gradeConfigurationRepository.GetGradeConfigurationByScore(total, schoolId);
            if (grade != null)
            {
                workSheet.Cell(row, col++).Value = grade != null ? grade.Name : "Chưa có xếp loại phù hợp";
            }
            else
            {
                col = col + 1;
            }
        
            var plus = selfCriticisms.SelectMany(x => x.AssessmentCriterias).Where(e => !e.IsDeduct).ToList();
            var sub = selfCriticisms.SelectMany(x => x.AssessmentCriterias).Where(e => e.IsDeduct).ToList();
            var currentRow = row;
            if(plus.Count() > 0 || sub.Count() > 0)
                max = max + (plus.Count() > sub.Count() ? plus.Count() : sub.Count());
            plus.ForEach(x =>
            {
                workSheet.Cell(row++, col).Value = x.Value.ToString();
            });
            workSheet.Cell(max, col).Value = plus.Sum(x => x.Value).ToString();
            workSheet.Cell(max, col).Style.Font.Bold = true;
            row = currentRow;
            plus.ForEach(x =>
            {
                workSheet.Cell(row++, col + 1).Value = x.Name;
            });
            row = currentRow;
            plus.ForEach(x =>
            {
                workSheet.Cell(row++, col + 2).Value = x.Description;
            });
            row = currentRow;
            sub.ForEach(x =>
            {
                workSheet.Cell(row++, col + 3).Value = x.Value.ToString();
            });
            workSheet.Cell(max, col + 3).Value = sub.Sum(x => x.Value).ToString();
            workSheet.Cell(max, col + 3).Style.Font.Bold = true;
            row = currentRow;
            sub.ForEach(x =>
            {
                workSheet.Cell(row++, col + 4).Value += x.Name;
            });
            row = currentRow;
            sub.ForEach(x =>
            {
                workSheet.Cell(row++, col + 5).Value = x.Description;
            });

            workSheet.Range(workSheet.Cell(max, 1), workSheet.Cell(max, col + 5)).Style.Border.BottomBorder =
                XLBorderStyleValues.Thin;
            for (int colS = 1 ; colS <= col + 5; colS++)
            {
                workSheet.Range(workSheet.Cell(currentRow, colS), workSheet.Cell(max, colS)).Style.Border.RightBorder =
                    XLBorderStyleValues.Thin;
            }
            
            row = max + 1;
            max = max + 1;
        }
        
        workSheet.Range(workSheet.Cell(9, 1), workSheet.Cell(max, 1)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        workSheet.Columns().AdjustToContents();
        workSheet.Column("A").Width = 10;
        workSheet.Column("C").Width = 30;
        
        return workbook;
    }
    
    public async Task<XLWorkbook> GetAccessTeacherExcelFile(XLWorkbook workbook, string schoolId, int month, int year, string userId)
    {
        var workSheet = workbook.Worksheets.Add();
        workSheet.Name = "Tổng hợp";
        var school = await _schoolRepository.GetById(schoolId);
        
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 5)).Merge().Value = school != null ? school.Name.ToUpper() : "TRƯỜNG THPT TRẦN NGUYÊN HÃN";
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 5)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 5)).Merge().Style.Font.Underline = XLFontUnderlineValues.Single;
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 5)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        var row = 8;
        var col = 3;
        string titleOfSheet = $"KẾT QUẢ THI ĐUA THÁNG {month}/{year} TOÀN TRƯỜNG";
        workSheet.Range(workSheet.Cell(4, 3), workSheet.Cell(5, 9)).Merge().Value = titleOfSheet;
        workSheet.Range(workSheet.Cell(4, 3), workSheet.Cell(5, 9)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(4, 3), workSheet.Cell(5, 9)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(4, 3), workSheet.Cell(5, 9)).Merge().Style.Font.FontSize = 18;
        var teachers = await _teacherRepository.GetAllTeachers();
        teachers = teachers.Where(x => x.SchoolId == schoolId && Guid.TryParse(x.GroupId, out Guid parsedGuid)).ToList();
        var teacherGroups = await _teacherGroupRepository.GetAllTeacherGroups();
        // workSheet.Cell(row, col++).Value = "Tổng cộng";
        // workSheet.Cell(row, col++).Value = teachers.Count().ToString();
        var grades = await _gradeConfigurationRepository.GetAllGradeConfigurations();
        var teacherOfGroupOfGrades = await GetTeachersOfGradeOfGroup(grades, teachers, teacherGroups.ToList(), month, year);
        var allTeacherOfGrade = teacherOfGroupOfGrades.SelectMany(x => x.TeachersOfGrade).ToList();
        // foreach (var grade in grades)
        // {
        //     var teachersOfGrade = allTeacherOfGrade.Where(x => x.Grade.Id == grade.Id).SelectMany(e => e.Teachers).ToList();
        //     workSheet.Cell(row, col).Value = grade != null ? grade.Name : "Chưa có xếp loại phù hợp";
        //     workSheet.Cell(row + 1, col).Value = teachersOfGrade.Count().ToString();
        //     col = col + 1;
        // }
        //
        // col = 3;
        // row = row + 5;
        workSheet.Cell(row, col).Value = "STT";
        workSheet.Cell(row, col).Style.Font.Bold = true;
        col++;
        workSheet.Cell(row, col).Value = "Tổ";
        workSheet.Cell(row, col).Style.Font.Bold = true;
        col++;
        workSheet.Cell(row, col).Value = "Số lượng";
        workSheet.Cell(row, col).Style.Font.Bold = true;
        col++;
        foreach (var grade in grades)
        {
            workSheet.Cell(row, col).Value = grade != null ? grade.Name : "Chưa có xếp loại phù hợp";
            workSheet.Cell(row, col).Style.Font.Bold = true;
            col++;
        }

        col = 3;
        row++;
        for (var i = 0; i < teacherOfGroupOfGrades.Count; i++ )
        {
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Value = i+1;
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            col++;
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Value = teacherOfGroupOfGrades[i].Group.Name;
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            col++;
            var count = teacherOfGroupOfGrades[i].TeachersOfGrade.Sum(x => x.Teachers.Count);
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Value = count.ToString();
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            col++;
            if (count > 0)
            {
                foreach (var teacherOfGrade in teacherOfGroupOfGrades[i].TeachersOfGrade)
                {
                    workSheet.Cell(row, col).Value = teacherOfGrade.Teachers.Count;
                    workSheet.Cell(row + 1, col++).Value = (teacherOfGrade.Teachers.Count * 100/ count).ToString() + "%" ;
                }
            }
            row = row + 2;
            col = 3;
        }
        workSheet.Cell(row, col++).Value = "Tổng:";
        col++;
        workSheet.Cell(row, col++).Value = teachers.Count().ToString();
        foreach (var grade in grades)
        {
            var teachersOfGrade = allTeacherOfGrade.Where(x => x.Grade.Id == grade.Id).SelectMany(e => e.Teachers).ToList();
            workSheet.Cell(row, col++).Value = teachersOfGrade.Count().ToString();
        }
        workSheet.Columns().AdjustToContents();
        workSheet.Range(workSheet.Cell(8, 3), workSheet.Cell(row, col)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(8, 3), workSheet.Cell(row, col)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        workSheet.RangeUsed().Style.Font.FontName = "Times New Roman";
        workSheet.RangeUsed().Style.Font.FontSize = 12;
        workSheet.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Column("D").Width = 40;
        return workbook;
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
    
    public async Task<XLWorkbook> CreateSheetNew(XLWorkbook workbook, List<Teacher> teachers, int month, int year, string schoolId, TeacherGroup group = null )
    {
        var workSheet = workbook.Worksheets.Add();
        workSheet.Name = group == null ? "Все" : group.Name;
        var school = await _schoolRepository.GetById(schoolId);
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 5)).Merge().Value = school != null ? school.Name.ToUpper() : "TRƯỜNG THPT TRẦN NGUYÊN HÃN";
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 5)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 5)).Merge().Style.Font.Underline = XLFontUnderlineValues.Single;
        var row = 5;
        var col = 1;
        string titleOfSheet = $"KẾT QUẢ THI ĐUA THÁNG {month}/{year}" + (group == null ? "" : $" - {group.Name}");
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 5)).Merge().Value = titleOfSheet;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 5)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 5)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 5)).Merge().Style.Font.FontSize = 12;

        row = 7;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Value = "STT";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col++;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Value = "Họ và tên";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col++;
        workSheet.Cell(row,col).Value= "Điểm cộng";
        workSheet.Cell(row,col).Style.Font.Bold = true;
        workSheet.Cell(row,col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row,col+1).Value= "Điểm trừ";
        workSheet.Cell(row,col+1).Style.Font.Bold = true;
        workSheet.Cell(row,col+1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row+1,col).Value= "Số điểm";
        workSheet.Cell(row+1,col).Style.Font.Bold = true;
        workSheet.Cell(row+1,col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row+1,col+1).Value= "Số điểm";
        workSheet.Cell(row+1,col+1).Style.Font.Bold = true;
        workSheet.Cell(row+1,col+1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col = col + 2;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Value = "Tổng điểm đạt được";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col++;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Value = "Xếp loại";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col++;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Value = "Ghi chú";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col++;
        row = row + 2;
        col = 1;
        
        for (var i = 0 ; i < teachers.Count(); i++)
        {
            col = 1;
            workSheet.Cell(row,col++).Value= (i+1).ToString();
            workSheet.Cell(row,col++).Value= teachers[i].Name;
            var selfCriticism = await _selfCriticismRepository.GetSelfCriticismsByTeacher(teachers[i].Id, month, year);
            var acesses = selfCriticism.SelectMany(x => x.AssessmentCriterias).Distinct().ToList();
            var plus = acesses.Where(x => !x.IsDeduct).Sum(e => e.Value);
            var sub = acesses.Where(x => x.IsDeduct).Sum(e => e.Value);
            var score = selfCriticism.Sum(x => x.TotalScore);
            workSheet.Cell(row,col++).Value= plus.ToString();
            workSheet.Cell(row,col++).Value= sub.ToString();
            workSheet.Cell(row,col++).Value= score.ToString();
            var grade = await  _gradeConfigurationRepository.GetGradeConfigurationByScore((int)score, schoolId);
            workSheet.Cell(row,col++).Value= grade != null ? grade.Name : "Chưa có xếp loại phù hợp";
            row++;
        }

        workSheet.Range(workSheet.Cell(7, 1), workSheet.Cell(row, col)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(7, 1), workSheet.Cell(row, col)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        
        var grades = await _gradeConfigurationRepository.GetAllGradeConfigurations();
        var teacherOfGroupOfGrades = await GetTeachersOfGradeOfGroup(grades, teachers, new List<TeacherGroup>(){group}, month, year);
        var allTeacherOfGrade = teacherOfGroupOfGrades.SelectMany(x => x.TeachersOfGrade).ToList();
        
        col = col + 2;
        var curentCol = col;
        row = 7;
        workSheet.Cell(row, col++).Value = "Tổ";
        workSheet.Cell(row, col++).Value = "Số lượng";
        foreach (var grade in grades)
        {
            workSheet.Cell(row, col).Value = grade != null ? grade.Name : "Chưa có xếp loại phù hợp";
            workSheet.Cell(row, col).Style.Font.Bold = true;
            col++;
        }

        col = curentCol;
        row = row + 1;
        workSheet.Column(curentCol).Width = 40;
        foreach (var teacherOfGroupOfGrade in teacherOfGroupOfGrades)
        {
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Value = teacherOfGroupOfGrade.Group.Name;
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            col++;
            var count = teacherOfGroupOfGrade.TeachersOfGrade.Sum(x => x.Teachers.Count);
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Value = count.ToString();
            workSheet.Range(workSheet.Cell(row, col),workSheet.Cell(row+1, col)).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            col++;
            if (count > 0)
            {
                foreach (var teacherOfGrade in teacherOfGroupOfGrade.TeachersOfGrade)
                {
                    workSheet.Cell(row, col).Value = teacherOfGrade.Teachers.Count;
                    workSheet.Cell(row + 1, col++).Value = (teacherOfGrade.Teachers.Count * 100/ count).ToString() + "%" ;
                }
            }
            row = row + 2;
            col = curentCol;
        }
        workSheet.Cell(row, col++).Value = "Tổng:";
        workSheet.Cell(row, col++).Value = teachers.Count().ToString();
        foreach (var grade in grades)
        {
            var teachersOfGrade = allTeacherOfGrade.Where(x => x.Grade.Id == grade.Id).SelectMany(e => e.Teachers).ToList();
            workSheet.Cell(row, col).Value = teachersOfGrade.Count().ToString();
            col++;
        }
        workSheet.Range(workSheet.Cell(7, curentCol), workSheet.Cell(row, col-1)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(7, curentCol), workSheet.Cell(row, col-1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(7, curentCol), workSheet.Cell(row, col-1)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Columns().AdjustToContents();
        workSheet.Column("E").Width = 30;
        workSheet.Column("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Column(curentCol).Width = 40;
        workSheet.RangeUsed().Style.Font.FontName = "Times New Roman";
        workSheet.RangeUsed().Style.Font.FontSize = 12;
        
        return workbook;
    }
    public async Task<XLWorkbook> GetSelfCriticismExcelFile(string schoolId, int month, int year, string userId, List<string> groupIds)
    {
        XLWorkbook workbook = new XLWorkbook();
        
        var teachers = new List<Teacher>();
        var teacher = await _teacherRepository.GetTeacherByUserId(userId);
        if (teacher != null && !teacher.Name.Contains("admin"))
        {
            teachers.Add(teacher);
            workbook = await CreateSheet(workbook, teachers, month, year, schoolId);
        }
        else 
        {
            teachers = await _teacherRepository.GetAllTeachers();
            var  groups = new List<TeacherGroup>();
            if (groupIds == null || groupIds.Count <= 0)
            {
                groups = await _teacherGroupRepository.GetTeacherGroupsBySchoolId(schoolId);
            }
            else
            {
                groups = await _teacherGroupRepository.GetTeacherGroupsByIds(groupIds, schoolId);
            }

            foreach (var group in groups)
            {
                workbook = await CreateSheet(workbook, teachers.Where(x => x.GroupId == group.Id).ToList(), month, year, schoolId, group);
            }
            workbook = await GetAccessTeacherExcelFile(workbook, schoolId, month, year, userId);
        }
        return workbook;
        //throw new NotImplementedException();
    }

    public async Task<XLWorkbook> CreateSheetOfTeacher(XLWorkbook workbook, string schoolId, Teacher teacher, int month, int year, TeacherGroup group)
    {
        var workSheet = workbook.Worksheets.Add("Cá nhân");
        var school = await _schoolRepository.GetById(schoolId);
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 3)).Merge().Value = school != null ? school.Name.ToUpper() : "TRƯỜNG THPT TRẦN NGUYÊN HÃN";
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 3)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 3)).Merge().Style.Font.Underline = XLFontUnderlineValues.Single;
        var row = 5;
        var col = 1;
        string titleOfSheet = $"KẾT QUẢ THI ĐUA THÁNG {month}/{year}" + (group == null ? "" : $" - {group.Name}");
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 5)).Merge().Value = titleOfSheet;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 5)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 5)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5, 5)).Merge().Style.Font.FontSize = 12;
        string titleOfSheetTeacher = $"Giáo viên: {teacher.Name}";
        workSheet.Range(workSheet.Cell(6, 1), workSheet.Cell(6, 5)).Merge().Value = titleOfSheetTeacher;
        workSheet.Range(workSheet.Cell(6, 1), workSheet.Cell(6, 5)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(6, 1), workSheet.Cell(6, 5)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(6, 1), workSheet.Cell(6, 5)).Merge().Style.Font.FontSize = 12;
        row = 8;
        col = 1;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Value = "STT";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col++;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row, col+1)).Merge().Value = "Điểm cộng/Điểm trừ";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row, col+1)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row, col+1)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        workSheet.Cell(row+1, col).Value = "Nội dung công việc";
        workSheet.Cell(row+1, col).Style.Font.Bold = true;
        workSheet.Cell(row+1, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Cell(row+1, col+1).Value = "Số điểm";
        workSheet.Cell(row+1, col+1).Style.Font.Bold = true;
        workSheet.Cell(row+1, col+1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col = col + 2;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Value = "Tổng điểm";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col++;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Value = "Xếp loại";
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row+1, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        row = row + 2;
        var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(teacher.Id, month, year);
        var accesses = selfCriticisms.SelectMany(x => x.AssessmentCriterias).Distinct().ToList();
        col = 4;
        for(int i = 1 ; i <= accesses.Count; i++)
        {
            col = 1;
            workSheet.Cell(row, col++).Value = i.ToString();
            workSheet.Cell(row, col++).Value = accesses[i-1].Name;
            workSheet.Cell(row, col++).Value = accesses[i-1].IsDeduct ? (-1) * accesses[i-1].Value : accesses[i-1].Value ;
            row++;
        }

        var score = selfCriticisms.Sum(x => x.TotalScore);
        workSheet.Range(workSheet.Cell(row - accesses.Count, col), workSheet.Cell(row, col)).Merge().Value = score;
        workSheet.Range(workSheet.Cell(row - accesses.Count, col), workSheet.Cell(row, col)).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        workSheet.Range(workSheet.Cell(row - accesses.Count, col), workSheet.Cell(row, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        col++;
        var grade = await _gradeConfigurationRepository.GetGradeConfigurationByScore((int)score, schoolId);
        workSheet.Range(workSheet.Cell(row - accesses.Count, col), workSheet.Cell(row, col)).Merge().Value = grade != null ? grade.Name : "Chưa có xếp loại phù hợp";
        workSheet.Range(workSheet.Cell(row - accesses.Count, col), workSheet.Cell(row, col)).Merge().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        workSheet.Range(workSheet.Cell(row - accesses.Count, col), workSheet.Cell(row, col)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(8, 1), workSheet.Cell(row, col)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(8, 1), workSheet.Cell(row, col)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        workSheet.Columns().AdjustToContents();
        workSheet.RangeUsed().Style.Font.FontName = "Times New Roman";
        workSheet.RangeUsed().Style.Font.FontSize = 12;
        workSheet.Column("D").Width = 20;
        workSheet.Column("E").Width = 30;
        return workbook;
    }

    public async Task<XLWorkbook> GetSelfCriticismExcelFileNew(string schoolId, int month, int year, string userId, List<string> groupIds)
    {
        XLWorkbook workbook = new XLWorkbook();

        var teachers = new List<Teacher>();
        var teacher = await _teacherRepository.GetTeacherByUserId(userId);
        if (teacher != null && !_roles.Contains("Admin"))
        {
            var group = await _teacherGroupRepository.GetTeacherGroupById(teacher.GroupId);
            if (teacher.Id == group.LeaderId)
            {
                teachers = await _teacherRepository.GetTeachersOfGroup(teacher.GroupId);
                workbook = await CreateSheetOfTeacher(workbook, schoolId, teacher, month, year, group);
                workbook = await CreateSheetNew(workbook, teachers, month, year, schoolId, group);
            }
            else
            {
                teachers.Add(teacher);
                workbook = await CreateSheetOfTeacher(workbook, schoolId, teacher, month, year, group);
                //workbook = await CreateSheetNew(workbook, teachers, month, year, schoolId, group);
            }
        }
        else 
        {
            teachers = await _teacherRepository.GetAllTeachers();
            var  groups = new List<TeacherGroup>();
            if (groupIds == null || groupIds.Count <= 0)
            {
                groups = await _teacherGroupRepository.GetTeacherGroupsBySchoolId(schoolId);
            }
            else
            {
                groups = await _teacherGroupRepository.GetTeacherGroupsByIds(groupIds, schoolId);
            }

            foreach (var group in groups)
            {
                workbook = await CreateSheetNew(workbook, teachers.Where(x => x.GroupId == group.Id).ToList(), month, year, schoolId, group);
            }
            workbook = await GetAccessTeacherExcelFile(workbook, schoolId, month, year, userId);
        }
        return workbook;
    }

    public async Task<XLWorkbook> GetSampleExcelFile(int year)
    {
        XLWorkbook workbook = new XLWorkbook();
        var workSheet = workbook.Worksheets.Add("Mẫu thông tin tài khoản");
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1,4)).Merge().Value = "Sở Giáo dục và Đào tạo Hải Phòng";
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 4)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(2, 1), workSheet.Cell(2,4)).Merge().Value = "THPT Trần Nguyên Hãn";
        workSheet.Range(workSheet.Cell(2, 1), workSheet.Cell(2, 4)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(3, 1), workSheet.Cell(3,12)).Merge().Value = "DANH SÁCH GIÁO VIÊN";
        workSheet.Range(workSheet.Cell(3, 1), workSheet.Cell(3,12)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(3, 1), workSheet.Cell(3,12)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(4, 1), workSheet.Cell(4,12)).Merge().Value = $"{year - 1}-{year}";
        workSheet.Range(workSheet.Cell(4, 1), workSheet.Cell(4,12)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(4, 1), workSheet.Cell(4,12)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var titles = new List<string>()
        {
            "STT", "Họ tên", "Ngày sinh", "Giới tính", "Điện thoại", "Vị trí làm việc", "Môn dạy", "Nhóm chức năng", "Email",
            "CMTND", "Bậc lương", "Hệ số lương"
        };
        int row = 6;
        int col = 1;
        for (int i = 0; i < titles.Count; i++ )
        {
            workSheet.Cell(row, col + i).Value = titles[i];
            workSheet.Cell(row, col + i).Style.Font.Bold = true;
        }
        workSheet.Columns().AdjustToContents();
        workSheet.Range(workSheet.Cell(6, 1), workSheet.Cell(7, 12)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(6, 1), workSheet.Cell(7, 12)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        workSheet.RangeUsed().Style.Font.FontName = "Times New Roman";
        workSheet.RangeUsed().Style.Font.FontSize = 12;
        workSheet.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        return workbook;
    }

    public async Task<XLWorkbook> GetCompetitionBoard(GetCompetitionBoardRequest model)
    {
        XLWorkbook workbook = new XLWorkbook();
        var workSheet = workbook.Worksheets.Add("Báo cáo thi đua");
        var school = await _schoolRepository.GetById(model.SchoolId);
        var teachers = await _teacherRepository.GetTeachersOfGroup(model.GroupId);
        var accessments = await _assessmentCriteria.GetAssessmentCriteriasBySchoolId(model.SchoolId);
        var group = await _teacherGroupRepository.GetTeacherGroupById(model.GroupId);
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1,accessments.Count + 1)).Merge().Value = $"BẢNG ĐIỂM THI ĐUA THÁNG {model.Month} NĂM {model.Year}";
        workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1,accessments.Count + 1)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(2, 1), workSheet.Cell(2,accessments.Count + 1)).Merge().Value = $"Họ và tên: ";
        workSheet.Range(workSheet.Cell(2, 1), workSheet.Cell(2,accessments.Count + 1)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(3, 1), workSheet.Cell(3,accessments.Count + 1)).Merge().Value = $"{group.Name}";
        workSheet.Range(workSheet.Cell(3, 1), workSheet.Cell(3,accessments.Count + 1)).Merge().Style.Font.Bold = true;
        
        workSheet.Cell(5, 1).Value = "STT";
        workSheet.Cell(5, 1).Style.Font.Bold = true;
        workSheet.Cell(5, 2).Value = "Họ và tên";
        workSheet.Cell(5, 2).Style.Font.Bold = true;
        int col = 3; int row = 5;
        foreach (var accessment in accessments)
        {
            workSheet.Cell(row, col).Value = accessment.Name;
            workSheet.Cell(row, col++).Style.Font.Bold = true;
        }
        
        col = 3; row++;
        foreach (var teacher in teachers)
        {
            workSheet.Cell(row, 1).Value = row - 5;
            workSheet.Cell(row, 2).Value = teacher.Name;
            
            var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(teacher.Id, model.Month, model.Year);
            foreach (var selfCriticism in selfCriticisms)
            {
                if (selfCriticism.AssessmentCriterias.Any())
                {
                    foreach (var assessmentCriteria in selfCriticism.AssessmentCriterias)
                    {
                        var accessment = accessments.FirstOrDefault(x => x.Id == assessmentCriteria.Id);
                        if (accessment != null)
                        {
                            workSheet.Cell(row, accessments.IndexOf(accessment) + 2).Value = assessmentCriteria.TotalScore;
                        }
                    }
                }
            }
            row++; col = 3;
        }
        
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5 + teachers.Count, 2 + accessments.Count)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(5 + teachers.Count, 2 + accessments.Count)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        workSheet.RangeUsed().Style.Font.FontName = "Times New Roman";
        workSheet.RangeUsed().Style.Font.FontSize = 12;
        workSheet.Columns().Width = 10;
        workSheet.Column("A").AdjustToContents();
        workSheet.Column("B").AdjustToContents();
        workSheet.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        return workbook;
    }

    public async Task<XLWorkbook> GetPersonalArchievement(GetPersonalArchievementRequest model)
    {
        XLWorkbook workbook = new XLWorkbook();
        var teachers = await _teacherRepository.GetByIds(model.TeacherIds);
        foreach (var teacher in teachers)
        {
            var workSheet = workbook.Worksheets.Add(teacher.Name + "_" + teacher.User.UserName);
            if(model.Year != null)
                workSheet.Range(workSheet.Cell(1,1), workSheet.Cell(1,7)).Merge().Value = $"BẢNG ĐIỂM THI ĐUA THÁNG {model.Month} NĂM {model.Year}";
            else workSheet.Range(workSheet.Cell(1,1), workSheet.Cell(1,7)).Merge().Value = $"BẢNG ĐIỂM THI ĐUA NĂM {model.Year}";
            workSheet.Range(workSheet.Cell(1, 1), workSheet.Cell(1, 7)).Merge().Style.Font.Bold = true;
            workSheet.Range(workSheet.Cell(2, 1), workSheet.Cell(2, 7)).Merge().Value = $"Họ và tên: {teacher.Name}";
            workSheet.Range(workSheet.Cell(2, 1), workSheet.Cell(2, 7)).Merge().Style.Font.Bold = true;
            var group = await _teacherGroupRepository.GetTeacherGroupById(teacher.GroupId);
            if (group != null)
            {
                workSheet.Range(workSheet.Cell(3, 1), workSheet.Cell(3, 7)).Merge().Value = group.Name;
                workSheet.Range(workSheet.Cell(3, 1), workSheet.Cell(3, 7)).Merge().Style.Font.Bold = true;
            }

            int row = 6; int col = 1;
            workSheet.Cell(5, 1).Value = "STT";
            workSheet.Cell(5, 1).Style.Font.Bold = true;
            workSheet.Range(workSheet.Cell(5, 2), workSheet.Cell(5, 3)).Merge().Value = $"DIỄN GIẢI NỘI DUNG CÁC TIÊU CHÍ";
            workSheet.Range(workSheet.Cell(5, 2), workSheet.Cell(5, 3)).Merge().Style.Font.Bold = true;
            workSheet.Cell(5, 4).Value = "ĐIỂM CỘNG";
            workSheet.Cell(5, 4).Style.Font.Bold = true;
            workSheet.Cell(5, 5).Value = "ĐIỂM TRỪ";
            workSheet.Cell(5, 5).Style.Font.Bold = true;
            workSheet.Cell(5, 6).Value = "TỔNG";
            workSheet.Cell(5, 6).Style.Font.Bold = true;
            workSheet.Cell(5, 7).Value = "XẾP LOẠI";
            workSheet.Cell(5, 7).Style.Font.Bold = true;
            
            var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(teacher.Id, model.Month, model.Year);
            var acessments = selfCriticisms.SelectMany(x => x.AssessmentCriterias).ToList();
            for (int i = 0 ; i < acessments.Count ; i++)
            {
                col = 1;
                workSheet.Cell(row, col++).Value = i + 1;
                var accessMentGroup = await _assessmentCriteriaGroup.GetAssessmentCriterGroupById(acessments[i].AssessmentCriteriaGroupId);
                workSheet.Cell(row, col).Value = accessMentGroup.Name;
                workSheet.Cell(row, col++).Style.Font.Bold = true;
                workSheet.Cell(row, col++).Value = acessments[i].Name;
                workSheet.Cell(row, col).Style.Font.Bold = true;
                workSheet.Cell(row, col++).Value = !acessments[i].IsDeduct ? acessments[i].Value : "";
                workSheet.Cell(row, col).Style.Font.Bold = true;
                workSheet.Cell(row, col++).Value = acessments[i].IsDeduct ? acessments[i].Value : "";
                row++;
            }
            workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(row, 7)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            workSheet.Range(workSheet.Cell(5, 1), workSheet.Cell(row, 7)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            var total1 = acessments.Where(x => !x.IsDeduct).Sum(x => x.Value);
            var total2 = acessments.Where(x => x.IsDeduct).Sum(x => x.Value);
            workSheet.Cell(row, 4).Value = total1;
            workSheet.Cell(row, 4).Style.Font.Bold = true;
            workSheet.Cell(row, 5).Value = total2;
            workSheet.Cell(row, 5).Style.Font.Bold = true;
            workSheet.Range(workSheet.Cell(6, 6), workSheet.Cell(row, 6)).Merge().Value = total1 - total2;
            workSheet.Range(workSheet.Cell(6, 6), workSheet.Cell(row, 6)).Style.Font.Bold = true;
            workSheet.Range(workSheet.Cell(6, 6), workSheet.Cell(row, 6)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            workSheet.Range(workSheet.Cell(6, 7), workSheet.Cell(row, 7)).Merge().Value = "";
            workSheet.Range(workSheet.Cell(6, 7), workSheet.Cell(row, 7)).Style.Font.Bold = true;
            workSheet.Range(workSheet.Cell(6, 7), workSheet.Cell(row, 7)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            workSheet.Columns().AdjustToContents();
            
            workSheet.RangeUsed().Style.Font.FontName = "Times New Roman";
            workSheet.RangeUsed().Style.Font.FontSize = 10;
            workSheet.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        return workbook;
    }
}