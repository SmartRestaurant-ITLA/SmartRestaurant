
using SmartRestaurant.Application.DTOs.Tables;

namespace SmartRestaurant.Web.ViewModels
{
    public class TableListItemViewModel
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; }

        public TableOperationalState OperationalState { get; set; }
    }
}
