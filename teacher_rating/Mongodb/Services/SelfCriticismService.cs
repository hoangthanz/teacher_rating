using ClosedXML.Excel;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Mongodb.Services;

public interface ISelfCriticismService
{
    Task<XLWorkbook> GetSelfCriticismExcelFile(string schoolId, int month, int year);
}

public class SelfCriticismService : ISelfCriticismService
{
    private readonly ISelfCriticismRepository _selfCriticismRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IGradeConfigurationRepository _gradeConfigurationRepository;

    public SelfCriticismService(ISelfCriticismRepository selfCriticismRepository, ITeacherRepository teacherRepository, IGradeConfigurationRepository gradeConfigurationRepository)
    {
        _selfCriticismRepository = selfCriticismRepository;
        _teacherRepository = teacherRepository;
        _gradeConfigurationRepository = gradeConfigurationRepository;
    }

    public async Task<XLWorkbook> GetSelfCriticismExcelFile(string schoolId, int month, int year)
    {
        // XLWorkbook workbook = new XLWorkbook();
        // var workSheet = workbook.Worksheets.Add();
        // var row = 7;
        // var col = 1;
        // var teachers = await _teacherRepository.GetAllTeachers();
        // var Titles = new[]
        // {
        //     "STT",
        //     "Họ và tên",
        //     "Tổng điểm",
        //     "Xếp loại",
        // };
        // foreach (var title in Titles)
        // {
        //     workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row + 1, col++)).Merge().Value = title;
        // }
        //
        // workSheet.Range(workSheet.Cell(row, col), workSheet.Cell(row, col + 1)).Merge().Value = "Điểm cộng";
        // workSheet.Range(workSheet.Cell(row, col + 2), workSheet.Cell(row, col + 3)).Merge().Value = "Điểm trừ";
        // workSheet.Cell(row + 1, col).Value = "Số điểm";
        // workSheet.Cell(row + 1, col + 1).Value = "Lí do";
        // workSheet.Cell(row + 1, col + 2).Value = "Số điểm";
        // workSheet.Cell(row + 1, col + 3).Value = "Lí do";
        //
        // row = row + 1;
        // col = 1;
        //
        // for (var i = 0 ; i < teachers.Count(); i++)
        // {
        //     workSheet.Cell(row + i, col++).Value = (i + 1).ToString();
        //     workSheet.Cell(row + i, col++).Value = teachers[i].Name;
        //     var selfCriticisms = await _selfCriticismRepository.GetSelfCriticismsByTeacher(teachers[i].Id, month, year);
        //     var total = (int)selfCriticisms.Sum(x => x.TotalScore);
        //     workSheet.Cell(row + i, col++).Value = total;
        //     var grade = await _gradeConfigurationRepository.GetGradeConfigurationByScore(total, schoolId);
        //     if (grade != null)
        //     {
        //         workSheet.Cell(row + i, col++).Value = grade.Name;
        //     }
        //     else
        //     {
        //         col = col + 1;
        //     }
        //
        //     var plus = selfCriticisms.SelectMany(x => x.AssessmentCriterias).Where(e => !e.IsDeduct).ToList();
        //     var sub = selfCriticisms.SelectMany(x => x.AssessmentCriterias).Where(e => e.IsDeduct).ToList();
        //
        //     var plusStringNumber = "";
        //     plus.ForEach(x =>
        //     {
        //         plusStringNumber += x.Value
        //     });
        // }
        throw new NotImplementedException();
    }
}