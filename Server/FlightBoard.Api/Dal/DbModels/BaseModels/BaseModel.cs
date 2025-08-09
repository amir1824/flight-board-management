using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBoard.Api.Dal.DbModels.BaseModels;

public abstract class BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; }
}