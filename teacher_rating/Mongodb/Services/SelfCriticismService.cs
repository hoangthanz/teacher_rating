using System.Security.Claims;
using ClosedXML.Excel;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Services;

public interface ISelfCriticismService
{
    Task<XLWorkbook> GetSelfCriticismExcelFile(string schoolId, int month, int year, string userId, List<string> groupIds);
}

public class SelfCriticismService : ISelfCriticismService
{
    private readonly ISelfCriticismRepository _selfCriticismRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IGradeConfigurationRepository _gradeConfigurationRepository;
    private readonly ITeacherGroupRepository _teacherGroupRepository;

    public SelfCriticismService(ISelfCriticismRepository selfCriticismRepository, ITeacherRepository teacherRepository, IGradeConfigurationRepository gradeConfigurationRepository
    ,IHttpContextAccessor httpContext, ITeacherGroupRepository teacherGroupRepository)
    {
        _selfCriticismRepository = selfCriticismRepository;
        _teacherRepository = teacherRepository;
        _gradeConfigurationRepository = gradeConfigurationRepository;
        _teacherGroupRepository = teacherGroupRepository;
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
                workSheet.Cell(row, col++).Value = grade.Name;
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
}