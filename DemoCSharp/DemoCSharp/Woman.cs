using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCSharp
{
    public class Woman : Person<Woman>
    {
        // const không thể thay đổi, có thê gọi trực tiếp từ class ex var gen = Woman.Gender // gen = "Nữ"
        public const string Gender = "Nữ";
        // statc có thể thay đổi ở bên ngoài, gọi trực tiếp từ class
        public static string Address = "Hà Nội";
        // readonly có thể thay đổi trong hàm khở tạo class, gọi từ đối tượng khi ở ngoài
        public readonly string Dob = "18/01/2003"; 
        public Woman() : base("Girl") { }
        //public Woman(string name) : base(name) { }
    }
}
