// See https://aka.ms/new-console-template for more information

using DemoCSharp;

// Console.WriteLine("Hello, World!");

Student student = new Student();
student.Fullname = "Hust";
student.Code = "20194141";
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine(student.Fullname + "-" + student.Code);
Console.WriteLine($"Họ và tên: {student.Fullname} \nMã số sinh viên: {student.Code}");
//Console.ReadLine();

////////
Person<Student> person = new Person<Student>("NEU");
var personName = person.Getname();
Console.WriteLine(personName);
Woman woman = new Woman();
string typeName = (new Person<Woman>("Hust")).getType(woman);
Console.WriteLine(typeName);
