using ClosedXML.Excel;
using teacher_rating.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Services;

public interface ITeacherService
{
    Task<XLWorkbook> GetAccessTeacherExcelFile(string schoolId, int month, int year, string userId);
    Task<bool> CheckTeacherOfGrade(Teacher teacher, string gradeId, int month, int year, string schoolId);
    Task<List<TeachersOfGradeOfGroup>> GetTeachersOfGradeOfGroup(List<GradeConfiguration> grades, List<Teacher> teachers, List<TeacherGroup> teacherGroups, int month, int year);
}

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IAssessmentCriteriaRepository _assessmentCriteriaRepository;
    private readonly IGradeConfigurationRepository _gradeConfigurationRepository;
    private readonly ISelfCriticismRepository _selfCriticismRepository;
    private readonly ITeacherGroupRepository _teacherGroupRepository;

    public TeacherService(ITeacherRepository teacherRepository,
        IAssessmentCriteriaRepository assessmentCriteriaRepository,
        IGradeConfigurationRepository gradeConfigurationRepository, ISelfCriticismRepository selfCriticismRepository, ITeacherGroupRepository teacherGroupRepository)
    {
        _teacherRepository = teacherRepository;
        _assessmentCriteriaRepository = assessmentCriteriaRepository;
        _gradeConfigurationRepository = gradeConfigurationRepository;
        _selfCriticismRepository = selfCriticismRepository;
        _teacherGroupRepository = teacherGroupRepository;
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
}