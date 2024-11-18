using System.ComponentModel.DataAnnotations.Schema;

namespace OrderServices.Domain.Entity
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CreateUser { get; set; }
        public DateTime CreataDate { get; set; }
        public Guid UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
