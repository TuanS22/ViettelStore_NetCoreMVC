using System.ComponentModel.DataAnnotations; // tag Key
using System.ComponentModel.DataAnnotations.Schema; // tag Table

namespace viettel_store.Models
{
    // Class sẽ tác động đến table nào
    [Table("Categories")]
    public class ItemCategory
    {
        // Định nghĩa key
        [Key]
        public int Id { get; set; }
        // dấu ? thể hiện trường này có thể null (trường nào nếu trong csdl có thể có giá trị null thì có thể có dấu ?, nếu không thì sẽ bị báo lỗi)
        public int ParentId { get; set; }
        public string Name { get; set; }
        public int DisplayHomePage { get; set; }
    }
}
