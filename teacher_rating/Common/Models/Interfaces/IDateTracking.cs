namespace teacher_rating.Common.Models.Interfaces
{
    public interface IDateTracking
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
