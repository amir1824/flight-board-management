using FlightBoard.Api.Dal.DbModels.BaseModels; 

namespace FlightBoard.Api.Dal.DbModels.BaseModels
{
    public abstract class CRUDModel : BaseModel
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
