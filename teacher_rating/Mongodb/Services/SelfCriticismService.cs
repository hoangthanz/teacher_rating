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
        XLWorkbook workbook = new XLWorkbook();
        var workSheet = workbook.Worksheets.Add();
        var row = 7;
        var col = 1;
        workSheet.Range(workSheet.Cell(5, 3), workSheet.Cell(5, 6)).Merge().Value = $"KẾT QUẢ THI ĐUA THÁNG {month}/{year}";
        workSheet.Range(workSheet.Cell(5, 3), workSheet.Cell(5, 6)).Merge().Style.Font.Bold = true;
        workSheet.Range(workSheet.Cell(5, 3), workSheet.Cell(5, 6)).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        workSheet.Range(workSheet.Cell(5, 3), workSheet.Cell(5, 6)).Merge().Style.Font.FontSize = 25;
        var teachers = await _teacherRepository.GetAllTeachers();
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
        //throw new NotImplementedException();
    }
}