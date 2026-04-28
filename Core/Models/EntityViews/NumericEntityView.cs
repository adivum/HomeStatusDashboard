namespace Core.Models.EntityViews
{
    public class NumericEntityView : EntityViewBase
    {
        public decimal Value { get; set; }
        public string? Unit { get; set; }
    }
}
