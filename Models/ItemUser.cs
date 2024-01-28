using System.ComponentModel.DataAnnotations; // tag Key
using System.ComponentModel.DataAnnotations.Schema; // tag Table

namespace viettel_store.Models
{
    // Class sẽ tác động đến table đó
    [Table("Users")]
    public class ItemUser
    {
        [Key]
        public int Id { get; set; }
        // dấu ? thể hiện trường này có thể null (trường nào nếu trong csdl có thể có giá trị null thì có thể có dấu ?, nếu không thì sẽ bị báo lỗi)
        public string ? Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
